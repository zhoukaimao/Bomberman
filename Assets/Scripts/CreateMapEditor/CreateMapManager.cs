using UnityEngine;

public class CreateMapManager : MonoBehaviour {

    /// <summary>
    /// 1.Map is 26*13
    /// 2. 3model,red-wall,green-wall_destroy,yellow-bomber_man
    /// </summary>
    const int mapWeigth = 26, mapHeigth = 13;
    private Color[] myMapCellColor = new Color[3] { Color.red, Color.green, Color.yellow };
    public enum MapCellChooseModel
    {
        Wall,Wall_destroy,Bomber_man
    }
    //array
    private int[,] myMap;
    private GameObject[,] myMapCell;
    public GameObject MapCelllPre;
    private GameObject MapCellPre_temp;
    public GameObject MapCellFATHER;

    private MapCellChooseModel myCurrentMapCellModel = MapCellChooseModel.Wall;
    private MapCell.MapCellPointArray setPoint_temp;
    // Use this for initialization
	void Start () {
        myMap = new int[mapHeigth, mapWeigth];
        myMapCell = new GameObject[mapHeigth, mapWeigth];
        for (int i = 0; i < mapHeigth; i++)
        {
            for (int j=0; j< mapWeigth; j++)
            {
                myMap[i, j] = 0;
                MapCellPre_temp = Instantiate(MapCelllPre, new Vector3(j / 9f, i / 9f, 0), transform.rotation) as GameObject;
                MapCellPre_temp.transform.parent = MapCellFATHER.transform;
                MapCellPre_temp.transform.localScale = new Vector3(1, 1, 1);
                myMapCell[i, j] = MapCellPre_temp;
                setPoint_temp.x = i;
                setPoint_temp.y = j;
                MapCellPre_temp.GetComponent<MapCell>().SendMessage("SetArrayPoint", setPoint_temp);
            }
        }
        MapCellFATHER.transform.position = new Vector3(-600 / 360f, -240 / 360f, 0f);
        ChangeColor();
	}
	
	// Update is called once per frame
	void Update () {
        ChangeColor();
	}
    private void ChangeColor()
    {
         for(int i = 0; i < mapHeigth; i++)
        {
            for(int j = 0; j < mapWeigth; j++)
            {
                myMapCell[i, j].GetComponent<UISprite>().color = myMapCellColor[myMap[i, j]];
            }
        }
    }

    //sendmessage
    private void pressed_MapCell(MapCell.MapCellPointArray pressedPoint)
    {
        myMap[pressedPoint.x, pressedPoint.y] = (int)myCurrentMapCellModel;
        ChangeColor();
    }
    private void pressed_MapCellModel(MapCellChooseModel myModel)
    {
        myCurrentMapCellModel = myModel;
    }
    private void OUTput()
    {
        //test
        for(int i = 0; i < mapHeigth; i++)
        {
            for(int j = 0; j < mapWeigth; j++)
            {
                Debug.Log(myMap[i, j]);
            }
        }
    }
}
