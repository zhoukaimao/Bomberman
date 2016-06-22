using UnityEngine;
using System.Collections;

public class MapParam{
    public CONSTANT.ObjEnum[,] map;
    public double bonusRate;
    public double trapRate;
    public MapParam()
    {

    }
    public MapParam(CONSTANT.ObjEnum[,] map, double bonusRate, double trapRate)
    {
        this.map = map;
        this.bonusRate = bonusRate;
        this.trapRate = trapRate;
    }

}
