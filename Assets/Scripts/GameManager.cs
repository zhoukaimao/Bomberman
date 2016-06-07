using UnityEngine;
using System.Collections;

public class GameManager : MonoBehaviour {
    private CONSTANT.ObjEnum[,] map;
    private int dwallLeft = 2;
	// Use this for initialization
	void Start () {
        map = new CONSTANT.ObjEnum[CONSTANT.ROW, CONSTANT.COL] { 
        {0,0,0,0,0,0,0,0,0,0,0,0,0,0,0},
        {0,0,0,0,0,0,0,CONSTANT.ObjEnum.Dwall,0,0,0,0,CONSTANT.ObjEnum.Dwall,0,0},
        {0,CONSTANT.ObjEnum.Dwall,CONSTANT.ObjEnum.Udwall,0,0,0,0,CONSTANT.ObjEnum.Udwall,CONSTANT.ObjEnum.Dwall,0,0,0,CONSTANT.ObjEnum.Udwall,CONSTANT.ObjEnum.Dwall,0},
        {0,0,CONSTANT.ObjEnum.Dwall,0,0,0,0,0,0,0,0,0,0,0,0},
        {0,0,0,0,0,0,0,0,0,0,0,0,0,0,0},
        {0,0,0,0,0,0,0,0,0,0,0,0,0,0,0},
        {0,0,0,0,0,0,0,0,0,0,0,0,CONSTANT.ObjEnum.Dwall,0,0},
        {0,CONSTANT.ObjEnum.Dwall,CONSTANT.ObjEnum.Udwall,0,0,0,0,CONSTANT.ObjEnum.Dwall,0,0,0,0,CONSTANT.ObjEnum.Udwall,CONSTANT.ObjEnum.Dwall,0},
        {0,0,CONSTANT.ObjEnum.Dwall,0,0,0,0,0,0,0,0,0,0,0,0},
        {0,0,0,0,0,0,0,0,0,0,0,0,0,0,0},
        {0,0,0,0,0,0,0,0,0,0,0,0,0,0,0},
        {0,0,CONSTANT.ObjEnum.Dwall,0,0,0,0,0,0,0,0,0,0,0,0},
        {0,CONSTANT.ObjEnum.Dwall,CONSTANT.ObjEnum.Udwall,0,0,0,CONSTANT.ObjEnum.Dwall,CONSTANT.ObjEnum.Udwall,CONSTANT.ObjEnum.Dwall,0,0,CONSTANT.ObjEnum.Dwall,CONSTANT.ObjEnum.Udwall,CONSTANT.ObjEnum.Dwall,0},
        {0,0,0,0,0,0,0,0,0,0,0,0,0,0,0},
        {0,0,0,0,0,0,0,0,0,0,0,0,0,0,0}
        };
        
	}
	
	// Update is called once per frame
	void Update () {
	
	}
    public int GetDwallLeft()
    {
        return dwallLeft;
    }
    public CONSTANT.ObjEnum[,] GetMap()
    {
        return map;
    }
}
