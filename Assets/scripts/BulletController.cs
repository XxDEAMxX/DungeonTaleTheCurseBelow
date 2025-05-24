
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
    private bool isFalling = false;
    private float fallTimer = 0f;
    private float fallDuration = 0.4f; // caída breve
    private Vector2 fallStartPos;
    private Vector2 fallEndPos;
    private float fallHeight = 0.7f;   // profundidad de la parábola
    private bool isDying = false;



    void Start()
    {
        animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();

        if (!isEnemyBullet)
        {
            lifeTime = 2.5f;
            transform.localScale = new Vector2(GameManager.BulletSize, GameManager.BulletSize);

        }
        else
        {
            transform.localScale = new Vector2(2f, 2f);
        }
        StartCoroutine(DeathDelay());
    }

    void Update()
    {
        if (isDying) return;
        if (isFalling)
        {
            fallTimer += Time.deltaTime;
            float t = fallTimer / fallDuration;

            if (t >= 1f)
            {
                isFalling = false;
                isDying = true;
                rb.velocity = Vector2.zero;
                rb.bodyType = RigidbodyType2D.Kinematic;
                GetComponent<Collider2D>().enabled = false;
                animator.SetBool("isDeath", true);
                return;
            }

            // Interpolación lineal hacia el punto final
            Vector2 pos = Vector2.Lerp(fallStartPos, fallEndPos, t);
            transform.position = pos;

            // Girar hacia la dirección del movimiento
            Vector2 dir = fallEndPos - fallStartPos;
            if (dir != Vector2.zero)
            {
                float targetAngle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
                Quaternion targetRotation = Quaternion.Euler(0, 0, targetAngle);

                // Factor de suavizado más bajo (más lento, más suave)
                float smoothFactor = 2f; // Prueba incluso con 1f si quieres ultra suave
                transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, Time.deltaTime * smoothFactor);
            }

            if (isEnemyBullet)
        {
            curPos = transform.position;
            transform.position = Vector2.MoveTowards(transform.position, playerPos, 5f * Time.deltaTime);
            // if (curPos == lastPos)
            // {
            //     Destroy(gameObject);
            // }
            lastPos = curPos;
        }

            return;
        }

        // Movimiento normal
        
    }



    public void GetPlayer(Transform player)
    {
        playerPos = player.position;
    }

    IEnumerator DeathDelay()
    {
        yield return new WaitForSeconds(lifeTime);

        isFalling = true;
        fallTimer = 0f;
        fallStartPos = transform.position;

        // Dirección horizontal actual de la lágrima
        Vector2 direction = transform.right.normalized;

        // Cae solo un poco hacia abajo (sin arco)
        fallEndPos = fallStartPos + direction * 0.4f + Vector2.down * 1.0f;

        // Desactiva físicas
        rb.velocity = Vector2.zero;
        rb.bodyType = RigidbodyType2D.Kinematic;
        GetComponent<Collider2D>().enabled = false;
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
    
    public void OnDeathAnimationEnd()
    {
        Destroy(gameObject);
    }
}

