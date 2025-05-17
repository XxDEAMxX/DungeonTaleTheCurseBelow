using UnityEngine;

public class Door : MonoBehaviour
{

    public enum DoorType
    {
        left,
        right,
        top,
        bottom
    }

    public DoorType doorType;
    public GameObject InvisibleWall;
    public GameObject doorCollider;
    private GameObject player;
    private float widthOffset = 4f;
    private void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");  
    }


    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            switch (doorType)
            {
                case DoorType.left:
                    player.transform.position = new Vector2(transform.position.x - widthOffset, transform.position.y);
                    break;
                case DoorType.right:
                    player.transform.position = new Vector2(transform.position.x + widthOffset, transform.position.y);
                    break;
                case DoorType.top:
                    player.transform.position = new Vector2(transform.position.x, transform.position.y + widthOffset);
                    break;
                case DoorType.bottom:
                    player.transform.position = new Vector2(transform.position.x, transform.position.y - widthOffset);
                    break;
            }
        }    
    }

}
