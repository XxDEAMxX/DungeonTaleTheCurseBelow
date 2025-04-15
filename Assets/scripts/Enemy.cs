using UnityEngine;

public class Enemy : MonoBehaviour
{
    private Animator animator;
    void Start()
    {
        animator = GetComponent<Animator>();
    }
    void Update()
    {
        
    }

    private void OnCollisionEnter2D(Collision2D other) {
        if (other.gameObject.CompareTag("Player"))
        {
            Vector2 direction =new Vector2(transform.position.x, 0);
            GameManager.instance.DecreaseLife(direction);
        }
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Sword"))
        {
           animator.SetBool("BlDeath", true);
        }  
    }

    void Death()
    {
        Destroy(gameObject);
        GameManager.instance.AddPoint(1);
    }
}
