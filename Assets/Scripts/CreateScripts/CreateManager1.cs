using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class CreateManager1 : MonoBehaviour {

    public GameObject _udwallBtn;
    public GameObject _dwallBtn;
    public GameObject _playerBtn;
    public GameObject _npcBtn;
    public GameObject _finishBtn;
    public GameObject _backBtn;
    public GameObject _bonusRate;
    public GameObject _trapRate;

    public GameObject udwall;
    public GameObject dwall;
    public GameObject player;
    public GameObject npc;
    public GameObject mapGridMark;

    private CONSTANT.ObjEnum[,] map;
    private GameObject[,] objectMap;

    private CONSTANT.ObjEnum currentChoosed;
    private int playerCount = 0;
    private bool showWindow=false;
	// Use this for initialization
	void Start () {
        mapGridMark = Instantiate(mapGridMark);
        map = new CONSTANT.ObjEnum[CONSTANT.ROW,CONSTANT.COL];
        objectMap = new GameObject[CONSTANT.ROW, CONSTANT.COL];
        map.Initialize();
        UIEventListener.Get(_finishBtn).onClick = Finsh;
        UIEventListener.Get(_backBtn).onClick = Back;
        UIEventListener.Get(_udwallBtn).onClick = ChooseUdwall;
        UIEventListener.Get(_dwallBtn).onClick = ChooseDwall;
        UIEventListener.Get(_playerBtn).onClick = ChoosePlayer;
        UIEventListener.Get(_npcBtn).onClick = ChooseNPC;
        
	}
	// Update is called once per frame
	void Update () {
        _finishBtn.GetComponent<UIButton>().isEnabled = playerCount > 0;
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        Vector3 hitPoint = ray.GetPoint(-ray.origin.y / ray.direction.y);
        Vector2 mapPos = Util.World2Map(hitPoint);
        if (Util.ValidMapIndex(mapPos.x, mapPos.y))
        {
            mapGridMark.transform.position = Util.Map2World(mapPos)+new Vector3(0,0.05f,0);
            if(Input.GetMouseButtonDown(0))
                PlaceObject(mapPos);
        }
	}
    void OnGUI()
    {
        if (showWindow)
        {
            GUI.Window(0, new Rect(110, 10, 200, 60), DoWindow, "Basic Window");
        }
    }
    void PlaceObject(Vector2 mapPos)
    {
        GameObject obj;
        int indexX = (int)mapPos.x;
        int indexY = (int)mapPos.y;
        switch (currentChoosed)
        {
            case CONSTANT.ObjEnum.Udwall: 
                obj = Instantiate(udwall);
                break;
            case CONSTANT.ObjEnum.Dwall: 
                obj = Instantiate(dwall);
                break;
            case CONSTANT.ObjEnum.Player:
                if (playerCount > 0)
                {
                    showWindow = true;
                    return;
                }
                obj = Instantiate(player);
                playerCount++;
                break;
            case CONSTANT.ObjEnum.Npc:
                obj = Instantiate(npc);
                break;
            default: 
                obj = null; 
                break;

        }
        
        if (obj != null)
        {
            obj.transform.position = Util.Map2World(mapPos);
            if (objectMap[indexX, indexY] != null)
            {
                Destroy(objectMap[indexX, indexY]);
                if (map[indexX, indexY] == CONSTANT.ObjEnum.Player) playerCount--;
            }
            map[indexX, indexY] = currentChoosed;
            objectMap[indexX, indexY] = obj;
        }
    }
    void Finsh(GameObject button)
    {
        double bonusRate = _bonusRate.GetComponent<UISlider>().value;
        double trapRate = _trapRate.GetComponent<UISlider>().value;
        if (playerCount <= 0)
        {
            showWindow = true;
        }
        MapParam mapData = new MapParam(map,bonusRate,trapRate);
        new DataStoreProcessor().Save(mapData,CONSTANT.SaveDataPath+@"Levels/mapData.map",true);
        SceneManager.LoadScene(MainGameManager.getStartSceneName());
    }
    void DoWindow(int windowId)
    {
        GUI.Label(new Rect(10,10,200,50),"Too many players!");
    }
    void Back(GameObject button)
    {
        SceneManager.LoadScene(MainGameManager.getStartSceneName());
    }
    public void ChooseUdwall(GameObject button)
    {
        currentChoosed = CONSTANT.ObjEnum.Udwall;
    }
    public void ChooseDwall(GameObject button)
    {
        currentChoosed = CONSTANT.ObjEnum.Dwall;
    }
    public void ChoosePlayer(GameObject button)
    {
        currentChoosed = CONSTANT.ObjEnum.Player;
    }
    public void ChooseNPC(GameObject button)
    {
        currentChoosed = CONSTANT.ObjEnum.Npc;
    }
}
