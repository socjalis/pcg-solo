using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AgentAlgorithm : Algorithm
{
    // parameters
    int mapWidth = 8;
    int mapHeight = 8;
    public int minRoomSize = 2;
    public int maxRoomSize;
    [Range(0, 100)]
    public int changingDirectionStep = 10;
    [Range(0, 100)]
    public int creatingRoomStep = 10;
    public int maxNumberOfSteps; //= 10000;

    // current data
    int currentDirectionChance = 50;
    int currentRoomChance = 50;
    string currentDirection;
    bool isEnoughSpace = true;
    Vector2Int digger = new Vector2Int();
    Dictionary<string, Vector2Int> directions = new Dictionary<string, Vector2Int>()
    {
        { "up", new Vector2Int(0, -1) },
        { "down", new Vector2Int(0, 1) },
        { "left", new Vector2Int(-1, 0) },
        { "right", new Vector2Int(1, 0) }
    };
    string[] directionIndexes = { "up", "down", "left", "right" };


    public override void setParameters(SortedDictionary<string, string> parameters)
    {
        mapWidth = int.Parse(parameters["Map width"]);
        mapHeight = int.Parse(parameters["Map height"]);
        changingDirectionStep = int.Parse(parameters["Changing direction step"]);
        creatingRoomStep = int.Parse(parameters["Creating room step"]);
        //maxNumberOfSteps = int.Parse(parameters["Max number of steps"]);
    }

    public override SortedDictionary<string, string> getParameters()
    {
        return new SortedDictionary<string, string>(){
            { "Map width", "string" },
            { "Map height", "string" },
            { "Changing direction step", "string" },
            { "Creating room step", "string" }//,
            //{ "Max number of steps", "string"}
        };
    }

    public void digTunnel(int[,] map)
    {
        digger += directions[currentDirection];
        if (digger.x >= mapWidth || digger.x < 0
            || digger.y >= mapHeight || digger.y < 0) // if digger reaches border
        {
            isEnoughSpace = false; // end of algorithm
            Debug.Log("Digger reached a border." + digger.x + digger.y);
        }

        else if (map[digger.x, digger.y] != 0)
        {
            map[digger.x, digger.y] = 0;
            Debug.Log("Digger digs.");
        }
        else // digger reaches a tunnel or a room
        {
            Debug.Log("Digger reached a tunnel.");
        }
    }

    public void tryToChangeDirection(System.Random random)
    {
        int number = random.Next(0, 101);
        Debug.Log("Change direction number: " + number);
        if (number <= currentDirectionChance)
        {
            int randomDir;
            do
            {
                randomDir = random.Next(0, 4);
            }
            while (directionIndexes[randomDir] == currentDirection);
            currentDirection = directionIndexes[randomDir];
            Debug.Log("Current direction: " + currentDirection);

            currentDirectionChance = 0;
        }
        else
        {
            currentDirectionChance += changingDirectionStep;
        }
    }

    bool digRoom(int[,] map, System.Random random)
    {
        int x = random.Next(minRoomSize, maxRoomSize + 1);
        int y = random.Next(minRoomSize, maxRoomSize + 1);
        int startX = digger.x - (int)Mathf.Ceil(x / 2);
        int startY = digger.y - (int)Mathf.Ceil(y / 2);
        int endX = digger.x + (int)Mathf.Floor(x / 2);
        int endY = digger.y + (int)Mathf.Floor(y / 2);
        if (startX < 0 || startY < 0 || endX >= mapWidth || endY >= mapHeight)
        {
            return false;
        }
        else
        {
            for (int i = startX; i <= endX; i++)
            {
                for (int j = startY; j < endY; j++)
                {
                    map[i, j] = 0;
                }
            }
            return true;
        }
    }

    bool checkIfRoomPossible(System.Random random)
    {
        int number = random.Next(0, 101);
        if (number <= currentRoomChance)
        {
            currentRoomChance = 0;
            return true;
        }
        else
        {
            currentRoomChance += creatingRoomStep;
            return false;
        }
    }


    public override int[,] generateMap()
    {
        //maxRoomSize = (int)Mathf.Floor((Mathf.Min(mapHeight, mapWidth)) / 4);
        maxRoomSize = 6;
        maxNumberOfSteps = mapWidth * mapHeight;
        int[,] map = new int[mapWidth, mapHeight];

        for (int i = 0; i < mapWidth; i++)
        {
            for (int j = 0; j < mapHeight; j++)
            {
                map[i, j] = 1;
            }
        }

        System.Random random = new System.Random();
        digger.x = random.Next(mapWidth / 4, 3 * mapWidth / 4);
        digger.y = random.Next(mapHeight / 4, 3 * mapHeight / 4);
        map[digger.x, digger.y] = 0;
        currentDirection = directionIndexes[random.Next(0, 4)];
        Debug.Log(currentDirection);

        while (isEnoughSpace && maxNumberOfSteps > 0)
        {
            digTunnel(map);
            tryToChangeDirection(random);
            if (checkIfRoomPossible(random))
            {
                if (!digRoom(map, random))
                {
                    Debug.Log("Cannot fit another room.");
                    isEnoughSpace = false;
                }
                else
                {
                    Debug.Log("Digging a room.");
                }
            }
            maxNumberOfSteps -= 1;
        }

        return map;
    }


}