using System;
using System.Collections;
using Unity.Mathematics;
using UnityEngine;

public enum EnemyState { Wander, Follow, Dead };
public class Enemy : MonoBehaviour
{
    private int life ;
    private Animator animator;
    private GameObject player;
    EnemyState curreState = EnemyState.Wander;
    public float range;
    public float speed;
    private bool chooseDirection = false;

    void Start()
    {   
        life = 5;
        animator = GetComponent<Animator>();
        player = GameObject.FindGameObjectWithTag("Player");
    }
    void Update()
    {
        if (curreState == EnemyState.Dead)
        return;
        switch (curreState)
        {
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
        if (isPlayerInRange(range))
        {
            curreState = EnemyState.Follow;
        }
        else
        {
            curreState = EnemyState.Wander;
        }
    }

    private bool isPlayerInRange(float range)
    {
        return Vector2.Distance(transform.position, player.transform.position) < range;
    }

    private IEnumerator ChooseDirection()
    {
        chooseDirection = true;
        //Todo: Random
        yield return new WaitForSeconds(UnityEngine.Random.Range(1f, 3f));
        chooseDirection = false;
    }

    void Wander(){
        if (!chooseDirection)
        {
            chooseDirection = true;
        }
        transform.position += transform.right * speed * Time.deltaTime;	
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
            curreState = EnemyState.Dead;   
            animator.SetBool("BlDeath", true);
            }
            
        }  
    }

    void Death()
    {
        Destroy(gameObject);
        GameManager.instance.AddPoint(1);
    }
}
