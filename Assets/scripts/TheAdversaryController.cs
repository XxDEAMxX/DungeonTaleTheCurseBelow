using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;


public enum TheAdversaryState { Init, Idle, Attack, Follow, Dead, GenerateChild };

public class TheAdversaryController : MonoBehaviour
{
    public static TheAdversaryController instance;
    void Awake() {
            instance = this;
    } 
    private int life;
    private Animator animator;
    private GameObject player;
    TheAdversaryState curreState = TheAdversaryState.Idle;
    public float speed;
    public bool notInRoom = false;
    private bool isInitFinish = false;
    private Rigidbody2D rb;


    public GameObject childPrefab; // Asigna el prefab del "hijo" desde el Inspector
    public Transform spawnPoint; // Punto donde se genera el hijo

    private Queue<System.Action> generationQueue = new Queue<System.Action>();
    private bool hasStartedChildGeneration = false;

    private int currentChildren = 0;
    public int maxChildren = 10;
    public bool isGenerateChild = false;


    
    private bool canDealDamage = true;
    private float damageCooldown = 1f; 

    private bool isTouchingPlayer = false;
    private float damageInterval = 1f;
    private float damageTimer = 0f;
    private float lastDamageTime = -999f;


    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        life = 2;
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
                stop();
                break;
        }
    }

    private void stop()
    {
        // rb.linearVelocity = Vector2.zero;
        // rb.bodyType = RigidbodyType2D.Static;
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
            transform.rotation = Quaternion.Euler(0, 0, 0);

            rb.linearVelocity = Vector2.zero;
            curreState = TheAdversaryState.GenerateChild;
            isGenerateChild = true;
            animator.SetBool("isGenerateChild", true);
            animator.SetBool("isIdle", false);
            animator.SetBool("isAttack", false);
            animator.SetBool("isInit", false);
        }
    }

    public void InitialChild()
    {
        Vector2 spawnOffset = Random.insideUnitCircle * 0.5f;
        Vector3 spawnPosition = spawnPoint.position + (Vector3)spawnOffset;

        Instantiate(childPrefab, spawnPosition, Quaternion.identity);
        currentChildren++;        
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
                curreState = TheAdversaryState.GenerateChild;

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
        isGenerateChild = false; 
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
        if (curreState != TheAdversaryState.Follow && !isGenerateChild) return; // seguridad extra
        // rb = rbTmp;

        animator.SetBool("isIdle", false);
        animator.SetBool("isAttack", true);
        animator.SetBool("isGenerateChild", false);
        animator.SetBool("isInit", false);

        Vector2 direction = (player.transform.position - transform.position).normalized;

        transform.position = Vector2.MoveTowards(transform.position, player.transform.position, speed * Time.deltaTime);

        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0, 0, angle + 90f); // mirar hacia abajo
    }


    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Sword") || collision.CompareTag("Bullet"))
        {
            life--;
            if (life <= 0)
            {
                transform.rotation = Quaternion.Euler(0, 0, 0);
                rb.linearVelocity = Vector2.zero;
                GetComponent<BoxCollider2D>().isTrigger = true;
                curreState = TheAdversaryState.Dead;
                animator.SetBool("isDeath", true);
                if (GameManager.instance != null)
                {
                    GameManager.instance.WinGame();
                }
                else
                {
                    Debug.LogError("GameManager.instance no está asignado en TheAdversaryController. No se puede llamar a WinGame().");
                }
                GameObject playerObject = GameObject.FindGameObjectWithTag("Player");
                if (playerObject != null)
                {
                    playerObject.SetActive(false);
                }
                else
                {
                    Debug.LogWarning("No se encontró ningún GameObject con la etiqueta 'Player' para desactivar.");
                }
            }

        }
        if (collision.CompareTag("BombRange"))
        {
            curreState = TheAdversaryState.Dead;
            animator.SetBool("isDeath", true);
        }
    }

    public void setDead()
    { 
        animator.SetBool("isDead", true);
    }

    private void OnCollisionEnter2D(Collision2D other)
    {
        if (other.gameObject.CompareTag("Player") && canDealDamage)
        {
            Vector2 rawDir = (other.transform.position - transform.position);
            Vector2 direction = Mathf.Abs(rawDir.x) > Mathf.Abs(rawDir.y)
                ? new Vector2(Mathf.Sign(rawDir.x), 0)
                : new Vector2(0, Mathf.Sign(rawDir.y));

            GameManager.instance.DecreaseLife(direction);
            StartCoroutine(DamageCooldown());
        }
    }

    private void OnCollisionStay2D(Collision2D other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            Vector2 rawDir = (other.transform.position - transform.position);
            Vector2 direction = Mathf.Abs(rawDir.x) > Mathf.Abs(rawDir.y)
                ? new Vector2(Mathf.Sign(rawDir.x), 0)
                : new Vector2(0, Mathf.Sign(rawDir.y));
            if (Time.time - lastDamageTime >= damageCooldown)
            {
                GameManager.instance.DecreaseLife(direction);
                lastDamageTime = Time.time;
            }
        }
    }

    private IEnumerator DamageCooldown()
    {
        canDealDamage = false;
        yield return new WaitForSeconds(damageCooldown);
        canDealDamage = true;
    }

    private void OnCollisionExit2D(Collision2D other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            isTouchingPlayer = false;
            damageTimer = 0f;
        }
    }
    
    void Death()
    {
        RoomController.instance.StartCoroutine(RoomController.instance.RoomCoroutine());
        Destroy(gameObject);
        GameManager.instance.AddPoint(1);
    }
}
