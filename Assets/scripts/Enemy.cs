using System;
using System.Collections;
using Unity.Mathematics;
using UnityEngine;

public enum EnemyState { Idle, Wander, Follow, Dead };
public class Enemy : MonoBehaviour
{
    private int life ;
    private Animator animator;
    private GameObject player;
    EnemyState curreState = EnemyState.Idle;
    public float range;
    public float speed;
    private Vector2 randomDirection;
    private float wanderTimer = 0f;
    private float waitTimer = 5f;
    private bool isWaiting = false;
    public bool notInRoom = false;


    void Start()
    {   
        life = 1;
        animator = GetComponent<Animator>();
        player = GameObject.FindGameObjectWithTag("Player");
    }
    void Update()
    {
        if (curreState == EnemyState.Dead)
        return;
        switch (curreState)
        {
            case EnemyState.Idle:
                Idle();
                break;
            case EnemyState.Wander:
                Wander();
                break;
            case EnemyState.Follow:
                Follow();
                break;
            case EnemyState.Dead:
                // Death();
                break;
        }
        if (!notInRoom){
            if (isPlayerInRange(range))
            {
                curreState = EnemyState.Follow;
            }
            else
            {
                curreState = EnemyState.Wander;
            }
        } else {
            curreState = EnemyState.Idle;
        }
    }

    private bool isPlayerInRange(float range)
    {
        return Vector2.Distance(transform.position, player.transform.position) < range;
    }

    void Idle()
    {
    }

    void Wander()
    {
        if (isWaiting)
        {
            waitTimer -= Time.deltaTime;
            if (waitTimer <= 0f)
            {
                //Todo: Random
                // Empezar a moverse
                isWaiting = false;
                randomDirection = new Vector2(UnityEngine.Random.Range(-1f, 1f), UnityEngine.Random.Range(-1f, 1f)).normalized;
                wanderTimer = UnityEngine.Random.Range(1f, 2f); // duración del movimiento
            }
            return; // No moverse mientras espera
        }

        // Movimiento
        transform.position += (Vector3)(randomDirection * speed * Time.deltaTime);
        wanderTimer -= Time.deltaTime;

        if (wanderTimer <= 0f)
        {
            // Detenerse y esperar
            isWaiting = true;
            waitTimer = UnityEngine.Random.Range(0.5f, 1.5f); // tiempo quieto
        }

        if (isPlayerInRange(range))
        {
            curreState = EnemyState.Follow;
        }
    }

    void Follow()
    {
        transform.position = Vector2.MoveTowards(transform.position, player.transform.position, speed * Time.deltaTime);
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

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Sword") || collision.CompareTag("Bullet"))
        {
            life--;
            if (life <= 0)
            {
                GetComponent<BoxCollider2D>().isTrigger = true;
                curreState = EnemyState.Dead;   
                animator.SetBool("BlDeath", true);
            }
            
        }  
        if (collision.CompareTag("BombRange"))
        {
            curreState = EnemyState.Dead;   
            animator.SetBool("BlDeath", true);
        }
    }

    void Death()
    {
        RoomController.instance.StartCoroutine(RoomController.instance.RoomCoroutine());
        Destroy(gameObject);
        GameManager.instance.AddPoint(1);
    }
}
