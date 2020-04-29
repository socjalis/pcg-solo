using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;

public class GeneticAlgorithm : Algorithm
{
    Vector2Int mapSize = new Vector2Int(40,40);
    [Range(0, 100)]
    int roomDensity = 20;
    int populationSize = 100;
    int generations = 1000;
    UnityEngine.Random random;
    enum Tile
    {
        LBCORNER,
        LUCORNER,
        RBCORNER,
        RUCORNER,
        BOTTOMEDGE,
        TOPEDGE,
        LEFTEDGE,
        RIGHTEDGE,
        VERTICALCORRIDOR,
        HORIZONTALCORRIDOR,
        ROOM,
        WALL,
        CORRIDOR1UPLEFT,
        CORRIDOR1UPRIGHT,
        CORRIDOR1DOWNLEFT,
        CORRIDOR1DOWNRIGHT,
        CORRIDOR1RIGHTUP,
        CORRIDOR1RIGHTDOWN,
        CORRIDOR1LEFTUP,
        CORRIDOR1LEFTDOWN,
        CORRIDORMIDLEFTUP,
        CORRIDORMIDUPRIGHT,
        CORRIDORMIDRIGHTDOWN,
        CORRIDORMIDDOWNLEFT,
        CORNERS,
        CROSS,
        FAILURE,
        OTHER,
        BORDER
    }

    class Room
    {
        public int[,] map;


        Vector2Int mapSize;
        Tile[,] tileMap;
        System.Random rnd;
        int roomDensity;
        int rooms;
        public int tileScore;


        public Room(Vector2Int size, int density)
        {
            mapSize = size;
            roomDensity = density;
            map = new int[mapSize.x, mapSize.y];
            tileMap = new Tile[mapSize.x, mapSize.y];
            rnd = new System.Random();
            tileScore = 0;

            generateRandomMap();
            classifyMap();
            tileScore = tilesTypeScore();
            rooms = 0;
        }

        public Room(Vector2Int size, int density, int[,] _map, int _rooms, Tile[,] _tileMap, int _tileScore)
        { 
            mapSize = size;
            map = new int[mapSize.x, mapSize.y];
            tileMap = new Tile[mapSize.x, mapSize.y];
            roomDensity = density;
            Array.Copy(_map, map, mapSize.x*mapSize.y);
            Array.Copy(_tileMap, tileMap, mapSize.x * mapSize.y);
            tileScore = _tileScore;
            rooms = _rooms;

            rnd = new System.Random();
        }


        static int corridorMidPoints = 50;
        static int corridor1Points = 20;
        static int cornerPoints = 15;
        static int edgePoints = 15;
        static int corridorPoints = 200;

        Dictionary<Tile, int> tilePoints = new Dictionary<Tile, int>(){
            {Tile.LBCORNER, cornerPoints},
            {Tile.LUCORNER, cornerPoints},
            {Tile.RBCORNER, cornerPoints},
            {Tile.RUCORNER, cornerPoints},
            {Tile.BOTTOMEDGE, cornerPoints},
            {Tile.TOPEDGE, edgePoints},
            {Tile.LEFTEDGE, edgePoints},
            {Tile.RIGHTEDGE, edgePoints},
            {Tile.VERTICALCORRIDOR, corridorPoints},
            {Tile.HORIZONTALCORRIDOR, corridorPoints},
            {Tile.ROOM, 60},
            {Tile.WALL, 4},
            {Tile.CORRIDOR1UPLEFT, corridor1Points},
            {Tile.CORRIDOR1UPRIGHT, corridor1Points},
            {Tile.CORRIDOR1DOWNLEFT, corridor1Points},
            {Tile.CORRIDOR1DOWNRIGHT, corridor1Points},
            {Tile.CORRIDOR1RIGHTUP, corridor1Points},
            {Tile.CORRIDOR1RIGHTDOWN, corridor1Points},
            {Tile.CORRIDOR1LEFTUP, corridor1Points},
            {Tile.CORRIDOR1LEFTDOWN, corridor1Points},
            {Tile.CORRIDORMIDLEFTUP, corridorMidPoints},
            {Tile.CORRIDORMIDUPRIGHT, corridorMidPoints},
            {Tile.CORRIDORMIDRIGHTDOWN, corridorMidPoints},
            {Tile.CORRIDORMIDDOWNLEFT, corridorMidPoints},
            {Tile.CROSS, 30 },
            {Tile.CORNERS, -80},
            {Tile.FAILURE, -500 },
            {Tile.OTHER, -3 },
            {Tile.BORDER, 0 }
        };

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

        void classifyMap()
        {
            for (int i = 0; i < mapSize.x; i++)
            {
                for (int j = 0; j < mapSize.y; j++)
                {
                    tileMap[i, j] = ClassifyTile(new Vector2Int(i, j));
                }
            }
        }

