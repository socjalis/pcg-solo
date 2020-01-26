using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OverlapingAlgorithm : Algorithm
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

    private class Room
    {
        public int x;
        public int y;
        public int x2;
        public int y2;
        public int overlaps;

        public Room(int x, int y, int x2, int y2)
        {
            this.x = x;
            this.y = y;
            this.x2 = x2;
            this.y2 = y2;
            this.overlaps = 0;
        }
    }

    private void countOverlaps(List<Room> rooms)
    {
        foreach (Room room in rooms) {
            room.overlaps = 0;
            foreach (Room room2 in rooms)
            {
                if (room == room2) continue;
                if ((room.x-1 > room2.x2+1) || (room2.x-1 > room.x2+1)) continue;
                if ((room.y2+1 < room2.y-1) || (room2.y2+1 < room.y-1)) continue;
                room.overlaps++;
            }
        }
    }

    private int maxOverlaps(List<Room> rooms)
    {
        int max = 0;
        foreach (Room room in rooms)
        {
            if (room.overlaps > max) max = room.overlaps;
        }
        return max;
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

        //Room[] rooms = new Room[roomChance];

        List <Room> rooms = new List<Room>();

        System.Random random = new System.Random();
        for (int k = 0; k < roomChance; k++)
        {
            int w = random.Next((int)System.Math.Floor(0.1 * mapWidth), (int)System.Math.Floor(mapWidth * 0.2));
            int h = random.Next((int)System.Math.Floor(mapHeight * 0.1), (int)System.Math.Floor(mapHeight * 0.2));

            int x = random.Next(1, mapWidth - w - 1);
            int y = random.Next(1, mapHeight - h - 1);

            rooms.Add(new Room(x, y, x + w, y + h));
        }

        countOverlaps(rooms);

        int max = maxOverlaps(rooms);
        while (max > 0)
        {
            foreach (Room room in rooms)
            {
                if (room.overlaps == max)
                {
                    rooms.Remove(room);
                    break;
                }
            }
            countOverlaps(rooms);
            max = maxOverlaps(rooms);
        }


        foreach (Room room in rooms)
        {
            for (int x = room.x; x <= room.x2; x++)
            {
                for (int y = room.y; y <= room.y2; y++)
                {
                    map[x, y] = 0;
                }
            }
        }

        return map;
    }
}
