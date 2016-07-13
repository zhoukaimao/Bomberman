using UnityEngine;
using System.Collections;
using System.Collections.Generic;
public class GameManager : MonoBehaviour {
    AudioManager _audioManager;
    UIManager _uiManager;
    /// <summary>
    /// Prefabs
    /// </summary>
    public GameObject[] dwallPrefabs;
    public GameObject[] udwallPrefabs;
    public GameObject playerPrefab;
    public GameObject[] npcPrefabs;
    public GameObject bombPrefab;
    public GameObject flamePrefab;
    public GameObject[] bonusPrefabs;
    public GameObject[] trapPrefabs;

    /// <summary>
    /// Map
    /// </summary>
    private CONSTANT.ObjEnum[,] initMap;//initialize map, loaded from map data file
    private CONSTANT.ObjEnum[,] runtimeMap;//store info about what's in this grid
    private GameObject[,] objectMap;//store index to objects in game map
    private float[,] dangerMap;//dangerness in each grid, that is, when flame will fill this grid
    private int dwallLeft = 0;

    private float timer = 180000f;

    /// <summary>
    /// Player and NPC and Bombs
    /// </summary>
    private GameObject player;
    private List<GameObject> npc;
    private List<GameObject> bombs;//store bombs in current frame

    private float bombLifeTime = 3000f;
    private float flameLifeTime = 1000f;
    [Range(0,1)]
    public float bonusRate;
    [Range(0, 1)]
    public float trapRate;

    private string gameMode;
	/// <summary>
	/// initialization, load map data, create scene
	/// </summary>
	void Start () {
        Time.timeScale = 0;
        runtimeMap = new CONSTANT.ObjEnum[CONSTANT.ROW, CONSTANT.COL];
        objectMap = new GameObject[CONSTANT.ROW, CONSTANT.COL];
        initMap = new CONSTANT.ObjEnum[CONSTANT.ROW, CONSTANT.COL];
        dangerMap = new float[CONSTANT.ROW, CONSTANT.COL];
        npc = new List<GameObject>();
        bombs = new List<GameObject>();
        runtimeMap.Initialize();
        objectMap.Initialize();
        initMap.Initialize();
        dangerMap.Initialize();

        LoadMap();
        CreateScene();
        _audioManager = GetComponent<AudioManager>();
        _uiManager = GetComponent<UIManager>();

        gameMode = PlayerPrefs.GetString("GameMode");
	}
	
	/// <summary>
	/// update danger map per frame
	/// </summary>
	void Update () {
        UpdateDangerMap();
        timer -= Time.deltaTime*1000;
        if (timer <= 0)
        {
            GameOver();
        }
        if (player == null || npc.Count == 0)
        {
            GameOver();
        }
	}
    /// <summary>
    /// update danger map
    /// </summary>
    void UpdateDangerMap()
    {
        for (var i = 0; i < CONSTANT.ROW; i++)
        {
            for (var j = 0; j < CONSTANT.COL; j++)
            {
                dangerMap[i, j] = Mathf.Infinity;
            }
        }
        //sort bomb list first
        //bombs.Sort();
        //for each bomb, fill its blast grid into its left time
        foreach (GameObject bomb in bombs)
        {
            if (bomb == null) continue;
            var leftTime = bomb.GetComponent<Bomb>().LeftTime;
            if (bomb.GetComponent<Bomb>().CanBetriggered()) leftTime = 0;
            var flameDist = bomb.GetComponent<Bomb>().flameDist;
            var bombPos = Util.World2Map(bomb.transform.position);
            int bombX = (int)bombPos.x;
            int bombY = (int)bombPos.y;
            leftTime = Mathf.Min(leftTime, dangerMap[bombX, bombY]);
            //if dangerness in this grid is smaller than leftTime, 
            //which means that this bomb will be triggered by other flame, 
            //then set leftTime to be the dangerness, the smaller one
            Vector2[] blastgrid = BlastGrid(bombX,bombY,flameDist);
            foreach (Vector2 grid in blastgrid)
            {
                int indexX = (int)grid.x;
                int indexY = (int)grid.y;
                dangerMap[indexX, indexY] = Mathf.Min(dangerMap[indexX,indexY],leftTime);
            }
        }
    }

