using UnityEngine;
using System.Collections;
using System.Collections.Generic;


public enum TheAdversaryState { Init, Idle, Attack, Follow, Dead, GenerateChild };

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
        Debug.Log("Current State: " + curreState);
        if (curreState == TheAdversaryState.Dead)
            return;

        if (!isInitFinish && notInRoom)
        {
            Debug.Log("Solo one");
            curreState = TheAdversaryState.Init;
            isInitFinish = true;
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
                break;
            case TheAdversaryState.Follow:
                Follow();
                break;
            case TheAdversaryState.Dead:
                // Death();
                break;
            case TheAdversaryState.GenerateChild:
                // generateSingleChild();
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
            curreState = TheAdversaryState.GenerateChild;
            animator.SetBool("isGenerateChild", true);
            animator.SetBool("isIdle", false);
            animator.SetBool("isAttack", false);
            animator.SetBool("isInit", false);

            Vector2 spawnOffset = Random.insideUnitCircle * 0.5f;
            Vector3 spawnPosition = spawnPoint.position + (Vector3)spawnOffset;

            Instantiate(childPrefab, spawnPosition, Quaternion.identity);
            currentChildren++;
        }
    }

    IEnumerator EnqueueChildGeneration()
    {
        while (curreState != TheAdversaryState.Dead)
        {
            //Todo: rando
            float randomInterval = Random.Range(0f, 7f);
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
        animator.SetBool("isInit", false);
    }

    public void toFollow()
    {
        animator.SetBool("isInit", false);
        animator.SetBool("isIdle", false);
        animator.SetBool("isAttack", true);
        animator.SetBool("isGenerateChild", false);
        curreState = TheAdversaryState.Follow;
    }

    private void Init()
    {
        animator.SetBool("isInit", true);
        animator.SetBool("isIdle", false);
        animator.SetBool("isAttack", false);
        animator.SetBool("isGenerateChild", false);

        if (!hasStartedChildGeneration)
        {
            StartCoroutine(EnqueueChildGeneration());
            hasStartedChildGeneration = true;
        }
    }

    private void Follow()
    {
        animator.SetBool("isIdle", false);
        animator.SetBool("isAttack", true);
        animator.SetBool("isGenerateChild", false);
        animator.SetBool("isInit", false);
        Vector2 direction = (player.transform.position - transform.position).normalized;
        transform.position = Vector2.MoveTowards(transform.position, player.transform.position, speed * Time.deltaTime);
        // Rotar hacia el jugador (considerando que el sprite mira hacia abajo por defecto)
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0, 0, angle + 90 ); // -90 porque el sprite mira hacia abajo
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
