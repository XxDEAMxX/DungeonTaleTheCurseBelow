using System.Collections.Generic;
using UnityEngine;

public class DungeonGenerarator: MonoBehaviour
{
    public DungeonGeneratorData dungeonGeneratorData;
    private List<Vector2Int> dungeonRooms;
    private void Start()
    {
        dungeonRooms = DungeonCrawlerController.GenerateDungeon(dungeonGeneratorData);
        SpawnRooms(dungeonRooms);
    }

    private void SpawnRooms(List<Vector2Int> rooms)
    {
        RoomController.instance.LoadRoom("Start", 0, 0);
        foreach (Vector2Int roomLocation in rooms)
        {
                RoomController.instance.LoadRoom(RoomController.instance.GetRandomRoomName(), roomLocation.x, roomLocation.y);
        }
    }

}
