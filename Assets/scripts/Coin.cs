using UnityEngine;

public class Coin : MonoBehaviour
{
    public int value = 1;
    void Start()
    {
    }
    void Update()
    {
    }
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            GameManager.instance.AddPoint(value);
            Destroy(gameObject);
        }
    }
}