    //void SetGridDanger(GameObject bomb,float leftTime)
    //{
    //    if (bomb == null)
    //    {
    //        return;
    //    }
    //    var flameDist = bomb.GetComponent<Bomb>().flameDist;
    //    var bombPos = Util.World2Map(bomb.transform.position);
    //    var bombX = (int)bombPos.x;
    //    var bombY = (int)bombPos.y;
    //    Vector2[] blastgrid = BlastGrid(bombX,bombY,flameDist);
    //    foreach (Vector2 grid in blastgrid)
    //    {
    //        int indexX = (int)grid.x;
    //        int indexY = (int)grid.y;
    //        dangerMap[indexX, indexY] = Mathf.Min(leftTime, dangerMap[indexX, indexY]);
    //        if (runtimeMap[indexX, indexY] == CONSTANT.ObjEnum.Bomb &&!(indexX==bombX&&indexY==bombY))
    //        {
    //            GameObject triggerBomb = objectMap[indexX, indexY];
    //            var newLeftTime = triggerBomb.GetComponent<Bomb>().LeftTime;
    //            if (triggerBomb.GetComponent<Bomb>().owner.GetComponent<Bomberman>().CanTrigger) newLeftTime = 0;
    //            if (newLeftTime > leftTime)
    //            {
    //                SetGridDanger(triggerBomb,leftTime);
    //            }
    //        }
    //    }
    //}
    /// <summary>
    /// find which grid will be filled with flame because of this bomb blast
    /// </summary>
    /// <param name="bombX"></param>
    /// <param name="bombY"></param>
    /// <param name="flameDist"></param>
    /// <returns></returns>
    public Vector2[] BlastGrid(int bombX,int bombY,float flameDist)
    {
        List<Vector2> blastgrid = new List<Vector2>();
        blastgrid.Add(new Vector2(bombX, bombY));
        bool blocked1 = false; bool blocked2 = false; bool blocked3 = false; bool blocked4 = false;
        for (var i = 1; i <= flameDist; i++)//blocked by udwall
        {
            if (!blocked1&&Util.ValidMapIndex(bombX + i, bombY))
            {
                if ( runtimeMap[bombX + i, bombY] == CONSTANT.ObjEnum.Udwall) blocked1 = true;
                else blastgrid.Add(new Vector2(bombX + i, bombY));
            }
            if (!blocked2&&Util.ValidMapIndex(bombX-i,bombY))
            {
                if (runtimeMap[bombX - i, bombY] == CONSTANT.ObjEnum.Udwall) blocked2 = true;
                else blastgrid.Add(new Vector2(bombX - i, bombY));
            }
            if (!blocked3&&Util.ValidMapIndex(bombX,bombY+i))
            {
                if (runtimeMap[bombX, bombY+i] == CONSTANT.ObjEnum.Udwall) blocked3 = true;
                else blastgrid.Add(new Vector2(bombX , bombY+i));
            }
            if (!blocked4&&Util.ValidMapIndex(bombX,bombY-i))
            {
                if (runtimeMap[bombX, bombY - i] == CONSTANT.ObjEnum.Udwall) blocked4 = true;
                else blastgrid.Add(new Vector2(bombX, bombY - i));
            }
        }
        return blastgrid.ToArray();
    }
    /// <summary>
    /// get runtime map
    /// </summary>
    /// <returns></returns>
    public CONSTANT.ObjEnum[,] GetMap()
    {
        return runtimeMap;
    }
    /// <summary>
    /// get danger map
    /// </summary>
    /// <returns></returns>
    public float[,] GetDangerMap()
    {
        return dangerMap;
    }
    /// <summary>
    /// get player
    /// </summary>
    /// <returns></returns>
    public GameObject GetPlayer()
    {
        return player;
    }
    public List<GameObject> GetNPC()
    {
        return npc;
    }
    /// <summary>
    /// get bomb list
    /// </summary>
    /// <returns></returns>
    public List<GameObject> GetBombs()
    {
        return bombs;
    }
    /// <summary>
    /// get number of dwall left
    /// </summary>
    /// <returns></returns>
    public int GetDwallLeft()
    {
        return dwallLeft;
    }
    /// <summary>
    /// get number of npc left
    /// </summary>
    /// <returns></returns>
    public int GetNPCLeft()
    {
        return npc.Count;
    }
    public float GetTimer()
    {
        return timer;
    }
    public void DestroyNPC(GameObject Deadnpc)
    {
        npc.Remove(Deadnpc);
        Destroy(Deadnpc);

    }
    public void DestroyBomberman(GameObject bomberman)
    {
        if (bomberman.GetComponent<NPC>() != null)
        {
            npc.Remove(bomberman);
        }
        else if (bomberman.GetComponent<Player>() != null)
        {
            
            //GameOver();
        }
        var ragdollPrefab = bomberman.GetComponent<Bomberman>().GetRagdollPrefab();
        var ragdoll = Instantiate(ragdollPrefab, bomberman.transform.position, bomberman.transform.rotation);
        Destroy(bomberman);
        Destroy(ragdoll, 5);
    }
    public void GameOver()
    {
<<<<<<< HEAD
        if (player == null)
        {
            _uiManager.Lose();
        }
        else if (gameMode.Equals("Normal"))
        {
            if (timer <= 0)
            {
                _uiManager.Lose();
            }
            else if (npc.Count == 0)
            {
                _uiManager.Win();
            }
        }
        else if (gameMode.Equals("Survive"))
        {
            _uiManager.Win();
        }
=======
        Time.timeScale = 0;
>>>>>>> parent of 3152d9f... Add map editor and UI
    }
    /// <summary>
    /// Instance bomb, 
    /// set bomb info, 
    /// update map, 
    /// decrease bombleft
    /// </summary>
    /// <param name="bomberman"></param>
    public void PlaceBomb(GameObject bomberman)
    {
        Vector2 vec = Util.World2Map(bomberman.transform.position);
        int indexX = (int)vec.x;
        int indexY = (int)vec.y;
        if (Util.ValidMapIndex(indexX, indexY)&&runtimeMap[indexX,indexY]!= CONSTANT.ObjEnum.Bomb)
        {
            runtimeMap[indexX, indexY] = CONSTANT.ObjEnum.Bomb;
            GameObject bomb = Instantiate(bombPrefab);
            objectMap[indexX, indexY] = bomb;
            bomb.transform.position = Util.Map2World(new Vector2(indexX, indexY));
            bomb.GetComponent<Bomb>().owner = bomberman;
            bomb.GetComponent<Bomb>().flameDist = bomberman.GetComponent<Bomberman>().flameDist;
            bomb.GetComponent<Bomb>().lifeTime = bombLifeTime;
            bomberman.GetComponent<Bomberman>().DecreaseBombLeft();
            _audioManager.PlayPlaceBombAudio();
            bombs.Add(bomb);
        }
        
    }

