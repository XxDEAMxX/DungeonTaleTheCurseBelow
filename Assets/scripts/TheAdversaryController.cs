using UnityEngine;
using System.Collections;
using System.Collections.Generic;


public enum TheAdversaryState { Init, Idle, Attack, Follow, Dead };

public class TheAdversaryController : MonoBehaviour
{
    private int life;
    private Animator animator;
    private GameObject player;
    TheAdversaryState curreState = TheAdversaryState.Idle;
    public float speed;
    public bool notInRoom = false;
    private bool isInitFinish = false;


    public GameObject childPrefab; // Asigna el prefab del "hijo" desde el Inspector
    public Transform spawnPoint; // Punto donde se genera el hijo

    private Queue<System.Action> generationQueue = new Queue<System.Action>();
    private bool hasStartedChildGeneration = false;

    private int currentChildren = 0;
    public int maxChildren = 10;


    void Start()
    {
        life = 100;
        animator = GetComponent<Animator>();
        player = GameObject.FindGameObjectWithTag("Player");
        StartCoroutine(ProcessQueue());
    }

    void Update()
    {
        Debug.Log(curreState);

        if (curreState == TheAdversaryState.Dead)
            return;

        if (curreState != TheAdversaryState.Dead &&
            curreState != TheAdversaryState.Attack &&
            curreState != TheAdversaryState.Follow &&
            curreState != TheAdversaryState.Init && !isInitFinish)
        {
            if (notInRoom && curreState != TheAdversaryState.Init)
            {
                curreState = TheAdversaryState.Init;
                isInitFinish = true;
            }
            else
                curreState = TheAdversaryState.Idle;
        }

        switch (curreState)
        {
            case TheAdversaryState.Idle:
                Idle();
                break;
            case TheAdversaryState.Init:
                Init();
                break;
            case TheAdversaryState.Attack:
                Attack();
                break;
            case TheAdversaryState.Follow:
                Follow();
                break;
            case TheAdversaryState.Dead:
                // Death();
                break;
        }
    }

    IEnumerator ProcessQueue()
    {
        while (true)
        {
            if (generationQueue.Count > 0)
            {
                var action = generationQueue.Dequeue();
                action.Invoke();
            }
            yield return null;
        }
    }

    void generateSingleChild()
    {
        if (childPrefab != null && spawnPoint != null && currentChildren < maxChildren)
        {
            animator.SetBool("isIdle", false);
            animator.SetBool("isAttack", false);
            animator.SetBool("isGenerateChild", true);
            animator.SetBool("isAttackUp", false);
            animator.SetBool("isAttackDown", false);
            animator.SetBool("isInit", false);

            Vector2 spawnOffset = Random.insideUnitCircle * 0.5f;
            Vector3 spawnPosition = spawnPoint.position + (Vector3)spawnOffset;

            Instantiate(childPrefab, spawnPosition, Quaternion.identity);
            currentChildren++;

            Debug.Log($"1 hijo generado. Total: {currentChildren}");
        }
    }

    IEnumerator EnqueueChildGeneration()
    {
        while (curreState != TheAdversaryState.Dead)
        {
            //Todo: rando
            float randomInterval = Random.Range(0f, 7f); // tiempo aleatorio entre 0 y 7 segundos
            yield return new WaitForSeconds(randomInterval);

            if (currentChildren < maxChildren)
            {
                generationQueue.Enqueue(() => generateSingleChild());
            }
        }
    }


    private void Idle()
    {
        animator.SetBool("isIdle", true);
        animator.SetBool("isAttack", false);
        animator.SetBool("isGenerateChild", false);
        animator.SetBool("isAttackUp", false);
        animator.SetBool("isAttackDown", false);
        animator.SetBool("isInit", false);
    }

    private void Attack()
    {
        animator.SetBool("isIdle", false);
        animator.SetBool("isAttack", true);
        animator.SetBool("isGenerateChild", false);
        animator.SetBool("isInit", false);

        if (player.transform.position.y > transform.position.y)
        {
            animator.SetBool("isAttackUp", true);
            animator.SetBool("isAttackDown", false);
        }
        else if (player.transform.position.y < transform.position.y)
        {
            animator.SetBool("isAttackDown", true);
            animator.SetBool("isAttackUp", false);
        }
        else
        {
            animator.SetBool("isAttackUp", false);
            animator.SetBool("isAttackDown", false);
        }
    }

    public void toIdle()
    {
        animator.SetBool("isInit", false);
        animator.SetBool("isIdle", true);
        animator.SetBool("isAttack", false);
        animator.SetBool("isGenerateChild", false);
        animator.SetBool("isAttackUp", false);
        animator.SetBool("isAttackDown", false);
        curreState = TheAdversaryState.Follow;
    }

    private void Init()
    {
        animator.SetBool("isInit", true);
        animator.SetBool("isIdle", false);
        animator.SetBool("isAttack", false);
        animator.SetBool("isGenerateChild", false);
        animator.SetBool("isAttackUp", false);
        animator.SetBool("isAttackDown", false);

        if (!hasStartedChildGeneration)
        {
            StartCoroutine(EnqueueChildGeneration());
            hasStartedChildGeneration = true;
        }
    }

    private void Follow()
    {
        animator.SetBool("isIdle", true);
        animator.SetBool("isAttack", false);
        animator.SetBool("isGenerateChild", false);
        animator.SetBool("isAttackUp", false);
        animator.SetBool("isAttackDown", false);
        animator.SetBool("isInit", false);
        Vector2 direction = (player.transform.position - transform.position).normalized;
        transform.position = Vector2.MoveTowards(transform.position, player.transform.position, speed * Time.deltaTime);
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Sword") || collision.CompareTag("Bullet"))
        {
            life--;
            if (life <= 0)
            {
                curreState = TheAdversaryState.Dead;
                animator.SetBool("isDeath", true);
            }

        }
        if (collision.CompareTag("BombRange"))
        {
            curreState = TheAdversaryState.Dead;
            animator.SetBool("isDeath", true);
        }
    }

    private void OnCollisionEnter2D(Collision2D other) {
        if (other.gameObject.CompareTag("Player"))
        {
            Vector2 rawDir = (other.transform.position - transform.position);
            Vector2 direction = Mathf.Abs(rawDir.x) > Mathf.Abs(rawDir.y)
                ? new Vector2(Mathf.Sign(rawDir.x), 0)
                : new Vector2(0, Mathf.Sign(rawDir.y));
            GameManager.instance.DecreaseLife(direction);
        }
    }
    
    void Death()
    {
        RoomController.instance.StartCoroutine(RoomController.instance.RoomCoroutine());
        Destroy(gameObject);
        GameManager.instance.AddPoint(1);
    }
}
