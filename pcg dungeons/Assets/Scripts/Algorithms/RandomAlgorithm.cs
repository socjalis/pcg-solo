using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RandomAlgorithm : Algorithm
{
    int mapWidth = 10;
    int mapHeight = 10;
    [Range(0, 100)]
    int roomChance = 50;

    public override void setParameters(SortedDictionary<string, string> parameters)
    {
        mapWidth = int.Parse(parameters["Map width"]);
        mapHeight = int.Parse(parameters["Map height"]);
        roomChance = int.Parse(parameters["Room chance"]);
    }

    public override SortedDictionary<string, string> getParameters()
    {
        return new SortedDictionary<string, string>(){
            { "Map width", "string" },
            { "Map height", "string" },
            { "Room chance", "string" }
        };
    }

    public override int[,] generateMap()
    {
        int [,] map = new int[mapWidth, mapHeight];
        System.Random random = new System.Random();
        for (int i = 0; i < mapWidth; i++)
        {
            for (int j = 0; j < mapHeight; j++)
            {
                int number = random.Next(0, 101);
                if(number <= roomChance)
                {
                    map[i, j] = 1;
                }
                else
                {
                    map[i, j] = 0;
                }
            }
        }
        return map;
    }
}
