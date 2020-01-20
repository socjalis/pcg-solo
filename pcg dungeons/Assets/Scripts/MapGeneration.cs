using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapGeneration : MonoBehaviour
{
    public Transform tilePrefab;
    public Transform wallPrefab;
    public Transform playerPrefab;
    public Transform enemyPrefab;
    public Transform grassPrefab;
    private Vector2 mapSize;
    private int[,] map;
    System.Random rnd;

    private void Start()
    {
        if (Algorithms.current != null)
        {
            map = Algorithms.current.generateMap();
        }
        else
        {
            Algorithms.current = new BSPAlgorithm();
            map = Algorithms.current.generateMap();
        }
        mapSize = new Vector2(map.GetLength(0), map.GetLength(1));
        rnd = new System.Random();
        GenerateMap();
        GenerateWalls();
        List<Vector2Int> roomTiles = GetRoomTiles();
        GeneratePlayer(roomTiles);
    }

    void GenerateMap()
    {
        string holderName = "Map";
        if (transform.Find(holderName)) {
            DestroyImmediate(transform.Find(holderName).gameObject);
        };

        Transform mapHolder = new GameObject(holderName).transform;
        mapHolder.parent = transform;

        for (int x = 0; x < mapSize.x; x++)
        {
            for (int y = 0; y < mapSize.y; y++)
            {
                Vector3 tilePosition = new Vector3(-mapSize.x / 2 + 0.5f + x, 0, -mapSize.y/2 + 0.5f + y);
                Transform newTile = Instantiate(tilePrefab, tilePosition, Quaternion.Euler(Vector3.right * 90)) as Transform;
                newTile.parent = mapHolder;
            }
        }
    }
    void GenerateWalls()
    {
        for (int x = 0; x < mapSize.x; x++)
        {
            for (int y = 0; y < mapSize.y; y++)
            {
                if (map[x, y] == 1)
                {
                    Vector3 wallPosition = new Vector3(-mapSize.x / 2 + 0.5f + x, 0.5f, -mapSize.y / 2 + 0.5f + y);
                    Transform newWall = Instantiate(wallPrefab, wallPosition, Quaternion.identity) as Transform;
                }
            }
        }
    }

    List<Vector2Int> GetRoomTiles()
    {
        List<Vector2Int> roomTiles = new List<Vector2Int>();
        for (int x = 0; x < mapSize.x; x++)
        {
            for (int y = 0; y < mapSize.y; y++)
            {
                if(map[x, y] == 0)
                {
                    roomTiles.Add(new Vector2Int(x, y));
                }
            }
        }
        return roomTiles;
    }

    void GeneratePlayer(List<Vector2Int> roomTiles)
    {
        int index = rnd.Next(0, roomTiles.Count);
        Vector2Int tile = roomTiles[index];
        roomTiles.RemoveAt(index);
        Vector3 playerPosition = new Vector3(-mapSize.x / 2 + 0.5f + tile.x, 0.5f, -mapSize.y / 2 + 0.5f + tile.y);
        
        Zoom.player = Instantiate(playerPrefab, playerPosition, Quaternion.identity).gameObject;

        GenerateEnemies(roomTiles, 10);
    }
     
    void GenerateEnemies(List<Vector2Int> roomTiles, int nEnemies)
    {
        int numberOfEnemies = Mathf.Min(nEnemies, roomTiles.Count);
        for(int i = 0; i<numberOfEnemies; i++)
        {
            int index = rnd.Next(0, roomTiles.Count);
            Vector2Int tile = roomTiles[index];
            roomTiles.RemoveAt(index);
            Vector3 enemyPosition = new Vector3(-mapSize.x / 2 + 0.5f + tile.x, 0.5f, -mapSize.y / 2 + 0.5f + tile.y);
            Instantiate(enemyPrefab, enemyPosition, Quaternion.identity);
        }
    }
}
