using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AgentAlgorithm : Algorithm
{
    // parameters
    int mapWidth = 40;
    int mapHeight = 40;

    int minRoomSize = 2;
    int maxRoomSize = 8;

    int roomChanceInc = 2;
    int directionChanceInc = 5;
    int tolerance = 50;

    List<Vector2Int> directions = new List<Vector2Int> {
        new Vector2Int(0, -1),
        new Vector2Int(-1, 0),
        new Vector2Int(1, 0),
        new Vector2Int(0, 1)
    };


    public override void setParameters(SortedDictionary<string, string> parameters)
    {
        mapWidth = int.Parse(parameters["Map width"]);
        mapHeight = int.Parse(parameters["Map height"]);
        tolerance = tolerance * mapWidth;
    }

    public override SortedDictionary<string, string> getParameters()
    {
        return new SortedDictionary<string, string>(){
            { "Map width", "string" },
            { "Map height", "string" },
        };
    }

    bool IsInMapRange(Vector2Int pos)
    {
        if (pos.x > mapWidth - 2 || pos.x < 1 || pos.y > mapHeight - 2 || pos.y < 1)
        {
            return false;
        }
        else return true;
    }

    bool IsEmptyEnough(Vector2Int pos, int[,] map, int limit)
    {
        int counter = 0;
        for(int i = pos.x - 1; i <= pos.x + 1; i++)
        {
            for(int j = pos.y - 1; j <= pos.y + 1; j++)
            {
                if(IsInMapRange(new Vector2Int(i, j)))
                {
                    if(map[i, j] == 0)
                    {
                        counter++;
                    }
                }
            }
        }
        return counter <= limit;
    }

    bool IsInMapRangeAndEmpty(Vector2Int pos, int[,] map)
    {
        if (pos.x > mapWidth - 2 || pos.x < 1 || pos.y > mapHeight - 2 || pos.y < 1 || map[pos.x, pos.y] == 0)
        {
            return false;
        }
        else return true;
    }


    bool CreateRoom(Vector2Int pos, int[,] map, int[,] rooms)
    {
        if(pos.x - 1 < 1 || (mapWidth - 1) - pos.x < 1 || pos.y - 1 < 1 || (mapHeight - 1) - pos.y < 1)
        {
            return false;
        }
        int max = (int)(maxRoomSize / 2);
        int roomLeft = Random.Range(1, Mathf.Min(max, pos.x - 1));
        int roomRight = Random.Range(1, Mathf.Min(max, (mapWidth - 1) - pos.x));
        int roomBottom = Random.Range(1, Mathf.Min(max, pos.y - 1));
        int roomTop = Random.Range(1, Mathf.Min(max, (mapHeight - 1) - pos.y));
        //Debug.Log("left: " + roomLeft + ", right: " + roomRight + ", bottom: " + roomBottom + ", top" + roomTop + ", pos: " + pos);

        for (int i = pos.x - roomLeft - 1; i <= pos.x + roomRight; i++)
        {
            for (int j = pos.y - roomBottom - 1; j <= pos.y + roomTop; j++)
            {
                if ( !(i > 0 && j > 0 && i < mapWidth - 1 && j < mapHeight - 1 && rooms[i, j] != 0) )
                {
                    //Debug.Log("blad");
                    return false;
                }
            }
        }

        for (int i = pos.x - roomLeft; i < pos.x + roomRight; i++)
        {
            for (int j = pos.y - roomBottom; j < pos.y + roomTop; j++)
            {
                map[i, j] = 0;
                rooms[i, j] = 0;
            }
        }
        //Debug.Log("created room");
        return true;
    }

    public override int[,] generateMap()
    {
        int[,] map = new int[mapWidth, mapHeight];
        int[,] actualRooms = new int[mapWidth, mapHeight];
        int[,] corridors = new int[mapWidth, mapHeight];
        for (int i=0; i<mapWidth; i++)
        {
            for(int j=0; j<mapHeight; j++)
            {
                map[i, j] = 1;
                actualRooms[i, j] = 1;
                corridors[i, j] = 1;
            }
        }
        List<Vector2Int> rooms = new List<Vector2Int>();
        List<Vector2Int> corrs = new List<Vector2Int>();


        Vector2Int position = new Vector2Int(Mathf.FloorToInt(mapHeight / 2), Mathf.FloorToInt(mapWidth / 2));
        int direction = Random.Range(0, 4);
        int directionChance = directionChanceInc;
        int roomChance = roomChanceInc;

        Vector2Int prevPosition = position;
        int reviveFailed = 0;
        int t = 0;

        void Revive()
        {
            int dir = Random.Range(0, 4);
            if ((dir + 2) % 4 == direction || dir == direction) dir++;
            direction = dir % 4;
            t++;
            if(reviveFailed >= 3)
            {
                //Debug.Log("revive failed 4x: ");

                position = corrs[Random.Range(0, corrs.Count)];
                reviveFailed = 0;
            }
        }

        // algorithm starts
        while(t < tolerance)
        {
            int dirRand = Random.Range(0, 100);
            int roomRand = Random.Range(0, 100);

            // change direction
            if (dirRand < directionChance)
            {
                direction = Random.Range(0, 4);
                directionChance = 0;
            }
            else directionChance += directionChanceInc;

            // create room
            if (roomRand < roomChance)
            {
                if(!CreateRoom(position, map, actualRooms))
                {
                    t++;
                }
                roomChance = 0;
            }
            else roomChance += roomChanceInc;

            Vector2Int newPosition = position + directions[direction];

            // dig
            if (IsInMapRange(newPosition) && IsEmptyEnough(newPosition, corridors, 2))
            {
                position = newPosition;
                map[position.x, position.y] = 0;
                corridors[position.x, position.y] = 0;
                corrs.Add(new Vector2Int(position.x, position.y));
                rooms.Add(new Vector2Int(position.x, position.y));
            }
            else
            {
                if (position == prevPosition) reviveFailed++;
                prevPosition = position;
                Revive();
            };

            // stop
            if(t > tolerance)
            {
                break;
            }
        }
        
        return map;
    }
}