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

    private int[,] map;


    // Start is called before the first frame update
    void Start()
    {
        if(Algorithms.current != null)
        {
            map = Algorithms.current.generateMap();
        } else {
            Algorithms.current = new RandomAlgorithm();
            map = Algorithms.current.generateMap();
        }
        int x = map.GetLength(0);
        int y = map.GetLength(1);


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
