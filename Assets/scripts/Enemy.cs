using System;
using System.Collections;
using Unity.Mathematics;
using UnityEngine;

public enum EnemyState { Idle, Wander, Follow, Dead };
public enum EnemyType { Melee, Ranged };
public class Enemy : MonoBehaviour
{
    private int life ;
    private Animator animator;
    private GameObject player;
    public GameObject bulletPrefab;
    public EnemyType enemyType;
    EnemyState curreState = EnemyState.Idle;
    public float range;
    public float speed;
    private Vector2 randomDirection;
    private float wanderTimer = 0f;
    private float waitTimer = 5f;
    private bool isWaiting = false;
    public bool notInRoom = false;
    private bool coolAtack = false;

    private bool canDealDamage = true;
    private float damageCooldown = 1f; 

    // private bool isTouchingPlayer = false;
    private float damageInterval = 1f;
    // private float damageTimer = 0f;
    private float lastDamageTime = -999f;

    void Start()
    { 
        switch (enemyType)
        {
            case EnemyType.Melee:
                life = 3;
                break;
            case EnemyType.Ranged:
                life = 5;
                break;
        }  
        animator = GetComponent<Animator>();
        player = GameObject.FindGameObjectWithTag("Player");
    }
    void Update()
    {
        if (curreState == EnemyState.Dead)
        return;
        // if (isTouchingPlayer)
        // {
        //     damageTimer += Time.deltaTime;
        //     if (damageTimer >= damageInterval)
        //     {
        //         Vector2 rawDir = (player.transform.position - transform.position);
        //         Vector2 direction = Mathf.Abs(rawDir.x) > Mathf.Abs(rawDir.y)
        //             ? new Vector2(Mathf.Sign(rawDir.x), 0)
        //             : new Vector2(0, Mathf.Sign(rawDir.y));
                
        //         GameManager.instance.DecreaseLife(direction);
        //         damageTimer = 0f; // reinicia para el siguiente intervalo
        //     }
        // }
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

        if (!notInRoom)
        {
            if (enemyType == EnemyType.Ranged && !coolAtack)
            {
                if (isPlayerInRange(5))
                {
                    Debug.Log("Attack");
                    Attack();
                }
            }
            else if (isPlayerInRange(range) && enemyType != EnemyType.Ranged)
            {
                curreState = EnemyState.Follow;
            }
            else
            {
                curreState = EnemyState.Wander;
            }
        }
        else
        {
            curreState = EnemyState.Idle;
        }
    }

    private IEnumerator DamageCooldown()
    {
        canDealDamage = false;
        yield return new WaitForSeconds(damageCooldown);
        canDealDamage = true;
    }

    private void Attack()
    {
        GameObject bullet = Instantiate(bulletPrefab, transform.position, Quaternion.identity) as GameObject;
        bullet.GetComponent<BulletController>().GetPlayer(player.transform);
        bullet.AddComponent<Rigidbody2D>().gravityScale = 0;
        bullet.GetComponent<BulletController>().isEnemyBullet = true;
        StartCoroutine(WaitToAttack());
    }

    private IEnumerator WaitToAttack()
    {
        coolAtack = true;
        yield return new WaitForSeconds(2f);
        coolAtack = false;
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
                randomDirection = EnemyRandomExtension.GetRandomDirection();
                wanderTimer = EnemyRandomExtension.GetWanderTime(); // duración del movimiento
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
            waitTimer = EnemyRandomExtension.GetWaitTime(); // tiempo quieto
        }

        if (isPlayerInRange(range))
        {
            curreState = EnemyState.Follow;
        }
    }

    void Follow()
    {
        if (!isPlayerInRange(range))
        {
            curreState = EnemyState.Wander;
            return;
        }
        transform.position = Vector2.MoveTowards(transform.position, player.transform.position, speed * Time.deltaTime);
    }

    // private void OnCollisionEnter2D(Collision2D other)
    // {
    //     if (other.gameObject.CompareTag("Player") && canDealDamage)
    //     {
    //         Vector2 rawDir = (other.transform.position - transform.position);
    //         Vector2 direction = Mathf.Abs(rawDir.x) > Mathf.Abs(rawDir.y)
    //             ? new Vector2(Mathf.Sign(rawDir.x), 0)
    //             : new Vector2(0, Mathf.Sign(rawDir.y));

    //         GameManager.instance.DecreaseLife(direction);
    //         StartCoroutine(DamageCooldown());
    //     }
    // }

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

    // private void OnCollisionExit2D(Collision2D other)
    // {
    //     if (other.gameObject.CompareTag("Player"))
    //     {
    //         isTouchingPlayer = false;
    //         damageTimer = 0f;
    //     }
    // }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("BulletSida") || collision.CompareTag("Bullet"))
        {
            if (collision.CompareTag("Bullet"))
            {
                life--;
            } else {
                life = life - 2;
            } 
            if (life <= 0)
            {
                GetComponent<BoxCollider2D>().isTrigger = true;
                curreState = EnemyState.Dead;
                animator.SetBool("BlDeath", true);
                if (enemyType == EnemyType.Ranged)
                {
                    RoomController.instance.StartCoroutine(RoomController.instance.RoomCoroutine());
                    Destroy(gameObject);
                }
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
