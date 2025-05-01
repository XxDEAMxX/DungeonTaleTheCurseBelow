using UnityEngine;

public class CameraController : MonoBehaviour
{
    public static CameraController instance;
    public Room currRoom;
    public float moveSpeedWhenRoomChange;

    void Update()
    {
        UpdatePosition();
    }

    void UpdatePosition()
    {
        if (currRoom != null)
        {
            Vector3 targetPosition = currRoom.GetRoomCenter();
            transform.position = Vector3.MoveTowards(transform.position, targetPosition, moveSpeedWhenRoomChange * Time.deltaTime);
        }
    }

    Vector3 GetCameraTargetPosition()
    {
        if (currRoom != null)
        {
            Vector3 targetPos = currRoom.GetRoomCenter();
            targetPos.z = transform.position.z;
            return targetPos;
        }
        
        return Vector3.zero;
    }

    void Awake()
    {
        instance = this;
    }

    public bool IsSwitchingScene()
    {
        return transform.position.Equals(GetCameraTargetPosition()) == false;
    }
}
