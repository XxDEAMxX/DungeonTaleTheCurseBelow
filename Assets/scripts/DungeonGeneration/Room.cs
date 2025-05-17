using System.Collections.Generic;
using UnityEngine;

public class Room : MonoBehaviour
{
    public int width;
    public int height;
    public int x;
    public int y;
    public Door leftDoor;
    public Door rightDoor;
    public Door topDoor;
    public Door bottomDoor;
    public List<Door> doors = new List<Door>();
    private bool updatedDoors = false;

    public Room(int x, int y)
    {
        this.x = x;
        this.y = y;
    }
    
    void Start()
    {
        if (RoomController.instance == null)
        {
            Debug.LogError("RoomController instance is null. Make sure it is initialized before using Room.");
            return;
        }

        Door[] ds = GetComponentsInChildren<Door>();
        foreach (Door d in ds)
        {
            doors.Add(d);
            switch (d.doorType)
            {
                case Door.DoorType.left:
                    leftDoor = d;
                    break;
                case Door.DoorType.right:
                    rightDoor = d;
                    break;
                case Door.DoorType.top:
                    topDoor = d;
                    break;
                case Door.DoorType.bottom:
                    bottomDoor = d;
                    break;
            }
        }

        RoomController.instance.RegisterRoom(this);
    }

    void Update()
    {
        if (name.Contains("End") && !updatedDoors)
        {
            RemoveUnconnectedDoors();
            updatedDoors = true;
        }
    }

    public void RemoveUnconnectedDoors()
    {
        foreach (Door door in doors)
        {
            switch (door.doorType)
            {
                case Door.DoorType.left:
                    if (GetLeftRoom() == null)
                    {
                        door.gameObject.SetActive(false);

                        if (door.InvisibleWall != null)
                        {
                            door.InvisibleWall.SetActive(true);

                            Collider2D wallCollider = door.InvisibleWall.GetComponent<Collider2D>();
                            if (wallCollider != null)
                            {
                                wallCollider.isTrigger = false;
                            }
                            else
                            {
                                Debug.LogWarning($"La InvisibleWall de la puerta '{door.doorType}' no tiene un Collider2D.");
                            }
                        }
                        else
                        {
                            Debug.LogWarning($"InvisibleWall no está asignado en la puerta '{door.doorType}'.");
                        }
                    }
                    break;
                case Door.DoorType.right:
                    if (GetRightRoom() == null)
                    {
                        door.gameObject.SetActive(false);

                        if (door.InvisibleWall != null)
                        {
                            door.InvisibleWall.SetActive(true);

                            Collider2D wallCollider = door.InvisibleWall.GetComponent<Collider2D>();
                            if (wallCollider != null)
                            {
                                wallCollider.isTrigger = false;
                            }
                            else
                            {
                                Debug.LogWarning($"La InvisibleWall de la puerta '{door.doorType}' no tiene un Collider2D.");
                            }
                        }
                        else
                        {
                            Debug.LogWarning($"InvisibleWall no está asignado en la puerta '{door.doorType}'.");
                        }
                    }
                    break;
                case Door.DoorType.top:
                    if (GetTopRoom() == null)
                    {
                        door.gameObject.SetActive(false);

                        if (door.InvisibleWall != null)
                        {
                            door.InvisibleWall.SetActive(true);

                            Collider2D wallCollider = door.InvisibleWall.GetComponent<Collider2D>();
                            if (wallCollider != null)
                            {
                                wallCollider.isTrigger = false;
                            }
                            else
                            {
                                Debug.LogWarning($"La InvisibleWall de la puerta '{door.doorType}' no tiene un Collider2D.");
                            }
                        }
                        else
                        {
                            Debug.LogWarning($"InvisibleWall no está asignado en la puerta '{door.doorType}'.");
                        }
                    }
                    break;
                case Door.DoorType.bottom:
                    if (GetBottomRoom() == null)
                    {
                        door.gameObject.SetActive(false);

                        if (door.InvisibleWall != null)
                        {
                            door.InvisibleWall.SetActive(true);

                            Collider2D wallCollider = door.InvisibleWall.GetComponent<Collider2D>();
                            if (wallCollider != null)
                            {
                                wallCollider.isTrigger = false;
                            }
                            else
                            {
                                Debug.LogWarning($"La InvisibleWall de la puerta '{door.doorType}' no tiene un Collider2D.");
                            }
                        }
                        else
                        {
                            Debug.LogWarning($"InvisibleWall no está asignado en la puerta '{door.doorType}'.");
                        }
                    }
                    break;
            }
        }
    }

    public Room GetRightRoom()
    {
        if (RoomController.instance.DoesRoomExist(x + 1, y))
        {
            return RoomController.instance.FindRoom(x + 1, y);
        }
        return null;
    }
    public Room GetLeftRoom()
    {
        if (RoomController.instance.DoesRoomExist(x - 1, y))
        {
            return RoomController.instance.FindRoom(x - 1, y);
        }
        return null;
    }
    public Room GetTopRoom()
    {
        if (RoomController.instance.DoesRoomExist(x, y + 1))
        {
            return RoomController.instance.FindRoom(x, y + 1);
        }
        return null;
    }
    public Room GetBottomRoom()
    {
        if (RoomController.instance.DoesRoomExist(x, y - 1))
        {
            return RoomController.instance.FindRoom(x, y - 1);
        }
        return null;
    }

    void OnDrawGizmos() {
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(transform.position, new Vector3(width, height, 0));    
    }

    public Vector3 GetRoomCenter()
{
    return new Vector3(x * width, y * height, -10f); // Z fijo para cámara
}

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            Debug.Log("Player entered room: " + gameObject.name);
            RoomController.instance.OnPlayerEnterRoom(this);
            // CameraController.instance.currRoom = this;
            // CameraController.instance.UpdatePosition();
        }
    }
    
}
