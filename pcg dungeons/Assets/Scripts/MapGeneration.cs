using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapGeneration : MonoBehaviour
{
    public Transform tilePrefab;
    public Transform wallPrefab;
    public Transform playerPrefab;
    public Transform enemyPrefab;
    public Transform plantPrefab;
    public Transform rockPrefab;
    public Transform resurectionPrefab;
    public int enemies;
    public int plants;
    public int rocks;

    static public List<Vector2Int> rooms;

    private Vector2 mapSize;
    private int[,] map;
 
    System.Random rnd;

    private void Start()
    {
        Time.timeScale = 1f;
        if (Algorithms.current != null)
        {
            map = Algorithms.current.generateMap();
            if (Algorithms.corridorMaker != null)
                map = Algorithms.corridorMaker.makeCorridors(map);
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
        GenerateGrass(roomTiles, plants);
        AI.CreateTree(map);
        rooms = GetRoomTiles();
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
        Vector3 resPosition = new Vector3(playerPosition.x, 0f, playerPosition.z);

        Zoom.player = Instantiate(playerPrefab, playerPosition, Quaternion.identity).gameObject;
        Instantiate(resurectionPrefab, resPosition, Quaternion.LookRotation(Vector3.up));

        GenerateEnemies(roomTiles, enemies);
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

    void GenerateGrass(List<Vector2Int> roomTiles, int plants)
    {
        int numberOfPlants= Mathf.Min(plants, roomTiles.Count);
        for (int i = 0; i < numberOfPlants; i++)
        {
            int index = rnd.Next(0, roomTiles.Count);
            Vector2Int tile = roomTiles[index];
            roomTiles.RemoveAt(index);
            float offsetX = (rnd.Next(0, 100) - 50f) / 100;
            float offsetY = (rnd.Next(0, 100) - 50f) / 100;
            Vector3 plantPosition = new Vector3(-mapSize.x / 2 + 0.5f + tile.x + offsetX, 0f, -mapSize.y / 2 + 0.5f + tile.y + offsetY);
            Instantiate(plantPrefab, plantPosition, Quaternion.identity * Quaternion.Euler(0f, rnd.Next(0, 180), 0f));
        }
        GenerateRocks(roomTiles, rocks);
    }
    
    void GenerateRocks(List<Vector2Int> roomTiles, int rocks)
    {
        int numberOfRocks = Mathf.Min(plants, roomTiles.Count);
        for (int i = 0; i < numberOfRocks; i++)
        {
            int index = rnd.Next(0, roomTiles.Count);
            Vector2Int tile = roomTiles[index];
            roomTiles.RemoveAt(index);
            float offsetX = (rnd.Next(0, 100) - 50f) / 100;
            float offsetY = (rnd.Next(0, 100) - 50f) / 100;
            Vector3 rockPosition = new Vector3(-mapSize.x / 2 + 0.5f + tile.x + offsetX, 0f, -mapSize.y / 2 + 0.5f + tile.y + offsetY);
            Instantiate(rockPrefab, rockPosition, Quaternion.identity * Quaternion.Euler(0f, rnd.Next(0, 180), 0f));
        }
    }
}
