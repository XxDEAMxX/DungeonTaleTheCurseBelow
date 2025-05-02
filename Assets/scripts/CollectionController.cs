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
    // Start is called before the first frame update
    void Start()
    {
        // GetComponent<SpriteRenderer>().sprite = item.itemImage;
        // Destroy(GetComponent<PolygonCollider2D>());
        gameObject.AddComponent<CircleCollider2D>().isTrigger = true;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.tag == "Player")
        {
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