using System.Collections;
using UnityEngine;

public class PooterController : MonoBehaviour
{
    private int life;
    private Animator animator;
    private float speed = 2f;
    private Vector2 moveDirection;
    private bool isDead = false;
    void Start()
    {
        life = 1;
        animator = GetComponent<Animator>();
        StartCoroutine(RandomWalk());
    }

    void Update()
    {
        if (!isDead)
        {
            transform.Translate(moveDirection * speed * Time.deltaTime);
        }
    }

    IEnumerator RandomWalk()
    {
        while (!isDead)
        {
            //Todo: ramdon
            // Elegir una dirección aleatoria: vertical u horizontal
            int direction = Random.Range(0, 4);
            switch (direction)
            {
                case 0: moveDirection = Vector2.up; break;
                case 1: moveDirection = Vector2.down; break;
                case 2: moveDirection = Vector2.left; break;
                case 3: moveDirection = Vector2.right; break;
            }

            // Camina por 1 segundo
            yield return new WaitForSeconds(1f);

            // Detener movimiento
            moveDirection = Vector2.zero;

            // Esperar 1 segundo antes del siguiente movimiento
            yield return new WaitForSeconds(2f);
        }
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Bullet"))
        {
            life--;
            if (life <= 0)
            {
                animator.SetBool("isDeth", true);
            }


        }  
        
         if (collision.CompareTag("BombRange"))
        {
            life = 0; // Matar al Pooter
            animator.SetBool("isDeth", true);
        }
    }

    void OnDestroy()
    {
        Destroy(gameObject);
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
}
