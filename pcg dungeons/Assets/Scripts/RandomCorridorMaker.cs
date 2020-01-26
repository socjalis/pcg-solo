using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RandomCorridorMaker : CorridorMaker
{
    private int[,] org_map;
    private int[,] copy_map;
    private int[,] return_map;

    private int[,,] rooms;

    private int w;
    private int h;

    System.Random random;

    private void redraw(int x, int y)
    {
        this.return_map[x, y] = 0;
        if (this.copy_map[x + 1, y] == 0 && this.return_map[x + 1, y] != 0) redraw(x + 1, y);
        if (this.copy_map[x, y + 1] == 0 && this.return_map[x, y + 1] != 0) redraw(x, y + 1);
        if (this.copy_map[x - 1, y] == 0 && this.return_map[x - 1, y] != 0) redraw(x - 1, y);
        if (this.copy_map[x, y - 1] == 0 && this.return_map[x, y - 1] != 0) redraw(x, y - 1);
    }

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

    private (int, int) corridor(int dim, int x, int y)
    {
        switch (dim) {
            case 0:
                for (int d = y+1; d < h - 1; d++)
                {
                    copy_map[x, d] = 0;
                    if (copy_map[x, d + 1] == 0)
                    {
                        return (x, d+1);
                    }
                    if (copy_map[x + 1, d] == 0)
                    {
                        return (x + 1, d);
                    }
                    if (copy_map[x - 1, d] == 0)
                    {
                        return (x - 1, d);
                    }
                }
                for (int d = h-1; d > y; d--)
                {
                    copy_map[x, d] = 1;
                }
                break;
            case 1:
                for (int d = y-1; d > 0; d--)
                {
                    copy_map[x, d] = 0;
                    if (copy_map[x, d - 1] == 0)
                    {
                        return (x, d - 1);
                    }
                    if (copy_map[x + 1, d] == 0)
                    {
                        return (x + 1, d);
                    }
                    if (copy_map[x - 1, d] == 0)
                    {
                        return (x - 1, d);
                    }
                }
                for (int d = 0; d < y; d++)
                {
                    copy_map[x, d] = 1;
                }
                break;
            case 2:
                for (int d = x+1; d < w -1; d++)
                {
                    copy_map[d, y] = 0;
                    if (copy_map[d + 1, y] == 0)
                    {
                        return (d + 1, y);
                    }
                    if (copy_map[d, y + 1] == 0)
                    {
                        return (d, y + 1);
                    }
                    if (copy_map[d, y - 1] == 0)
                    {
                        return (d, y - 1);
                    }
                }
                for (int d = w - 1; d > x; d--)
                {
                    copy_map[d, y] = 1;
                }
                break;
            case 3:
                for (int d = x-1; d > 0; d--)
                {
                    copy_map[d, y] = 0;
                    if (copy_map[d - 1, y] == 0)
                    {
                        return (d - 1, y);
                    }
                    if (copy_map[d, y + 1] == 0)
                    {
                        return (d, y + 1);
                    }
                    if (copy_map[d, y - 1] == 0)
                    {
                        return (d, y - 1);
                    }
                }
                for (int d = 0; d < x; d++)
                {
                    copy_map[d, y] = 1;
                }
                break;
        }
        return (-1, -1);
    }

    private void drawCorridors(int x, int y)
    {
        if (org_map[x, y] == 1) return;

        int x2;
        int y2;
        (x2, y2) = findBottomCorner(x, y);

        int xx = random.Next(x, x2);
        int xx2 = random.Next(x, x2);

        int yy = random.Next(y, y2);
        int yy2 = random.Next(y, y2);

        int nx, ny, nsx, nsy;

        if (rooms[x, y, 0] == 0)
        {
            (nx, ny) = corridor(0, xx, y2);
            rooms[x, y, 0] = 1;
            (nsx, nsy) = findTopCorner(nx, ny);
            if (nsx >= 0 && nsy >= 0)
            {
                rooms[nsx, nsy, 1] = 1;
                drawCorridors(nsx, nsy);
            }
        }

        if (rooms[x,y,1] == 0)
        {
            (nx, ny) = corridor(1, xx2, y);
            rooms[x,y,1] = 1;
            (nsx, nsy) = findTopCorner(nx, ny);
            if (nsx >= 0 && nsy >= 0)
            {
                rooms[nsx, nsy, 0] = 1;
                drawCorridors(nsx, nsy);
            }
        }

        if (rooms[x, y, 2] == 0)
        {
            (nx, ny) = corridor(2, x2, yy);
            rooms[x, y, 2] = 1;
            (nsx, nsy) = findTopCorner(nx, ny);
            if (nsx >= 0 && nsy >= 0)
            {
                rooms[nsx, nsy, 3] = 1;
                drawCorridors(nsx, nsy);
            }
        }

        if (rooms[x, y, 3] == 0)
        {
            (nx, ny) = corridor(3, x, yy2);
            rooms[x, y, 3] = 1;
            (nsx, nsy) = findTopCorner(nx, ny);
            if (nsx >= 0 && nsy >= 0)
            {
                rooms[nsx, nsy, 2] = 1;
                drawCorridors(nsx, nsy);
            }
        }


    }

    override public int[,] makeCorridors(int[,] map)
    {
        w = map.GetLength(0);
        h = map.GetLength(1);
        
        random = new System.Random();
        
        this.org_map = map.Clone() as int[,];
        this.copy_map = map.Clone() as int[,];
        this.return_map = new int[w, h];

        this.rooms = new int[w, h, 4];

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
                this.return_map[x, y] = 1;
            }
        }


        this.drawCorridors(start_x, start_y);

        this.redraw(start_x, start_y);


        for (int z = 0; z < w; z++)
        {
            for (int r = 0; r < h; r++)
            {
                if (org_map[z, r] == 0)
                    return_map[z, r] = 0;
            }
        }


        return this.return_map;
    }
}
