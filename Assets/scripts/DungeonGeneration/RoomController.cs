using System.Collections;
using UnityEngine;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using System.Linq;

public class RoomInfo{

    public string name;
    public int x;
    public int y;
}

public class RoomController : MonoBehaviour{
    public static RoomController instance;
    string currentWorldName = "Basement";
    RoomInfo currentLoadRoomData;
    Room currRoom;
    Queue<RoomInfo> loadRoomQueue = new Queue<RoomInfo>();
    public List<Room> loadedRooms = new List<Room>();
    bool isLoadingRoom = false;
    bool updatedRooms = false;
    bool spawnedBossRoom = false;
    bool bossRoomLoaded = false;
    void Start() {
        // LoadRoom("Start", 0, 0);
        // LoadRoom("Empty", 1, 0);
        // LoadRoom("Empty", -1, 0);
        // LoadRoom("Empty", 0, 1);
        // LoadRoom("Empty", 0, -1);}
    }

    void Update() {
        UpdateRoomQueue();
    }

    void UpdateRoomQueue() {
        if (isLoadingRoom) {
            return;
        }
        if (loadRoomQueue.Count == 0) {
    if(!spawnedBossRoom){
        StartCoroutine(SpawnBossRoom());
    }
    else if (spawnedBossRoom && bossRoomLoaded && !updatedRooms && !isLoadingRoom) {
        foreach (Room room in loadedRooms) {
            room.RemoveUnconnectedDoors();
        }
        updatedRooms = true;
        Debug.Log("RemoveUnconnectedDoors ejecutado en todas las habitaciones, incluyendo la del jefe.");
    }
    return;
}
        currentLoadRoomData = loadRoomQueue.Dequeue();
        isLoadingRoom = true;
        StartCoroutine(LoadRoomCoroutine(currentLoadRoomData));
    }

    IEnumerator SpawnBossRoom()
    {
        spawnedBossRoom = true;
        yield return new WaitForSeconds(0.5f);
        if(loadRoomQueue.Count == 0)
        {
            Room bossRoom = loadedRooms[loadedRooms.Count - 1];
            Room tempRoom = new Room(bossRoom.x, bossRoom.y);
            Destroy(bossRoom.gameObject);
            var roomToRemove = loadedRooms.Single(r => r.x == tempRoom.x && r.y == tempRoom.y);
            loadedRooms.Remove(roomToRemove);
            LoadRoom("End", tempRoom.x, tempRoom.y);
        }
    }

    public void LoadRoom(string name, int x, int y) {
        if (DoesRoomExist(x, y)) {
            Debug.Log("Room already loaded: " + name + " " + x + ", " + y);
            return;
        }

        RoomInfo newRoomData = new RoomInfo();
        newRoomData.name = name;
        newRoomData.x = x;
        newRoomData.y = y;
        loadRoomQueue.Enqueue(newRoomData);
    }
    
    IEnumerator LoadRoomCoroutine(RoomInfo info) {
        string roomName = currentWorldName + info.name;

        AsyncOperation loadRoom = SceneManager.LoadSceneAsync(roomName, LoadSceneMode.Additive);

        while (!loadRoom.isDone) {
            yield return null;
        }

        float timeout = 1f;
        yield return new WaitForSeconds(timeout);
        if (isLoadingRoom) {
            Debug.LogWarning("Timeout: room no registrado correctamente, liberando carga.");
            isLoadingRoom = false;
        }
    }

    public void RegisterRoom(Room room) {
    if (!DoesRoomExist(currentLoadRoomData.x, currentLoadRoomData.y)) {
        room.transform.position = new Vector3(
            currentLoadRoomData.x * room.width,
            currentLoadRoomData.y * room.height,
            0
        );

        room.x = currentLoadRoomData.x;
        room.y = currentLoadRoomData.y;
        room.name = currentWorldName + "-" + currentLoadRoomData.name + " " + room.x + ", " + room.y;
        room.transform.parent = transform;
        
        isLoadingRoom = false;

        if(loadedRooms.Count == 0) {
            CameraController.instance.currRoom = room;
        }

        loadedRooms.Add(room);

        // Marcar si es la boss room
        if (currentLoadRoomData.name == "End") {
            bossRoomLoaded = true;
        }

    } else {
        Destroy(room.gameObject);
        isLoadingRoom = false;
    }
}

    public bool DoesRoomExist(int x, int y) {
        return loadedRooms.Find(item => item.x == x && item.y == y) != null;
    }


    public void OnPlayerEnterRoom(Room room) {
        CameraController.instance.currRoom = room;
        currRoom = room;
    }
    public Room FindRoom(int x, int y) {
        return loadedRooms.Find(item => item.x == x && item.y == y);
    }
    
    void Awake() {
            instance = this;
    } 
}
