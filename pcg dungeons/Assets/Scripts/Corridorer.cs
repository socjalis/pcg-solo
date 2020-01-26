using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Corridorer
{
    //private class Room
    //{
    //    int topX;
    //    int topY;
    //    int bottomX;
    //    int bottomY;

    //    public Room(int x1, int y1, int x2, int y2)
    //    {
    //        this.topX = x1;
    //        this.topY = y1;
    //        this.bottomX = x2;
    //        this.bottomY = y2;
    //    }
    //}

    private int[,] org_map;
    private int[,] copy_map;
    
    private void redraw(int x, int y)
    {
        this.copy_map[x, y] = 0;
        if (this.org_map[x + 1, y] == 0 && this.copy_map[x + 1, y] != 0) redraw(x + 1, y);
        if (this.org_map[x, y + 1] == 0 && this.copy_map[x, y + 1] != 0) redraw(x, y + 1);
        if (this.org_map[x - 1, y] == 0 && this.copy_map[x - 1, y] != 0) redraw(x - 1, y);
        if (this.org_map[x, y - 1] == 0 && this.copy_map[x, y - 1] != 0) redraw(x, y - 1);
    }

    public int[,] makeCorridors(int[,] map)
    {
        int w = map.GetLength(0);
        int h = map.GetLength(1);


        System.Random random = new System.Random();


        for (int r = 0; r < 20; r++)
        {
            int dim = random.Next(0, 2);
            bool draw = false;
            if (dim == 0)
            {
                int x = random.Next(0, w);

                for (int y = 1; y < h-1; y++)
                { 
                    if (map[x,y] == 0)
                    {
                        draw = !draw;
                    }
                    if (draw) map[x, y] = 0;
                }
            } else
            {
                int y = random.Next(0, h);

                for (int x = 1; x < w-1; x++)
                {
                    if (map[x, y] == 0)
                    {
                        draw = !draw;
                    }
                    if (draw) map[x, y] = 0;
                }
            }
        }

        this.org_map = map;
        this.copy_map = new int[w, h];

        int start_x = -1;
        int start_y = -1;

        for (int x = 0; x < w; x++)
        {
            for (int y = 0; y < h; y++)
            {
                if (start_x < 0 && map[x,y] == 0)
                {
                    start_x = x;
                    start_y = y;
                }
                this.copy_map[x,y] = 1;
            }
        }

        Debug.Log(start_x);
        Debug.Log(start_y);

        this.redraw(start_x, start_y);

        return this.copy_map;
    }
}
