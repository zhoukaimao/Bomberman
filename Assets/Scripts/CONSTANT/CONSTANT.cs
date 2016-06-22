using UnityEngine;
using System.Collections;

public class CONSTANT{
    public static string SaveDataPath = Application.dataPath + @"/Resources/Settings/";
    public const int ROW = 15;
    public const int COL = 15;
    //public const int LENGTH_NUM = 20, WIDTH_NUM = 16;//必须是偶数
    public enum ObjEnum
    {
        Empty=0,
        Udwall,
        Dwall,
        Bomb,
        Bonus,
        Trap,
        Flame,
        Player,
        Npc
    };
    public enum BonusEnum
    {
        AddHP = 0,
        AddBomb,
        AddFlame,
        AddSpeed
    };
    public enum TrapEnum
    {
        Hurt = 0

    };
    public enum NPCEnum
    {
        detroyer = 0,//try to destroy wall
        carrful,//try to avoid player
        crazy,//always try to kill player
        guard//patrol in a given field
    };
    public enum MapCell
    {
        Null,
        Wall_Normal, Wall_Destroy, Player,
        Enemy_Normal, Enemy_Eat, Enemy_Throw
    }

}
