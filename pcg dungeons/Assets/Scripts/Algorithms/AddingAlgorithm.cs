using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AddingAlgorithm : Algorithm
{
    int mapWidth = 50;
    int mapHeight = 50;
    int roomChance = 50;


    public override void setParameters(SortedDictionary<string, string> parameters)
    {
        mapWidth = int.Parse(parameters["Map width"]);
        mapHeight = int.Parse(parameters["Map height"]);
        roomChance = int.Parse(parameters["Number of rooms"]);
    }

    public override SortedDictionary<string, string> getParameters()
    {
        return new SortedDictionary<string, string>(){
            { "Map width", "string" },
            { "Map height", "string" },
            { "Number of rooms", "string" }
        };
    }

    private bool checkRoom(int[,] map, int x, int y, int w, int h)
    {
        for (int i = x - 1; i < x + w + 1; i++)
        {
            if (i >= mapWidth || i < 0) return false;
            for (int j = y - 1; j < y + h + 1; j++)
            {
                if (j >= mapHeight || j < 0) return false;
                if (map[i, j] == 0) return false;
            }
        }
        return true;
    }

    public override int[,] generateMap()
    {
        int[,] map = new int[mapWidth, mapHeight];

        for (int i = 0; i < mapWidth; i++)
        {
            for (int j = 0; j < mapHeight; j++)
            {
                map[i, j] = 1;
            }
        }

        System.Random random = new System.Random();
        for (int k = 0; k < roomChance; k++)
        {
            int x = random.Next(0, mapWidth);
            int y = random.Next(0, mapHeight);

            int w = random.Next((int)System.Math.Floor(0.1 * mapWidth), (int)System.Math.Floor(mapWidth * 0.2));
            int h = random.Next((int)System.Math.Floor(mapHeight * 0.1), (int)System.Math.Floor(mapHeight * 0.2));

            if (checkRoom(map, x, y, w, h))
            {
                for (int i = x; i < x + w; i++)
                {
                    if (i >= mapWidth) continue;
                    for (int j = y; j < y + h; j++)
                    {
                        if (j >= mapHeight) continue;
                        map[i, j] = 0;
                    }
                }
            }
        }
        return map;
    }
}
