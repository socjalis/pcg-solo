using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;

public class GeneticAlgorithm2 : Algorithm
{
    Vector2Int mapSize = new Vector2Int(40, 40);
    [Range(0, 100)]
    int roomDensity = 20;
    int populationSize = 100;
    int generations = 5000;
    System.Random random;

    class Room
    {
        public int[,] map;

        Vector2Int mapSize;
        System.Random rnd;
        int roomDensity;
        int rooms;
        int mapLength;

        public Room(Vector2Int size, int density)
        {
            mapSize = size;
            roomDensity = density;
            map = new int[mapSize.x, mapSize.y];
            rnd = new System.Random();

            rooms = 0;
            generateRandomMap();
            mapLength = mapSize.x * mapSize.y;
        }

        public Room(Vector2Int size, int density, int[,] _map, int _rooms)
        {
            mapSize = size;
            map = new int[mapSize.x, mapSize.y];
            roomDensity = density;
            Array.Copy(_map, map, mapSize.x * mapSize.y);
            rooms = _rooms;
            mapLength = mapSize.x * mapSize.y;
            rnd = new System.Random();
        }


        static int corridorMidPoints = 50;
        static int corridor1Points = 20;
        static int cornerPoints = 15;
        static int edgePoints = 15;
        static int corridorPoints = 200;

        void generateRandomMap()
        {
            for (int i = 1; i < mapSize.x - 1; i++)
            {
                for (int j = 1; j < mapSize.y - 1; j++)
                {
                    int number = rnd.Next(1, 101);
                    if (number <= 50)
                    {
                        map[i, j] = 1;
                    }
                    else
                    {
                        map[i, j] = 0;
                        rooms++;
                    }
                }
            }

            for (int x = 0; x < mapSize.x; x++)
            {
                map[x, 0] = 1;
                map[x, mapSize.y - 1] = 1;
            }
            for (int y = 0; y < mapSize.y - 1; y++)
            {
                map[0, y] = 1;
                map[mapSize.x - 1, y] = 1;
            }
        }

        public float Score()
        {
            float densityMultiplier = 1f;
            float separationMultiplier = 3f;

            //density 0-arraySize
            float density = fastDensityScore();
            float densityScore = DensityScoreFunction(density);
            if (densityScore < 0) Debug.LogWarning("densityScore: " + densityScore);

            // separation of rooms
            float seperationScore = SeperateRoomsScore() * separationMultiplier;

            //float score = density * densityMultiplier + tilesType * tilesTypeMultiplier;
            float score = seperationScore * densityScore;
            return score;
        }

        // 1-100
        public float DensityScoreFunction(float density)
        {
            float left = Mathf.Max(0f, roomDensity - 10f);
            float right = Mathf.Min(roomDensity + 10f, 100f);
            float pointsLeft = 20f;
            float pointsRight = 20f;

            if (density <= left)
            {
                return 0f + density * (pointsLeft / left);
            }
            else if (density < right)
            {
                float res = -1f * Mathf.Pow(0.9f * (density - roomDensity), 2f) + 80f + pointsLeft;
                if (res < 0) Debug.LogWarning("res: " + res + " density: " + density + " expected: " + left + "-" + right);
                return res;
            }
            else
            {
                float res = 20f - density * (pointsRight / 100f);
                if (res < 0) Debug.LogWarning("res: " + res + " density on else: " + density);
                return res;
            }
        }

        public float fastDensityScore()
        {
            if (mapLength < 0) Debug.LogWarning("ARRAYSIZE: " + mapLength);
            if (rooms < 0) Debug.LogWarning("rooms: " + rooms);

            float res = ((float)rooms / (float)mapLength) * 100f;
            if (res < 0) Debug.LogWarning("RES: " + res);
            return res;
        }

        public int DensityScore()
        {
            int z = 0;
            for (int x = 1; x < mapSize.x - 1; x++)
            {
                for (int y = 1; y < mapSize.y - 1; y++)
                {
                    if (map[x, y] == 0)
                    {
                        z++;
                    }
                }
            }

            return z;
        }

        public float SeperateRoomsScore()
        {
            bool[,] visited = new bool[mapSize.x, mapSize.y];
            int r = 0;
            for (int x = 1; x < mapSize.y - 1; x++)
            {
                for (int y = 1; y < mapSize.y - 1; y++)
                {
                    Vector2Int v = new Vector2Int(x, y);
                    if (map[x, y] == 0 && !visited[x, y])
                    {
                        r++;
                        VisitNeighbours(v, visited);
                    }
                }
            }

            return Mathf.Max(0f, 100-r);
        }

