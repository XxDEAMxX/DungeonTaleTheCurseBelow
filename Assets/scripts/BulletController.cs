
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Collections;

public class BulletController : MonoBehaviour
{
    public float lifeTime = 2f; // Tiempo de vida de la bala
    void Start()
    {
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
            Destroy(gameObject);
        }
    }
}

