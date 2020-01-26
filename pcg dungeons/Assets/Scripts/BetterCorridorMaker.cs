using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BetterCorridorMaker : CorridorMaker
{
    private int[,] org_map;
    private int[,] return_map;

    private int w;
    private int h;

    System.Random random;

    //private void redraw(int x, int y)
    //{
    //    this.return_map[x, y] = 0;
    //    if (this.copy_map[x + 1, y] == 0 && this.return_map[x + 1, y] != 0) redraw(x + 1, y);
    //    if (this.copy_map[x, y + 1] == 0 && this.return_map[x, y + 1] != 0) redraw(x, y + 1);
    //    if (this.copy_map[x - 1, y] == 0 && this.return_map[x - 1, y] != 0) redraw(x - 1, y);
    //    if (this.copy_map[x, y - 1] == 0 && this.return_map[x, y - 1] != 0) redraw(x, y - 1);
    //}

    private (int, int) findBottomCorner(int x, int y)
    {
        int x2 = -1;
        int y2 = -1;
        for (int d = x; d < w; d++)
        {
            if (org_map[d + 1, y] == 1)
            {
                x2 = d;
                break;
            }
        }

        for (int d = y; d < h; d++)
        {
            if (org_map[x, d + 1] == 1)
            {
                y2 = d;
                break;
            }
        }
        return (x2, y2);
    }

    private (int, int) findTopCorner(int x, int y)
    {
        int x2 = -1;
        int y2 = -1;
        for (int d = x; d > 0; d--)
        {
            if (org_map[d - 1, y] == 1)
            {
                x2 = d;
                break;
            }
        }

        for (int d = y; d > 0; d--)
        {
            if (org_map[x, d - 1] == 1)
            {
                y2 = d;
                break;
            }
        }
        return (x2, y2);
    }

    private class Room
    {
        public int x;
        public int y;
        public int x2;
        public int y2;
        public int centerX;
        public int centerY;

        public bool connected = false;

        public Room(int x, int y, int x2, int y2)
        {
            this.x = x;
            this.y = y;
            this.x2 = x2;
            this.y2 = y2;
            this.centerX = (int) System.Math.Floor((x + x2) / 2.0);
            this.centerY = (int) System.Math.Floor((y + y2) / 2.0);
        }
        public (int, int) center()
        {
            return (centerX, centerY);
        }
    }

    private Room createRoom(int x, int y)
    {
        if (org_map[x - 1, y] == 1 && org_map[x, y - 1] == 1)
        {
            (x, y) = findTopCorner(x, y);
        }
        int x2, y2;
        (x2, y2) = findBottomCorner(x, y);
        return new Room(x, y, x2, y2);
    }

    private void fill(int x, int y, int x2, int y2, int v, int[,] m)
    {
        for (int i = x; i <= x2; i++)
        {
            for (int j = y; j <= y2; j++)
            {
                m[i, j] = v;
            }
        }
    }

    private Room findNearestRoom(Room room, Room[] rooms)
    {
        Room nearestRoom = null;
        double nearestDistance = double.MaxValue;
        double distance;

        foreach (Room r in rooms)
        {
            if (r.connected) continue;
            distance = System.Math.Sqrt(System.Math.Pow((r.centerX - room.centerX), 2) + System.Math.Pow((r.centerY - room.centerY), 2));
            //Debug.Log("" + r.centerX + "-" + room.centerX + "/" + r.centerY + "-" + room.centerY + "/" + distance);

            if (distance < nearestDistance)
            {
                nearestDistance = distance;
                nearestRoom = r;
            }
        }

        return nearestRoom;
    }

    private Room[] findRooms()
    {
        int[,] copy_map = org_map.Clone() as int[,];
        List<Room> rooms = new List<Room>();

        for (int x = 0; x < w; x++)
        {
            for (int y = 0; y < h; y++)
            {
                if (copy_map[x, y] == 0)
                {
                    Room r = createRoom(x, y);
                    fill(r.x, r.y, r.x2, r.y2, 1, copy_map);
                    rooms.Add(r);
                }
            }
        }

        return rooms.ToArray();
    }

    private void drawCorridor(Room r1, Room r2, int[,] map)
    {   
        if (r1.centerX < r2.centerX)
        {
            for (int x = r1.centerX; x <= r2.centerX; x++)
            {
                map[x, r1.centerY] = 0;
            }
        } else
        {
            for (int x = r2.centerX; x <= r1.centerX; x++)
            {
                map[x, r1.centerY] = 0;
            }
        }
        if (r1.centerY < r2.centerY)
        {
            for (int y = r1.centerY; y <= r2.centerY; y++)
            {
                map[r2.centerX, y] = 0;
            }
        } else
        {
            for (int y = r2.centerY; y <= r1.centerY; y++)
            {
                map[r2.centerX, y] = 0;
            }
        }   
    }

    override public int[,] makeCorridors(int[,] map)
    {
        w = map.GetLength(0);
        h = map.GetLength(1);

        this.org_map = map.Clone() as int[,];

        random = new System.Random();

        int start_x = -1;
        int start_y = -1;

        for (int x = 0; x < w; x++)
        {
            for (int y = 0; y < h; y++)
            {
                if (start_x < 0 && map[x, y] == 0)
                {
                    start_x = x;
                    start_y = y;
                }
            }
        }

        Room[] rooms = findRooms();
        Room nearest;


        foreach (Room r in rooms)
        {
            Debug.Log("(" + r.x + "," + r.y + ") do (" + r.x2 + "," + r.y2 + ") środek (" + r.centerX + "," + r.centerY + ")");
            r.connected = true;
            nearest = findNearestRoom(r, rooms);
            if (nearest != null)
            {
                drawCorridor(r, nearest, map);
            }
            else Debug.LogWarning("Nie znaleziono najbliższego pokoju!");
        }



        return map;
    }
}