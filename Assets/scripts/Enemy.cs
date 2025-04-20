using UnityEngine;

public class Enemy : MonoBehaviour
{
    private int life ;
    private Animator animator;
    void Start()
    {   
        life = 5;
        animator = GetComponent<Animator>();
    }
    void Update()
    {
        
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
