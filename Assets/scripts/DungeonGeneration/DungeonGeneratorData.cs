using UnityEngine;

[CreateAssetMenu(fileName = "DungeonGeneratorData.asset", menuName = "DungeonGeneratorData/Dungeon Data")]
public class DungeonGeneratorData: ScriptableObject
{
    public int numberOfCrawlers;
    public int iterationMin;
    public int iterationMax;
}
