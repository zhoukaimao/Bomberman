using UnityEngine;
using System.Collections;

public static class Util{
    public static Vector2 World2Map(Vector3 worldPos)
    {
        return new Vector2(Mathf.FloorToInt(worldPos.x + CONSTANT.ROW / 2.0f), Mathf.FloorToInt(worldPos.z + CONSTANT.COL / 2.0f));
    }
    public static Vector3 Map2World(Vector2 mapPos)
    {
        return new Vector3(Mathf.Floor(mapPos.x - CONSTANT.ROW / 2), 0, Mathf.Floor(mapPos.y - CONSTANT.COL / 2));
    }
    public static float BlockDist(Vector2 dest, Vector2 src)
    {
        return Mathf.Abs(src.x - dest.x) + Mathf.Abs(src.y - dest.y);
    }
    public static bool ValidMapIndex(float indX,float indY)
    {
        return indX >= 0 && indX < CONSTANT.ROW && indY >= 0 && indY < CONSTANT.COL;
    }
}
