using UnityEngine;
using System.Collections;
using System.IO;

public class mapCreate : MonoBehaviour {
    //枚举型地图二维数组,可以直接通过 mapCreate.myMap 调用，MapCell 定义在 createObjectByMouse
    static public createObjectByMouse.MapCell[,] myMap;

    private string mMapString;
    private int LENGTH = 0, WIDTH = 0;

    public GameObject[] MapCells;
    public GameObject PARENT_OBJECT;
    // Use this for initialization
    void Start () {
        //文件读取 进mMapString
        //文件部分暂时还没搞定
        //mMapString = ReadMapString(MainGameManager.CurrentLevelName);
        mMapString = ReadMapString("test.map");
        //解析数组
        GetMap();

        //生成地图
        createTheMap();
	}
	
	// Update is called once per frame
	void Update () {

    }

    private string ReadMapString(string fileName)
    {
        string temp_mapString = null;
        //读入地图文件
        string FilePath = Application.dataPath;
        fileName = FilePath + (char)92 + fileName;

        string[] str = File.ReadAllLines(fileName);

        if (str.Length == 1)
        {
            temp_mapString = str[0];
        }else
        {
            Debug.Log("Eorror");
        }

        Debug.Log(temp_mapString);

        return temp_mapString;
    } 

    void GetMap()
    {
        string lengthAndwidth = null;
        bool getLandW = false;
        string m_mapString = null;
        for (int i = 0; i < mMapString.Length; i++)
        {
            if (mMapString[i] != '=' && !getLandW)
            {
                lengthAndwidth += mMapString[i];
            }
            else if (getLandW == false)
            {
                getLandW = true;
            }
            else if (getLandW)
            {
                m_mapString += mMapString[i];
            }
        }

        if (getLandW)
        {
            string length = null, width = null;
            bool get_length = false;
            int tempLENGTH, tempWIDTH;
            for (int i = 0; i < lengthAndwidth.Length; i++)
            {
                if (!get_length)
                {
                    if (lengthAndwidth[i] != ':')
                    {
                        length += lengthAndwidth[i];
                    }
                    else
                    {
                        get_length = true;
                    }
                }
                else
                {
                    width += lengthAndwidth[i];
                }
            }
            Debug.Log(length + "   " + width);
            tempLENGTH = int.Parse(length);
            tempWIDTH = int.Parse(width);

            LENGTH = tempLENGTH;
            WIDTH = tempWIDTH;

            myMap = new createObjectByMouse.MapCell[LENGTH, WIDTH];
            int currentTemp = -1;
            string[,] cells = new string[LENGTH, WIDTH];
            for (int i = 0; i < LENGTH; i++)
                for (int j = 0; j < WIDTH; j++)
                {
                    cells[i, j] = null;
                    myMap[i, j] = (createObjectByMouse.MapCell)0;
                }
            for (int i = 0; i < m_mapString.Length; i++)
            {
                if (m_mapString[i] == '{')
                {
                    currentTemp++;
                }
                cells[currentTemp / WIDTH, currentTemp % WIDTH] += m_mapString[i];
            }
            for (int i = 0; i < LENGTH; i++)
            {
                for (int j = 0; j < WIDTH; j++)
                {
                    string tempX = null, tempY = null, tempMapKind = null;
                    bool getX = false, getY = false;

                    for (int stringL = 0; stringL < cells[i, j].Length; stringL++)
                    {
                        if (cells[i, j][stringL] != '{' && cells[i, j][stringL] != '}')
                        {
                            if (!getX)
                            {
                                if (cells[i, j][stringL] != ',')
                                {
                                    tempX += cells[i, j][stringL];
                                }
                                else
                                {
                                    getX = true;
                                }
                            }
                            else if (!getY)
                            {
                                if (cells[i, j][stringL] != ',')
                                {
                                    tempY += cells[i, j][stringL];
                                }
                                else
                                {
                                    getY = true;
                                }
                            }
                            else
                            {
                                tempMapKind += cells[i, j][stringL];
                            }
                        }
                    }
                    int mx = int.Parse(tempX), my = int.Parse(tempY), mmk = int.Parse(tempMapKind);
                    myMap[mx, my] = (createObjectByMouse.MapCell)mmk;
                }
            }
        }
    }

    void createTheMap()
    {
        //生成底座
        for(int x=0;x<LENGTH;x++)
            for(int z = 0; z < WIDTH; z++)
            {
                GameObject temp = Instantiate(MapCells[0], new Vector3(x, 0, z), transform.rotation) as GameObject;
                temp.transform.parent = PARENT_OBJECT.transform;
            }

        //其他地图元素
        for(int i = 0; i < LENGTH; i++)
        {
            for (int j = 0; j < WIDTH; j++)
            {
                if ((int)myMap[i, j] > 0)
                {
                    GameObject temp = Instantiate(MapCells[(int)myMap[i, j] - 1], new Vector3(i, 1, j), transform.rotation) as GameObject;
                    temp.transform.parent = PARENT_OBJECT.transform;
                }
            }
        }
    }
}
