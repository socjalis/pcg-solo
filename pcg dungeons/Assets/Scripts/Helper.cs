using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Helper
{
    static public Vector2Int WorldTo2d(Vector3 pos)
    {
        return new Vector2Int(Mathf.RoundToInt(pos.x + AI.mapSize.x / 2 - 0.5f), Mathf.RoundToInt(pos.z + AI.mapSize.y / 2 - 0.5f));
    }

    static public Vector3 _2dToWorld(Vector2Int pos)
    {
        return new Vector3(-AI.mapSize.x / 2 + 0.5f + pos.x, 0.5f, -AI.mapSize.y / 2 + 0.5f + pos.y);
    }
}
