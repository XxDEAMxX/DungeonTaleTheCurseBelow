
using System.Collections.Generic;
using System.Collections;
using UnityEngine;

public class GridController : MonoBehaviour
{
    public Room room;

    [System.Serializable]
    public struct Grid{
        public int columns, rows;
        public float vericalOffset, horizontalOffset;
    }

    public Grid grid;
    public GameObject gridTile;
    public List<Vector2> availablePositions = new List<Vector2>();

    void Awake()
    {
        room = GetComponentInParent<Room>();
        grid.columns = room.width - 3;
        grid.rows = room.height - 3;
        GenerateGrid();
    }

    public void GenerateGrid()
    {
        grid.vericalOffset += room.transform.localPosition.y;
        grid.horizontalOffset += room.transform.localPosition.x;

        for (int y = 0; y < grid.rows; y++)
        {
            for (int x = 0; x < grid.columns; x++)
            {
                GameObject go = Instantiate(gridTile, transform);
                go.GetComponent<Transform>().position 
                    = new Vector2(x - (grid.columns - grid.horizontalOffset), y - (grid.rows - grid.vericalOffset));
                go.name = "X: " + x + ", Y: " + y;
                availablePositions.Add(go.transform.position);
                go.SetActive(false);
            }
        }
        GetComponentInParent<ObjectRoomSpawner>().InitialiseObjectSpawning();
    }

}