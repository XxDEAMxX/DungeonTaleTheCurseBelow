
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Collections;

public class BulletController : MonoBehaviour
{
    public float lifeTime = 2f; // Tiempo de vida de la bala
    private Rigidbody2D rb;

    private Animator animator;
    void Start()
    {
        animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
        StartCoroutine(DeathDelay()); 
    }

    void Update()
    {
    }

    IEnumerator DeathDelay()
    {
        yield return new WaitForSeconds(lifeTime);
        Destroy(gameObject);
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Zombie"))
        {
            animator.SetBool("isCrash", true);
            rb.linearVelocity = Vector2.zero;
            rb.bodyType = RigidbodyType2D.Static;
        }
    }

    void OnDestroy()
    {
        Destroy(gameObject);
        
    }
}

