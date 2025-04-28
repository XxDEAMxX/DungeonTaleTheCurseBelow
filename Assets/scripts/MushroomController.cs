using UnityEngine;

public class MushroomController : MonoBehaviour
{
    private Animator animator;
    void Start()
    {
        animator = GetComponent<Animator>();
    }

    void Update()
    {
        
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Sword") || collision.CompareTag("Bullet"))
        {
            // life--;
            // if (life <= 0)
            // {
            // curreState = EnemyState.Dead;   
            animator.SetBool("isDamage", true);
            Destroy(gameObject, 0.5f);
            }

    }

    void isDamgeFalse()
    {
        animator.SetBool("isDamage", false);
    }   
            
     
}