        void VisitNeighbours(Vector2Int tile, bool[,] visited)
        {
            visited[tile.x, tile.y] = true;
            Vector2Int down = new Vector2Int(tile.x, tile.y - 1);
            Vector2Int up = new Vector2Int(tile.x, tile.y + 1);
            Vector2Int left = new Vector2Int(tile.x - 1, tile.y);
            Vector2Int right = new Vector2Int(tile.x + 1, tile.y);


            if (down.y > 0 && map[down.x, down.y] == 0 && !visited[down.x, down.y])
            {
                VisitNeighbours(down, visited);
            }
            if (up.y < mapSize.y && map[up.x, up.y] == 0 && !visited[up.x, up.y])
            {
                VisitNeighbours(up, visited);
            }
            if (left.x > 0 && map[left.x, left.y] == 0 && !visited[left.x, left.y])
            {
                VisitNeighbours(left, visited);
            }
            if (right.x < mapSize.x && map[right.x, right.y] == 0 && !visited[right.x, right.y])
            {
                VisitNeighbours(right, visited);
            }
        }

        public void Mutate()
        {
            int x = rnd.Next(1, mapSize.x - 1);
            int y = rnd.Next(1, mapSize.y - 1);
            if (map[x, y] == 0)
            {
                map[x, y] = 1;
                rooms -= 1;
                if (rooms < 0) Debug.LogWarning("rooms;;;" + rooms);
            }
            else
            {
                map[x, y] = 0;
                rooms += 1;
            }

            //RecalculateNeighbours(new Vector2Int(x, y));
        }

        public void Mutate(int x, int y)
        {
            if (map[x, y] == 0)
            {
                map[x, y] = 1;
                rooms -= 1;
            }
            else
            {
                map[x, y] = 0;
                rooms += 1;
            }
        }

        public void MultipleMutate(int n)
        {
            int prex = rnd.Next(2, mapSize.x - 2);
            int prey = rnd.Next(2, mapSize.y - 2);

            for (int i = 0; i < n; i++)
            {
                int x = rnd.Next(prex - 1, prex + 2);
                int y = rnd.Next(prey - 1, prey + 2);
                Mutate(x, y);
            }


        }
        public void MultipleMutate(int n, int times)
        {
            for (int i = 0; i < times; i++)
            {
                MultipleMutate(n);
            }
        }

        public Room CreateCopy()
        {
            return new Room(mapSize, roomDensity, map, rooms);
        }
    }

    public override void setParameters(SortedDictionary<string, string> parameters)
    {
        mapSize.x = int.Parse(parameters["Map width"]);
        mapSize.y = int.Parse(parameters["Map height"]);
        roomDensity = int.Parse(parameters["Room density"]);
    }

    public override SortedDictionary<string, string> getParameters()
    {
        return new SortedDictionary<string, string>(){
            { "Map width", "string" },
            { "Map height", "string" },
            { "Room density", "string" }
        };
    }

    public override int[,] generateMap()
    {
        List<Room> rooms = new List<Room>();
        int[,] bestMap = new int[mapSize.x, mapSize.y];
        float bestScore = -100000;
        random = new System.Random();

        int half = Mathf.FloorToInt(populationSize / 2);
        int arraySize = mapSize.x * mapSize.y;

        //initialize
        for (int i = 0; i < populationSize; i++)
        {
            rooms.Add(new Room(mapSize, roomDensity));
        }

        rooms = rooms.OrderByDescending(r => r.Score()).ToList();
        for (int i = 0; i < generations; i++)
        {
            // nautral selection and mutations
            for (int j = half; j < populationSize; j++)
            {
                rooms[j] = rooms[populationSize - j].CreateCopy();
                int x =  random.Next(1, 8);
                int y =  random.Next(1, 4);
                rooms[j].MultipleMutate(x, y);
            }

            // mutate the best ones
            for (int j = (int)half / 2; j < half; j++)
            {

                rooms[j].MultipleMutate(2);
            }

            // sort
            rooms = rooms.OrderByDescending(r => r.Score()).ToList();

            //check best
            if (rooms[0].Score() > bestScore)
            {
                Array.Copy(rooms[0].map, bestMap, arraySize);
                bestScore = rooms[0].Score();
            }
            Debug.Log("Generation: " + i + " Score: " + rooms[0].Score() + " Density: " + rooms[0].DensityScoreFunction(rooms[0].fastDensityScore()) + " Separation: " + rooms[0].SeperateRoomsScore());
        }

        return rooms[0].map;
    }
}
