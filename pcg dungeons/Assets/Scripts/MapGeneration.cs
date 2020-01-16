using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapGeneration : MonoBehaviour
{
    public Transform tilePrefab;
    public Transform wallPrefab;
    private Vector2 mapSize;

    [Range(0,1)]
    public float outlinePercent;

    public int[,] map;

    private void Start()
    {
        if (Algorithms.current != null)
        {
            map = Algorithms.current.generateMap();
        }
        else
        {
            Algorithms.current = new RandomAlgorithm();
            map = Algorithms.current.generateMap();
        }
        mapSize = new Vector2(map.GetLength(0), map.GetLength(1));
        GenerateMap();
        GenerateWalls();
    }

    public void GenerateMap()
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
                newTile.localScale = Vector3.one * (1 - outlinePercent);
                newTile.parent = mapHolder;
            }
        }
    }
    public void GenerateWalls()
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
                //tilemap.SetTile(new Vector3Int(-i + x / 2, -j + y / 2, 1), chosenTile);
            }
        }
    }
}