        public float Score()
        {
            float densityMultiplier = 1f;
            float tilesTypeMultiplier = 1f;
            float separationMultiplier = 5f;

            //density 0-arraySize
            float density = fastDensityScore();
            float densityScore = DensityScoreFunction(density);
            if (densityScore < 0) Debug.LogWarning("densityScore: " + densityScore);
            //types of rooms
            int tilesType = tileScore;
            // separation of rooms
            float seperationScore = SeperateRoomsScore() * separationMultiplier;
            if (seperationScore > 0) Debug.LogWarning("seperationScore: " + seperationScore);

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
            float pointsRight = 80f;

            if(density <= left)
            {
                return 0f + density * (pointsLeft / left);
            }
            else if(density < right)
            {
                float res = -1f * Mathf.Pow(0.9f * (density - roomDensity), 2f) + 80f + pointsLeft;
                if (res < 0) Debug.LogWarning("res: " + res + " density: " + density + " expected: " + left + "-" + right);
                return res;
            }
            else
            {
                return 20f - density * (pointsRight / right);
            }
        }

        public float fastDensityScore()
        {
            int arraySize = mapSize.x * mapSize.y;
            if (arraySize < 0) Debug.LogWarning("ARRAYSIZE: " + arraySize);
            if (rooms < 0) Debug.LogWarning("rooms: " + rooms);

            float res = ((float)rooms / (float)arraySize) * 100f;
            if (res < 0) Debug.LogWarning("RES: " + res);
            return res;
        }

        public int DensityScore()
        {
            int z = 0;
            for (int x = 1; x < mapSize.x-1; x++)
            {
                for (int y = 1; y < mapSize.y-1; y++)
                {
                    if(map[x,y] == 0)
                    {
                        z++;
                    }
                }
            }
            
            return z;
        }

        int tilesTypeScore()
        {
            int score = 0;
            for (int x = 1; x < mapSize.x - 1; x++)
            {
                for (int y = 1; x < mapSize.y - 1; x++)
                {
                    score += tilePoints[ClassifyTile(new Vector2Int(x, y))];
                }
            }
            return score;
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
                    if (map[x, y] == 0 && !visited[x,y])
                    {
                        r++;
                        VisitNeighbours(v, visited);
                    }
                }
            }

            return -r;
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

       
        Tile ClassifyTile(Vector2Int tile)
        {
            if(tile.x == 0 || tile.y == 0 || tile.x == mapSize.x - 1 || tile.y == mapSize.y - 1)
            {
                return Tile.BORDER;
            }

            int t11 = map[tile.x - 1, tile.y + 1], t12 = map[tile.x, tile.y + 1], t13 = map[tile.x + 1, tile.y + 1];
            int t21 = map[tile.x - 1, tile.y], t22 = map[tile.x, tile.y], t23 = map[tile.x + 1, tile.y];
            int t31 = map[tile.x - 1, tile.y - 1], t32 = map[tile.x, tile.y - 1], t33 = map[tile.x + 1, tile.y - 1];

            int bin = int.Parse(t11.ToString() + t12 + t13 + t21 + t22 + t23 + t31 + t32 + t33);
            //lb corner = 5 
            if (bin == 100100111)
            {
                return Tile.LBCORNER;
            }
            // lu corner = 5
            else if (bin == 111100100)
            {
                return Tile.LUCORNER;
            }
            // rb corner = 5
            else if (bin == 001001111)
            {
                return Tile.RBCORNER;
            }
            // ru corner = 5
            else if (bin == 111001001)
            {
                return Tile.RUCORNER;
            }
            // bottom edge = 3
            else if (bin == 000000111)
            {
                return Tile.BOTTOMEDGE;
            }
            // top edge = 3
            else if (bin == 111000000)
            {
                return Tile.TOPEDGE;
            }
            // left edge = 3
            else if (bin == 100100100)
            {
                return Tile.LEFTEDGE;
            }
            // right edge = 3
            else if (bin == 001001001)
            {
                return Tile.RIGHTEDGE;
            }
            // vertical corridor = 3
            else if (bin == 010010010)
            {
                return Tile.VERTICALCORRIDOR;
            }
            // horizontal corridor = 3
            else if (bin == 000111000)
            {
                return Tile.HORIZONTALCORRIDOR;
            }
            // room = 7
            else if (bin == 0)
            {
                return Tile.ROOM;
            }
            // wall = 7
            else if (bin == 111111111)
            {
                return Tile.WALL;
            }
            // corridor1upleft1 = 2
            else if (bin == 001101101)
            {
                return Tile.CORRIDOR1UPLEFT;
            }
            // corridor1upright2 = 2
            else if (bin == 100101101)
            {
                return Tile.CORRIDOR1UPRIGHT;
            }
            // corridor1downleft3 = 2
            else if (bin == 101101001)
            {
                return Tile.CORRIDOR1DOWNLEFT;
            }
            // corridor1downright4 = 2
            else if (bin == 101101100)
            {
                return Tile.CORRIDOR1DOWNRIGHT;
            }
            // corridor1right up5 = 2
            else if (bin == 110000111)
            {
                return Tile.CORRIDOR1RIGHTUP;
            }
            // corridor1right down6 = 2
            else if (bin == 111000110)
            {
                return Tile.CORRIDOR1RIGHTDOWN;
            }
            // corridor1left up7 = 2
            else if (bin == 011000111)
            {
                return Tile.CORRIDOR1LEFTUP;
            }
            // corridor1left down8 = 2
            else if (bin == 111000011)
            {
                return Tile.CORRIDOR1LEFTDOWN;
            }
            // corridor2 left up = 5
            else if (bin == 101001111)
            {
                return Tile.CORRIDORMIDLEFTUP;
            }
            // corridor2 up right = 5
            else if (bin == 101100111)
            {
                return Tile.CORRIDORMIDUPRIGHT;
            }
            // corridor2 right down = 5
            else if (bin == 111100101)
            {
                return Tile.CORRIDORMIDRIGHTDOWN;
            }
            // corridor2 down left = 5
            else if (bin == 111001101)
            {
                return Tile.CORRIDORMIDDOWNLEFT;
            }
            // hole
            else if (t12 == 1 && t21 == 1 && t22 == 0 && t23 == 1 && t32 == 1)
            {
                //map[tile.x, tile.y] = 1;
                //RecalculateNeighbours(new Vector2Int(tile.x, tile.y));
                return Tile.FAILURE;
            }
            // point
            else if (t12 == 0 && t21 == 0 && t22 == 1 && t23 == 0 && t32 == 0)
            {
                //    map[tile.x, tile.y] = 0;
                //    RecalculateNeighbours(new Vector2Int(tile.x, tile.y));
                return Tile.FAILURE;
            }
            // corners sticking
            else if ((
                t11 == 0 && t12 == 1 && t21 == 1 ||
                t13 == 0 && t23 == 1 && t21 == 1 ||
                t31 == 0 && t32 == 1 && t23 == 1 ||
                t33 == 0 && t21 == 1 && t32 == 1 ) && t22 == 0)
            {
                return Tile.CORNERS;
            }
            else
            {
                return Tile.OTHER;
            }
        }

