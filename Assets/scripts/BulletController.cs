
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Collections;

public class BulletController : MonoBehaviour
{
    public float lifeTime;
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
}

