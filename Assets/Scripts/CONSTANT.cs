using UnityEngine;
using System.Collections;
using System;

public static class CONSTANT {
    public const int ROW = 13;//rows of the map
    public const int COL = 26;//cols of the map
    public const float SZBOX = 1;//size of each box
    public static Vector2 ConvertToMap(Vector3 pos)
    {
        Vector2 mapPos = new Vector2();
        mapPos.x = (float)Math.Floor(pos.x / SZBOX);
        mapPos.y = (float)Math.Floor(pos.z / SZBOX);
        return mapPos;
    }
    public static Vector3 ConvertToWorld(Vector2 mapPos)
    {
        Vector3 pos = new Vector3();
        pos.x = mapPos.x * SZBOX + SZBOX / 2;
        pos.y = 0;
        pos.z = mapPos.y * SZBOX + SZBOX / 2;
        return pos;
    }
}
