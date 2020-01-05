using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using Unity;

public class TileDrawer : MonoBehaviour
{
    public Tilemap tilemap;
    public Tile wall;
    public Tile room;
    private int x;
    private int y;

    private int[,] map;


    // Start is called before the first frame update
    void Start()
    {
        x = 20;
        y = 20;
        map = new int[x, y];
        System.Random random = new System.Random();
        for (int i = 0; i < x; i++)
        {
            for (int j = 0; j < y; j++)
            {
                map[i, j] = random.Next(0, 2);
            }
        }

        for (int i = 0; i < x; i++)
        {
            for (int j = 0; j < y; j++)
            {
                Tile chosenTile = map[i, j] == 1 ? wall : room;
                tilemap.SetTile(new Vector3Int(-i + x / 2,-j + y/2, 1), chosenTile);
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
