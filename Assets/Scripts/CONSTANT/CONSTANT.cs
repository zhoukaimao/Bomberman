using UnityEngine;
using System.Collections;

public class CONSTANT{

    public const int ROW = 15;
    public const int COL = 15;
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
        Hurt = 0,
        Slow,
        DropBomb

    };
    public enum NPCEnum
    {
        detroyer=0,//try to destroy wall
        carrful,//try to avoid player
        crazy,//always try to kill player
        guard//patrol in a given field
    }
    public enum GameMode
    {
        Normal,
        Survive
    }
    public enum MapFolder
    {
        Levels,
        UserMap
    }

}
