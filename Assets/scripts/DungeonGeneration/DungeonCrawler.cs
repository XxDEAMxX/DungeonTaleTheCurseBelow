using UnityEngine;
using System.Collections.Generic;

public class DangeonCrawler: MonoBehaviour
{
    public Vector2Int Position { get; set; }
    public DangeonCrawler(Vector2Int startPosition)
    {
        Position = startPosition;
    }

    public Vector2Int Move(Dictionary<Direction, Vector2Int> directionMovementMap)
    {
        Direction toMove = (Direction)CustomRandom.Range(0, directionMovementMap.Count);
        Position += directionMovementMap[toMove];
        return Position;
    }

}
