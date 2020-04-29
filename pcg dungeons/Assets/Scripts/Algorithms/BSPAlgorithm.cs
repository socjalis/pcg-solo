using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BSPAlgorithm : Algorithm
{
    int mapWidth = 40;
    int mapHeight = 40;
    int minSize = 4;
    int wallThickness = 2;
    int wallSpace = 2;

    public override void setParameters(SortedDictionary<string, string> parameters)
    {
        mapWidth = int.Parse(parameters["Map width"]);
        mapHeight = int.Parse(parameters["Map height"]);
        minSize = int.Parse(parameters["Min. room size"]);
    }


    
    public override SortedDictionary<string, string> getParameters()
    {
        return new SortedDictionary<string, string>(){
            { "Map width", "string" },
            { "Map height", "string" },
            { "Min. room size", "string" }
        };
    }

    public override int[,] generateMap()
    {
        int [,] map = new int[mapWidth, mapHeight];
        for (int x = 0; x < mapWidth; x++)
        {
            for (int y = 0; y < mapHeight; y++)
            {
                map[x, y] = 1;
            }
        }
        Node tree = new Node();
        tree.lu = new Vector2Int(0, mapHeight-1);
        tree.rb = new Vector2Int(mapWidth-1, 0);

        List<Node> rooms = new List<Node>(); 
        DivideNode(tree, rooms);
        CreateRooms(rooms);
        CreateCorridors(tree, map);
        
        
        foreach(Node room in rooms)
        {
            for(int x=room.lu.x; x <= room.rb.x; x++)
            {
                for (int y = room.rb.y; y <= room.lu.y; y++)
                {
                    map[x, y] = 0;
                }
            }
        }

        return map;
    }

    void DivideNode(Node node, List<Node> rooms)
    {
        string[] directions = { "horizontal", "vertical" };
        string direction = directions[Random.Range(0,2)];
        int minL = node.lu.x + minSize + wallThickness+wallSpace;
        int maxR = node.rb.x - minSize - wallThickness-wallSpace;

        int maxU = node.lu.y - minSize - wallThickness-wallSpace;
        int minB = node.rb.y + minSize + wallThickness+wallSpace;

        if (minL >= maxR && maxU > minB)
        {
            direction = directions[1];
        }
        else if(maxU <= minB && minL < maxR)
        {
            direction = directions[0];
        }

        if (direction == "horizontal")
        {
            
            if (minL >= maxR)
            {
                CreateLeaf(node, rooms);
                return;
            }
            else
            {
                int line = Random.Range(minL, maxR+1);
                node.left = new Node();
                node.left.lu = node.lu;
                node.left.rb = new Vector2Int(line, node.rb.y);
                node.right = new Node();
                node.right.lu = new Vector2Int(line, node.lu.y);
                node.right.rb = node.rb;
                DivideNode(node.left, rooms);
                DivideNode(node.right, rooms);
            }
        }
        else
        {
            if (maxU <= minB)
            {
                CreateLeaf(node, rooms);
                return;
            }
            else
            {
                int line = Random.Range(minB, maxU+1);
                node.left = new Node();
                node.left.lu = node.lu;
                node.left.rb = new Vector2Int(node.rb.x, line);
                node.right = new Node();
                node.right.lu = new Vector2Int(node.lu.x, line);
                node.right.rb = node.rb;
                DivideNode(node.left, rooms);
                DivideNode(node.right, rooms);
            }
        }
    }

    void CreateLeaf(Node node, List<Node> rooms)
    {
        rooms.Add(node);
        Debug.Log(rooms.Count);
    }

    void CreateRooms(List<Node> rooms)
    {
        foreach(Node node in rooms)
        {
            //half++rooms
            //Debug.Log("lu: " + node.lu.ToString() + " rb: " + node.rb.ToString());
            int minLeft = node.lu.x + wallThickness;
            int maxLeft = Mathf.FloorToInt((node.lu.x + node.rb.x) / 2);
            int left = Random.Range(minLeft, maxLeft);
            int minRight = left + minSize;
            int maxRight = node.rb.x - wallThickness;
            //Debug.Log("minRight:" + minRight + " maxRight: " + maxRight);
            int right = Random.Range(minRight, maxRight + 1);
            //Debug.Log("LEFT: " + left + " RIGHT: " + right);

            int minBot = node.rb.y + wallThickness;
            int maxBot = Mathf.FloorToInt((node.rb.y + node.lu.y) / 2);
            int bot = Random.Range(minBot, maxBot);
            int minTop = bot + minSize;
            int maxTop = node.lu.y - wallThickness;
            int top = Random.Range(minTop, maxTop + 1);

            node.lu = new Vector2Int(left, top);
            node.rb = new Vector2Int(right, bot);

            //small rooms
            //Debug.Log("lu: " + node.lu.ToString() + " rb: " + node.rb.ToString());
            //int left = node.lu.x + wallThickness;
            //int right = node.rb.x - minSize;
            ////Debug.Log("left: " + left + "-" + right);

            //int l = random.Next(left, right);
            ////Debug.Log("right: " + (l + minSize) + "-" + (node.rb.x - wallThickness));
            //int r = random.Next(l + minSize, node.rb.x);

            //int up = node.lu.y - wallThickness;
            //int down = node.rb.y + minSize;
            ////Debug.Log("up: " + down + "-" + up);

            //int u = random.Next(down, up);
            ////Debug.Log("down: " + (node.rb.y + wallThickness) + "-" + (u - minSize));

            //int b = random.Next(node.rb.y + wallThickness, u);
            //node.lu = new Vector2Int(l, u);
            //node.rb = new Vector2Int(r, b);

            //max sized rooms
            //node.lu = node.lu + new Vector2Int(wallThickness, -wallThickness);
            //node.rb = node.rb + new Vector2Int(-wallThickness, wallThickness);
        }
    }

    void CreateCorridors(Node node, int[,] map)
    {
        if (node.left != null && node.right != null)
        {
            CreateCorridors(node.left, map);
            CreateCorridors(node.right, map);
            CreateCorridor(node.left, node.right, map);
            CreateCorridor(node, node.right, map);
        }
    }

    void CreateCorridor(Node room1, Node room2, int[,] map)
    {
        Vector2Int center1 = new Vector2Int((room1.lu.x + room1.rb.x) / 2, (room1.lu.y + room1.rb.y) / 2);
        Vector2Int center2 = new Vector2Int((room2.lu.x + room2.rb.x) / 2, (room2.lu.y + room2.rb.y) / 2);

        int startHorizontal = center1.x < center2.x ? center1.x : center2.x;
        int endHorizontal = center1.x > center2.x ? center1.x : center2.x;
        int startVertical = center1.y < center2.y ? center1.y : center2.y;
        int endVertical = center1.y > center2.y ? center1.y : center2.y;

        for(int x=startHorizontal; x<=endHorizontal; x++)
        {
            map[x, center1.y] = 0;
        }
        for(int y=startVertical; y<=endVertical; y++)
        {
            map[endHorizontal, y] = 0;
        }

    }
}