    /// <summary>
    /// destroy bomb
    /// create flame
    /// bombleft increase
    /// update map
    /// </summary>
    /// <param name="bomb"></param>
    public void BombBlast(GameObject bomb)
    {
        var flameDist = bomb.GetComponent<Bomb>().flameDist;
        var owner = bomb.GetComponent<Bomb>().owner;
        var bombPos = Util.World2Map(bomb.transform.position);
        var bombX = (int)bombPos.x;
        var bombY = (int)bombPos.y;
        bombs.Remove(bomb);
        Destroy(bomb);//destroy bomb
        objectMap[bombX, bombY] = null;
        runtimeMap[bombX, bombY] = CONSTANT.ObjEnum.Empty;
        
        if(owner!=null)
            owner.GetComponent<Bomberman>().IncreaseBombLeft();//increase bomb left
        FlameResult(bombX, bombY);
        bool blocked1 = false; bool blocked2 = false; bool blocked3 = false; bool blocked4 = false;
        for (var i = 1; i <= flameDist; i++)//blocked by udwall
        {
            if (!blocked1)
            {
                blocked1 = FlameResult(bombX+i,bombY);
            }
            if (!blocked2)
            {
                blocked2 = FlameResult(bombX - i, bombY);
            } 
            if (!blocked3)
            {
                blocked3 = FlameResult(bombX, bombY + i);
            } 
            if (!blocked4)
            {
                blocked4 = FlameResult(bombX, bombY - i);
            }
        }

        _audioManager.PlayBombBlastAudio();
    }
    /// <summary>
    /// Instance flame
    /// record in flame component whether destroy a wall or take place of a bonus or trap
    /// return if blocked
    /// </summary>
    /// <param name="indexX"></param>
    /// <param name="indexY"></param>
    /// <returns></returns>
    public bool FlameResult(int indexX, int indexY)
    {
        bool blocked = false;
        if (!Util.ValidMapIndex(indexX, indexY) || runtimeMap[indexX, indexY] == CONSTANT.ObjEnum.Udwall) return true;
        var flame = Instantiate(flamePrefab);
        flame.transform.position = Util.Map2World(new Vector2(indexX, indexY));
        flame.GetComponent<Flame>().lifeTime = flameLifeTime;
        if (objectMap[indexX, indexY] != null)
        {
            if (runtimeMap[indexX, indexY] == CONSTANT.ObjEnum.Dwall)
            {
                flame.GetComponent<Flame>().destroyWall = true;
                dwallLeft--;
                Destroy(objectMap[indexX, indexY]);
                blocked = true;
            }
            else if (runtimeMap[indexX, indexY] == CONSTANT.ObjEnum.Bonus)
            {
                flame.GetComponent<Flame>().takePlaceBonus = objectMap[indexX, indexY];
            }
            else if (runtimeMap[indexX, indexY] == CONSTANT.ObjEnum.Trap)
            {
                flame.GetComponent<Flame>().takePlaceTrap = objectMap[indexX, indexY];
            }
            else if (runtimeMap[indexX, indexY] == CONSTANT.ObjEnum.Bomb)
            {
                BombBlast(objectMap[indexX,indexY]);
                runtimeMap[indexX,indexY]= CONSTANT.ObjEnum.Empty;
            }
            
            
        }
        runtimeMap[indexX, indexY] = CONSTANT.ObjEnum.Flame;
        objectMap[indexX, indexY] = flame;
        
        return blocked;
    }
    /// <summary>
    /// Destroy flame
    /// recover bonus or trap if have
    /// if destroy wall, create bonus
    /// </summary>
    /// <param name="flame"></param>
    public void FlameDestroy(GameObject flame)
    {
        var flamePos = Util.World2Map(flame.transform.position);
        int indexX = (int)flamePos.x;
        int indexY = (int)flamePos.y;
        Flame flameInfo = flame.GetComponent<Flame>();
        if (flameInfo.destroyWall)
        {
            if (Random.Range(0, 100) / 100f > 1 - bonusRate)
            {
                if (Random.Range(0, 100) / 100f > 1 - trapRate)
                {
                    var trap = Instantiate(trapPrefabs[Random.Range(0, trapPrefabs.Length)]);
                    trap.transform.position = flame.transform.position;
                    runtimeMap[indexX, indexY] = CONSTANT.ObjEnum.Trap;
                    objectMap[indexX, indexY] = trap;
                }
                else
                {
                    var bonus = Instantiate(bonusPrefabs[Random.Range(0, bonusPrefabs.Length)]);
                    bonus.transform.position = flame.transform.position;
                    runtimeMap[indexX, indexY] = CONSTANT.ObjEnum.Bonus;
                    objectMap[indexX, indexY] = bonus;
                }
            }
            else
            {
                runtimeMap[indexX, indexY] = CONSTANT.ObjEnum.Empty;
                objectMap[indexX, indexY] = null;
            }
        }
        else if (flameInfo.takePlaceBonus != null)
        {
            runtimeMap[indexX, indexY] = CONSTANT.ObjEnum.Bonus;
            objectMap[indexX, indexY] = flameInfo.takePlaceBonus;
        }
        else if (flameInfo.takePlaceTrap != null)
        {
            runtimeMap[indexX, indexY] = CONSTANT.ObjEnum.Trap;
            objectMap[indexX, indexY] = flameInfo.takePlaceTrap;
        }
        else 
        {
            runtimeMap[indexX, indexY] = CONSTANT.ObjEnum.Empty;
            objectMap[indexX, indexY] = null;
        }

        Destroy(flame);
    }
    public void PickupBonus(GameObject bonus)
    {
        var pos = Util.World2Map(bonus.transform.position);
        int indexX = (int)pos.x;
        int indexY = (int)pos.y;
        runtimeMap[indexX, indexY] = CONSTANT.ObjEnum.Empty;
        objectMap[indexX, indexY] = null;
        Destroy(bonus);
        _audioManager.PlayPickupBonusAudio();
    }
    public void PickupTrap(GameObject trap)
    {
        var pos = Util.World2Map(trap.transform.position);
        int indexX = (int)pos.x;
        int indexY = (int)pos.y;
        runtimeMap[indexX, indexY] = CONSTANT.ObjEnum.Empty;
        objectMap[indexX, indexY] = null;
        Destroy(trap);
        _audioManager.PlayPickupTrapAudio();
    }
    public void DestroyTrap(GameObject trap)
    {
        var pos = Util.World2Map(trap.transform.position);
        int indexX = (int)pos.x;
        int indexY = (int)pos.y;
        runtimeMap[indexX, indexY] = CONSTANT.ObjEnum.Empty;
        objectMap[indexX, indexY] = null;
        Destroy(trap);
    }
    /// <summary>
    /// Load Map data from Json file or any other ways
    /// </summary>
    void LoadMap()
    {
<<<<<<< HEAD
        string myCurrentMapName = MainGameManager.currentMapFile;
        initMap = new CONSTANT.ObjEnum[CONSTANT.ROW, CONSTANT.COL];
        MapParam mapParam = new DataStoreProcessor().Load<MapParam>(myCurrentMapName, true);
        initMap = mapParam.map;
        bonusRate = (float)mapParam.bonusRate;
        trapRate = (float)mapParam.trapRate;
        //initMap = new CONSTANT.ObjEnum[CONSTANT.ROW, CONSTANT.COL] { 
        //{CONSTANT.ObjEnum.Player,0,0,0,0,0,0,0,0,0,0,0,0,0,0},
        //{0,0,0,0,0,0,0,CONSTANT.ObjEnum.Dwall,0,0,0,0,CONSTANT.ObjEnum.Dwall,0,0},
        //{0,CONSTANT.ObjEnum.Dwall,CONSTANT.ObjEnum.Udwall,0,0,0,0,CONSTANT.ObjEnum.Udwall,CONSTANT.ObjEnum.Dwall,0,0,0,CONSTANT.ObjEnum.Udwall,CONSTANT.ObjEnum.Dwall,0},
        //{0,0,CONSTANT.ObjEnum.Dwall,0,0,0,0,0,0,0,0,0,0,0,0},
        //{0,0,0,0,0,0,0,0,0,0,0,0,0,0,0},
        //{0,0,0,0,0,0,0,0,0,0,0,0,0,0,0},
        //{0,0,0,0,0,0,0,0,0,0,0,0,CONSTANT.ObjEnum.Dwall,0,0},
        //{0,CONSTANT.ObjEnum.Dwall,CONSTANT.ObjEnum.Udwall,0,0,0,0,CONSTANT.ObjEnum.Dwall,0,0,0,0,CONSTANT.ObjEnum.Udwall,CONSTANT.ObjEnum.Dwall,0},
        //{0,0,CONSTANT.ObjEnum.Dwall,0,0,0,0,0,0,0,0,0,0,0,0},
        //{0,0,0,0,0,0,0,0,0,0,0,0,0,0,0},
        //{0,0,0,0,0,0,0,0,0,0,0,0,0,0,0},
        //{0,0,CONSTANT.ObjEnum.Dwall,0,0,0,0,0,0,0,0,0,0,0,0},
        //{0,CONSTANT.ObjEnum.Dwall,CONSTANT.ObjEnum.Udwall,0,0,0,CONSTANT.ObjEnum.Dwall,CONSTANT.ObjEnum.Udwall,CONSTANT.ObjEnum.Dwall,0,0,CONSTANT.ObjEnum.Dwall,CONSTANT.ObjEnum.Udwall,CONSTANT.ObjEnum.Dwall,0},
        //{0,0,0,0,0,0,0,0,0,0,0,0,0,0,0},
        //{CONSTANT.ObjEnum.Npc,0,0,0,0,0,0,0,0,0,0,0,0,0,CONSTANT.ObjEnum.Npc}
        //};
=======
        initMap = new CONSTANT.ObjEnum[CONSTANT.ROW, CONSTANT.COL] { 
        {CONSTANT.ObjEnum.Player,0,0,0,0,0,0,0,0,0,0,0,0,0,0},
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
        {CONSTANT.ObjEnum.Npc,0,0,0,0,0,0,0,0,0,0,0,0,0,CONSTANT.ObjEnum.Npc}
        };
>>>>>>> parent of 3152d9f... Add map editor and UI
    }
    /// <summary>
    /// according to map loaded, create game scene, and initialize variables
    /// </summary>
    void CreateScene()
    {
<<<<<<< HEAD
=======
        npc = new List<GameObject>();
        runtimeMap = initMap;
        objectMap = new GameObject[CONSTANT.ROW, CONSTANT.COL];
>>>>>>> parent of 3152d9f... Add map editor and UI
        for (int i = 0; i < CONSTANT.ROW; i++)
        {
            for (int j = 0; j < CONSTANT.COL; j++)
            {
                GameObject inst;
                switch (initMap[i, j])
                {
                    case CONSTANT.ObjEnum.Dwall:
                        inst = Instantiate(dwallPrefabs[Random.Range(0,dwallPrefabs.Length)]);
                        inst.transform.position = Util.Map2World(new Vector2(i, j));
                        objectMap[i, j] = inst;
                        dwallLeft++;
                        break;
                    case CONSTANT.ObjEnum.Udwall: 
                        inst = Instantiate(udwallPrefabs[Random.Range(0,udwallPrefabs.Length)]);
                        inst.transform.position = Util.Map2World(new Vector2(i, j));
                        objectMap[i, j] = inst;
                        break;
                    case CONSTANT.ObjEnum.Player: 
                        inst = Instantiate(playerPrefab);
                        inst.transform.position = Util.Map2World(new Vector2(i, j));
                        player = inst;
                        break;
                    case CONSTANT.ObjEnum.Npc: 
                        inst = Instantiate(npcPrefabs[Random.Range(0,npcPrefabs.Length)]);
                        inst.transform.position = Util.Map2World(new Vector2(i, j));
                        npc.Add(inst);
                        break;
                    default: break;

                }
            }
        }
        if (player == null)
        {
            Debug.Log("Map contains no player!");
            player = Instantiate(playerPrefab);
            if (runtimeMap[0, 0] != CONSTANT.ObjEnum.Empty)
            {
                player.transform.position = Util.Map2World(new Vector2(0,0));
            }
            else
            {
                runtimeMap[0, 0] = CONSTANT.ObjEnum.Empty;
                player.transform.position = Util.Map2World(new Vector2(0, 0));
                if (objectMap[0, 0] != null) Destroy(objectMap[0, 0]);
            }
        }
    }
}
