
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Collections;

public class BulletController : MonoBehaviour
{
    public float lifeTime = 2f; // Tiempo de vida de la bala
    private Rigidbody2D rb;

    private Animator animator;
    public bool isEnemyBullet = false;
    private Vector2 lastPos;
    private Vector2 curPos;
    private Vector2 playerPos;

    void Start()
    {
        animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
        if (!isEnemyBullet)
        { 
            transform.localScale = new Vector2(GameManager.BulletSize, GameManager.BulletSize);
        }
        StartCoroutine(DeathDelay()); 
        transform.localScale = new Vector3(GameManager.BulletSize, GameManager.BulletSize, 1);
    }

    void Update()
    {
        if (isEnemyBullet)
        {
            curPos = transform.position;
            transform.position = Vector2.MoveTowards(transform.position, playerPos, 5f * Time.deltaTime);
            if (curPos == lastPos)
            {
                Destroy(gameObject);
            }
            lastPos = curPos;
            
        }
    }

    public void GetPlayer(Transform player)
    {
        playerPos = player.position;
    }

    IEnumerator DeathDelay()
    {
        yield return new WaitForSeconds(lifeTime);
        Destroy(gameObject);
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Zombie") && !isEnemyBullet)
        {
            animator.SetBool("isCrash", true);
            rb.linearVelocity = Vector2.zero;
            rb.bodyType = RigidbodyType2D.Static;
        }
        if (collision.CompareTag("Player") && isEnemyBullet)
        {
            // animator.SetBool("isCrash", true);
            Destroy(gameObject);
            rb.linearVelocity = Vector2.zero;
            rb.bodyType = RigidbodyType2D.Static;
            Vector2 rawDir = (collision.transform.position - transform.position);
            Vector2 direction = Mathf.Abs(rawDir.x) > Mathf.Abs(rawDir.y)
                ? new Vector2(Mathf.Sign(rawDir.x), 0)
                : new Vector2(0, Mathf.Sign(rawDir.y));

            GameManager.instance.DecreaseLife(direction);
        }
    }

    void OnDestroy()
    {
        Destroy(gameObject);
        
    }
}