        void RecalculateNeighbours(Vector2Int tile)
        {
            int preScore = 0;
            for (int i = -1; i < 2; i++)
            {
                for (int j = -1; j < 2; j++)
                {
                    preScore += tilePoints[tileMap[tile.x + i, tile.y + j]];
                }
            }

            int postScore = 0;
            for (int i = -1; i < 2; i++)
            {
                for (int j = -1; j < 2; j++)
                {
                    tileMap[tile.x + i, tile.y + j] = ClassifyTile(new Vector2Int(tile.x + i, tile.y + j));
                    postScore += tilePoints[tileMap[tile.x + i, tile.y + j]];
                }
            }
            tileScore += postScore - preScore;
        }

        public void Mutate()
        {
            int x = rnd.Next(1, mapSize.x - 1);
            int y = rnd.Next(1, mapSize.y - 1);
            if (map[x, y] == 0)
            {
                map[x, y] = 1;
                rooms--;
                if (rooms < 0) Debug.LogWarning("rooms;;;" + rooms);
            }
            else
            {
                map[x, y] = 0;
                rooms++;
            }

            //RecalculateNeighbours(new Vector2Int(x, y));
        }

        public void Mutate(int x, int y)
        {
            if (map[x, y] == 0)
            {
                map[x, y] = 1;
                rooms--;
            }
            else
            {
                map[x, y] = 0;
                rooms++;
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
                RecalculateNeighbours(new Vector2Int(x, y));
            }


        }
        public void MultipleMutate(int n, int times)
        {
            for(int i = 0; i<times; i++)
            {
                MultipleMutate(n);
            }
        }

        public Room CreateCopy()
        {
            return new Room(mapSize, roomDensity, map, rooms, tileMap, tileScore);
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
        int[,] map = new int[0, 0];
        List<Room> rooms = new List<Room>();
        int half = Mathf.FloorToInt(populationSize / 2);
        int arraySize = mapSize.x * mapSize.y;
        int[,] bestMap = new int[mapSize.x, mapSize.y];
        float bestScore = -100000;
        random = new UnityEngine.Random();
        for (int i = 0; i < populationSize; i++)
        {
            rooms.Add(new Room(mapSize, roomDensity));
        }

        rooms = rooms.OrderByDescending(r => r.Score()).ToList();
        for (int i = 0; i < generations; i++)
        {
            for (int j = half; j < populationSize; j++)
            {
                rooms[j] = rooms[populationSize - j].CreateCopy();
                int x = (int)UnityEngine.Random.value * 8;
                int y = (int)UnityEngine.Random.value * 4;
                rooms[j].MultipleMutate(x, y);
            }

            for (int j = (int)half/2; j < half; j++)
            {
                
                rooms[j].MultipleMutate(2);
            }
            rooms = rooms.OrderByDescending(r => r.Score()).ToList();
            if(rooms[0].Score() > bestScore)
            {
                Array.Copy(rooms[0].map, bestMap, arraySize);
                bestScore = rooms[0].Score();
            }
            Debug.Log("Generation: " + i + " Score: " + rooms[0].Score() + " Density: " + rooms[0].DensityScoreFunction(rooms[0].fastDensityScore()) + " Separation: " + rooms[0].SeperateRoomsScore());
        }

        return rooms[0].map;
    }
}
