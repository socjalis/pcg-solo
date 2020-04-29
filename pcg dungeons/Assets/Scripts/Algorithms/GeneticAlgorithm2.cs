using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;
using Random = UnityEngine.Random;

public class GeneticAlgorithm2 : Algorithm
{
    Vector2Int mapSize = new Vector2Int(40, 40);
    [Range(0, 100)]
    int roomDensity = 20;
    int populationSize = 6;
    int generations = 200;
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
        BORDER,
        HOLE,
        POINT
    }
    class Room
    {
        public int[,] map;

        Vector2Int mapSize;
        Tile[,] tileMap;

        int roomDensity;
        int rooms;
        int mapLength;
        public int tileScore;
        public float sc = 0;

        public Room(Vector2Int size, int density)
        {
            mapSize = size;
            roomDensity = density;
            map = new int[mapSize.x, mapSize.y];
            tileMap = new Tile[mapSize.x, mapSize.y];

            rooms = 0;
            generateRandomMap();
            mapLength = mapSize.x * mapSize.y;
            //tileScore = tilesTypeScore();
        }

        public Room(Vector2Int size, int density, int[,] _map, int _rooms, Tile[,] _tileMap, int _tileScore)
        {
            mapSize = size;
            map = new int[mapSize.x, mapSize.y];
            tileMap = new Tile[mapSize.x, mapSize.y];

            roomDensity = density;
            Array.Copy(_map, map, mapSize.x * mapSize.y);
            rooms = _rooms;
            mapLength = mapSize.x * mapSize.y;
            tileScore = _tileScore;
        }


        static int corridorMidPoints = 10;
        static int corridor1Points = 8;
        static int cornerPoints = 10;
        static int edgePoints = 4;
        static int corridorPoints = 1;
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
            {Tile.ROOM, -30},
            {Tile.WALL, 10},
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
            {Tile.CROSS, 3 },
            {Tile.CORNERS, -8},
            {Tile.HOLE, 1 },
            {Tile.POINT, 1 },
            {Tile.OTHER, 0 },
            {Tile.BORDER, 0 },
            {Tile.FAILURE, -30 }
        };
        void generateRandomMap()
        {
            for (int i = 1; i < mapSize.x - 1; i++)
            {
                for (int j = 1; j < mapSize.y - 1; j++)
                {
                    int number = Random.Range(1, 101);
                    if (number < 50)
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
            bool[,] visited = new bool[mapSize.x, mapSize.y];

            int density = 0;
            int tile = 0;
            int separation = 0;
            for (int x = 1; x < mapSize.x - 1; x++)
            {
                for (int y = 1; y < mapSize.y - 1; y++)
                {
                    if (map[x, y] == 0)
                    {
                        density++;

                        Vector2Int v = new Vector2Int(x, y);
                        if (!visited[x, y])
                        {
                            separation++;
                            VisitNeighbours(v, visited);
                        }
                    }
                    Tile type = ClassifyTile(new Vector2Int(x, y));
                    tile += tilePoints[type];
                }
            }

            separation = -separation;


            float densityMultiplier = 5 * DensityScoreFunction(density);
            float separationMultiplier = 50 * mapSize.x * separation;
            float tileMultiplier = 1f * tile;

            //density 0-arraySize
            //float density = fastDensityScore();
            //float densityScore = DensityScoreFunction(density) * densityMultiplier;
            //if (densityScore < 0) Debug.LogWarning("densityScore: " + densityScore);

            // separation of rooms
            //float separationScore = SeperateRoomsScore() * separationMultiplier;
            // tile score


            //float score = density * densityMultiplier + tilesType * tilesTypeMultiplier;
            Debug.Log("den: " + densityMultiplier + ", sep: " + separationMultiplier + ", tile: " + tileMultiplier);
            float score = densityMultiplier + separationMultiplier + tileMultiplier;
            sc = score;
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

        Tile ClassifyTile(Vector2Int tile)
        {
            if (tile.x == 0 || tile.y == 0 || tile.x == mapSize.x - 1 || tile.y == mapSize.y - 1)
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
                map[tile.x, tile.y] = 1;
                //RecalculateNeighbours(new Vector2Int(tile.x, tile.y));
                return Tile.HOLE;
            }
            // point
            else if (t12 == 0 && t21 == 0 && t22 == 1 && t23 == 0 && t32 == 0)
            {
                map[tile.x, tile.y] = 0;
                //    RecalculateNeighbours(new Vector2Int(tile.x, tile.y));
                return Tile.POINT;
            }
            // corners sticking
            else if ((
                t11 == 0 && t12 == 1 && t21 == 1 ||
                t13 == 0 && t23 == 1 && t21 == 1 ||
                t31 == 0 && t32 == 1 && t23 == 1 ||
                t33 == 0 && t21 == 1 && t32 == 1) && t22 == 0)
            {
                return Tile.CORNERS;
            }
            else
            {
                return Tile.OTHER;
            }
        }


        public void Mutate()
        {
            int x = Random.Range(1, mapSize.x - 1);
            int y = Random.Range(1, mapSize.y - 1);
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
                }
            }
            for (int i = -1; i < 2; i++)
            {
                for (int j = -1; j < 2; j++)
                {
                    postScore += tilePoints[tileMap[tile.x + i, tile.y + j]];
                }
            }
            //Debug.Log("pre: " + preScore + ", post: " + postScore);

            tileScore += postScore - preScore;
        }

        public void Mutate(int x, int y)
        {
            if (map[x, y] == 0)
            {
                map[x, y] = 1;
                //rooms -= 1;
            }
            else
            {
                map[x, y] = 0;
                //rooms += 1;
            }
        }

        public void MultipleMutate(int n)
        {
            int prex = Random.Range(2, mapSize.x - 2);
            int prey = Random.Range(2, mapSize.y - 2);

            for (int i = 0; i < n; i++)
            {
                int x = Random.Range(prex - 1, prex + 2);
                int y = Random.Range(prey - 1, prey + 2);
                Mutate(x, y);
                //RecalculateNeighbours(new Vector2Int(x, y));
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

    void VisitNeighbours(Vector2Int tile, bool[,] visited, int[,] map, List<Vector2Int> room)
    {
        visited[tile.x, tile.y] = true;
        room.Add(new Vector2Int(tile.x, tile.y));
        Vector2Int down = new Vector2Int(tile.x, tile.y - 1);
        Vector2Int up = new Vector2Int(tile.x, tile.y + 1);
        Vector2Int left = new Vector2Int(tile.x - 1, tile.y);
        Vector2Int right = new Vector2Int(tile.x + 1, tile.y);


        if (down.y > 0 && map[down.x, down.y] == 0 && !visited[down.x, down.y])
        {
            VisitNeighbours(down, visited, map, room);
        }
        if (up.y < mapSize.y && map[up.x, up.y] == 0 && !visited[up.x, up.y])
        {
            VisitNeighbours(up, visited, map, room);
        }
        if (left.x > 0 && map[left.x, left.y] == 0 && !visited[left.x, left.y])
        {
            VisitNeighbours(left, visited, map, room);
        }
        if (right.x < mapSize.x && map[right.x, right.y] == 0 && !visited[right.x, right.y])
        {
            VisitNeighbours(right, visited, map, room);
        }
    }

    void CreateCorridor(Vector2Int center1, Vector2Int center2, int[,] map)
    {
        int startHorizontal = center1.x < center2.x ? center1.x : center2.x;
        int endHorizontal = center1.x > center2.x ? center1.x : center2.x;
        int startVertical = center1.y < center2.y ? center1.y : center2.y;
        int endVertical = center1.y > center2.y ? center1.y : center2.y;

        for (int x = startHorizontal; x <= endHorizontal; x++)
        {
            map[x, center1.y] = 0;
        }
        for (int y = startVertical; y <= endVertical; y++)
        {
            map[endHorizontal, y] = 0;
        }

    }

    List<Vector2Int> GetClosest(List<Vector2Int> room1, List<Vector2Int> room2)
    {
        int best = 100000;
        List<Vector2Int> closest = new List<Vector2Int>{ room1[0], room2[0] };

        for(int i = 0; i < room1.Count; i++)
        {
            for(int j = 0; j < room2.Count; j++)
            {
                int score = Mathf.Abs(room1[i].x - room2[j].x) + Mathf.Abs(room1[i].y - room2[j].y);
                if(score < best)
                {
                    closest[0] = room1[i];
                    closest[1] = room2[j];
                }
            }
        }

        return closest;
    }

    void ConnectMap(int[,] map)
    {
        bool[,] visited = new bool[mapSize.x, mapSize.y];
        List<List<Vector2Int>> rooms = new List<List<Vector2Int>>();

        for (int x = 1; x < mapSize.y - 1; x++)
        {
            for (int y = 1; y < mapSize.y - 1; y++)
            {
                Vector2Int v = new Vector2Int(x, y);
                if (map[x, y] == 0 && !visited[x, y])
                {
                    List<Vector2Int> room = new List<Vector2Int>();
                    VisitNeighbours(v, visited, map, room);
                    rooms.Add(room);
                }
            }
        }
        Debug.Log("Separate rooms: " + rooms.Count);
        for (int i = 0; i < rooms.Count-1; i++)
        {

            List<Vector2Int> closest = GetClosest(rooms[i], rooms[i + 1]);
            CreateCorridor(closest[0], closest[1], map);
        }
    }

    public override int[,] generateMap()
    {
        List<Room> rooms = new List<Room>();
        int[,] bestMap = new int[mapSize.x, mapSize.y];
        float bestScore = -100000000;

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
                int x = Random.Range(1, 4);
                int y = Random.Range(1, 5);
                rooms[j].MultipleMutate(x, y);
            }

            // mutate the best ones
            for (int j = 0; j < half; j++)
            {

                rooms[j].MultipleMutate(3);
            }

            // sort
            rooms = rooms.OrderByDescending(r => r.Score()).ToList();

            //check best
            if (rooms[0].sc > bestScore)
            {
                Array.Copy(rooms[0].map, bestMap, arraySize);
                bestScore = rooms[0].sc;
            }
            Debug.Log("Generation: " + i + " Score: " + rooms[0].sc);
        }
        ConnectMap(rooms[0].map);
        return rooms[0].map;
    }
}
