using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class AI
{
    static List<Vector2Int>[,] tree;
    public static Vector2Int mapSize;

    static public void CreateTree(int[,] map)
    {
        //Debug.Log("create tree");
        mapSize = new Vector2Int(map.GetLength(0), map.GetLength(1));
        tree = new List<Vector2Int>[mapSize.x, mapSize.y];
 
        for (int x = 0; x < mapSize.x; x++)
        {
            for (int y = 0; y < mapSize.y; y++)
            {
                if (map[x, y] == 0)
                {
                    tree[x, y] = new List<Vector2Int>();
                    for (int i = -1; i < 2; i++)
                    {
                        for (int j = -1; j < 2; j++)
                        {
                            if (x + i > 0 && x + i < mapSize.x && y + j > 0 && y + j < mapSize.y)
                            {
                                if (map[x+i,y+j] == 0)
                                {
                                    tree[x, y].Add(new Vector2Int(x + i, y + j));
                                }
                            }
                        }
                    }
                }
            }
        }
    }

    static public Vector2Int NextStep(Vector2Int start, Vector2Int dest)
    {
        //Debug.Log("Next step");
        List<Vector2Int> path = FindPath(start, dest);
        //Debug.Log("oddaje: " + path[0]);
        return path[0];
    }

    static List<Vector2Int> FindPath(Vector2Int start, Vector2Int dest)
    {
        System.Diagnostics.Stopwatch sw = new System.Diagnostics.Stopwatch();
        sw.Start();
        
        //Debug.Log("Find path");
        Queue<Vector2Int> q = new Queue<Vector2Int>();
        int[,] color = new int[mapSize.x, mapSize.y];
        int[,] distance = new int[mapSize.x, mapSize.y];
        Vector2Int[,] parent = new Vector2Int[mapSize.x, mapSize.y];

        color[start.x, start.y] = 1;
        distance[start.x, start.y] = 1;
        q.Enqueue(start);
        
        while(q.Count != 0)
        {
            Vector2Int t = q.Dequeue();
            //Debug.Log("dequeued: " + t);
            foreach(Vector2Int n in tree[t.x, t.y])
            {
                if(color[n.x, n.y] == 0)
                {
                    color[n.x, n.y] = 1;
                    distance[n.x, n.y] = color[t.x, t.y] + 1;
                    parent[n.x, n.y] = t;
                    q.Enqueue(n);
                    if (n.Equals(dest))
                        return GetPath(distance, parent, start, dest);

                    if (sw.Elapsed.TotalMilliseconds > 500)
                    {
                        Debug.Log("PRZERWANIE");
                        return new List<Vector2Int>();
                    }
                }
            }
            color[t.x, t.y] = 2;
        }
        return GetPath(distance, parent, start, dest);
    }

    static List<Vector2Int> GetPath(int[,] distance, Vector2Int[,] parent, Vector2Int start, Vector2Int dest)
    {
        //Debug.Log("GetPath");
        System.Diagnostics.Stopwatch sw = new System.Diagnostics.Stopwatch();
        sw.Start();
        List<Vector2Int> path = new List<Vector2Int>();
        path.Add(dest);
        Vector2Int cur = dest;
        if (distance[cur.x, cur.y] == 0)
            return path;

        while(cur.x != 0 || cur.y !=0)
        {
            Vector2Int p = parent[cur.x, cur.y];
            if (p.Equals(start))
            {
                path.Reverse();
                return path;
            }
            //Debug.Log("dodaję:" + cur.ToString());
            
            path.Add(parent[cur.x, cur.y]);
            cur = parent[cur.x, cur.y];
            if (sw.Elapsed.TotalMilliseconds > 50)
            {
                Debug.Log("PRZERWANIE");
                return new List<Vector2Int>();
            }
        }
        path.Reverse();
        return path;
    }
}
