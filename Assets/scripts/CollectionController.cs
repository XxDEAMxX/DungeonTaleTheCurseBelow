using UnityEngine;

[System.Serializable]
public class Item
{
    public string name;
    public string description;
    public Sprite itemImage;
}

public class CollectionController : MonoBehaviour
{

    public Item item;
    public int healthChange;
    public float moveSpeedChange;
    public float attackSpeedChange;
    public float bulletSizeChange;
    public AudioClip pickupSound; // Campo para el sonido de recolección
    private AudioSource audioSource; // Componente AudioSource

    // Start is called before the first frame update
    void Start()
    {
        // GetComponent<SpriteRenderer>().sprite = item.itemImage;
        // Destroy(GetComponent<PolygonCollider2D>());
        gameObject.AddComponent<CircleCollider2D>().isTrigger = true;
        // Intentar obtener un AudioSource existente o añadir uno nuevo si no existe
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null) 
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.tag == "Player")
        {
            if (pickupSound != null && audioSource != null)
            {
                // Reproducir el sonido en la posición del item antes de destruirlo
                // Usamos PlayClipAtPoint para que el sonido no se corte si el objeto se destruye inmediatamente
                AudioSource.PlayClipAtPoint(pickupSound, transform.position);
            }
            IsaacController.collectedAmount++;
            GameManager.HealPlayer(healthChange);
            GameManager.MoveSpeedChange(moveSpeedChange);
            GameManager.FireRateChange(attackSpeedChange);
            GameManager.BulletSizeChange(bulletSizeChange);
            GameManager.instance.UpdateCollectedItems(this);
            Destroy(gameObject);
        }
    }
}