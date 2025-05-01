using System.Collections.Generic;
using UnityEngine;

public enum Direction
{
    top = 0,
    left = 1,
    down = 2,
    Right = 3
}

public class DungeonCrawlerController: MonoBehaviour
{
    public static List<Vector2Int> positionsVisited = new List<Vector2Int>();
    private static readonly Dictionary<Direction, Vector2Int> directionOffsets = new Dictionary<Direction, Vector2Int>
    {
        { Direction.top, Vector2Int.up },
        { Direction.left, Vector2Int.left },
        { Direction.down, Vector2Int.down },
        { Direction.Right, Vector2Int.right }
    };
    public static List<Vector2Int> GenerateDungeon(DungeonGeneratorData dungeonGeneratorData)
    {
        List<DangeonCrawler> dungeonCrawlers = new List<DangeonCrawler>();

        for (int i = 0; i < dungeonGeneratorData.numberOfCrawlers; i++)
        {
            dungeonCrawlers.Add(new DangeonCrawler(Vector2Int.zero));
        }

        int iterations = Random.Range(dungeonGeneratorData.iterationMin, dungeonGeneratorData.iterationMax);

        for (int i = 0; i < iterations; i++)
        {
            foreach (DangeonCrawler dangeonCrawler in dungeonCrawlers)
            {
                Vector2Int newPosition = dangeonCrawler.Move(directionOffsets);
                positionsVisited.Add(newPosition);
            }
        }
        return positionsVisited;
    }
}
