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
            // if (roomLocation == dungeonRooms[dungeonRooms.Count - 1] && !(roomLocation == Vector2Int.zero))
            // {
            //     RoomController.instance.LoadRoom("End", roomLocation.x, roomLocation.y);
            // } else{
                RoomController.instance.LoadRoom("Empty", roomLocation.x, roomLocation.y);
            // }
        }
    }

}
