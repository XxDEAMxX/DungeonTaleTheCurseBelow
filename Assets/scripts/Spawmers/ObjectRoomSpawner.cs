using UnityEngine;

public class ObjectRoomSpawner : MonoBehaviour
{
    [System.Serializable]
    public struct RandomSpawner{
        public string name;
        public SpawnerData spawnerData;
    }
    public GridController grid;
    public RandomSpawner[] spawnerData;
    void Start()
    {
    grid = GetComponentInChildren<GridController>();
    }

    public void InitialiseObjectSpawning()
    {
        foreach (RandomSpawner rs in spawnerData)
        {
            SpawnObjects(rs);
        }
    }

    void SpawnObjects(RandomSpawner data)
    {
        int randomIteration = Random.Range(data.spawnerData.minSpawn, data.spawnerData.maxSpawn + 1);
        Debug.Log(data.name + " " + randomIteration);
        for (int i = 0; i < randomIteration; i++)
        {
            int randomPos = Random.Range(0, grid.availablePositions.Count - 1);
            GameObject go = Instantiate(data.spawnerData.itemToSpawn, grid.availablePositions[randomPos], Quaternion.identity, transform) as GameObject;
            grid.availablePositions.RemoveAt(randomPos);
            Debug.Log("Spawned Object");
        }  
    }
}
