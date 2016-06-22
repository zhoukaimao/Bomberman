using UnityEngine;
using System.Collections;
using System.Collections.Generic;
public class GameManager : MonoBehaviour {
    AudioManager _audioManager;
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
    private CONSTANT.ObjEnum[,] initMap;
    private CONSTANT.ObjEnum[,] runtimeMap;
    private GameObject[,] objectMap;
    private int dwallLeft = 0;

    /// <summary>
    /// Player and NPC
    /// </summary>
    private GameObject player;
    private List<GameObject> npc;

    private float bombLifeTime = 3000f;
    private float flameLifeTime = 1000f;
    [Range(0,1)]
    public float bonusRate;
    [Range(0, 1)]
    public float trapRate;

	// Use this for initialization
	void Start () {
        LoadMap();
        CreateScene();
        _audioManager = GetComponent<AudioManager>();
	}
	
	// Update is called once per frame
	void Update () {

	}
    public CONSTANT.ObjEnum[,] GetMap()
    {
        return runtimeMap;
    }
    public GameObject GetPlayer()
    {
        return player;
    }
    public void DestroyNPC(GameObject Deadnpc)
    {
        npc.Remove(Deadnpc);
        Destroy(Deadnpc);

    }
    public void GameOver()
    {
        Time.timeScale = 0;
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
        }
        _audioManager.PlayPlaceBombAudio();
    }
    public int GetDwallLeft()
    {
        return dwallLeft;
    }
    public int GetNPCLeft()
    {
        return npc.Count;
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
    }
    /// <summary>
    /// according to map loaded, create game scene, and initialize variables
    /// </summary>
    void CreateScene()
    {
        npc = new List<GameObject>();
        runtimeMap = initMap;
        objectMap = new GameObject[CONSTANT.ROW, CONSTANT.COL];
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
