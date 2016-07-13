using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;
using System.IO;


public class createObjectByMouse : MonoBehaviour {
    public GameObject[] Objects;
    private int ObjectsCount;
    private GameObject m_CreateObject;

    public GameObject PARENT_LEVEL;

    const int LENGTH_NUM = 20, WIDTH_NUM = 16;//必须是偶数

    Camera m_cam;
    Vector3 mMousePos;
    Ray mRayCam;
    RaycastHit mHit;

    public GameObject Controller;
    private GameObject m_cameraMoveController;
    private float m_Horizontal, m_Vertical;
    public float CameraMoveSpeed;
    public float MaxAngleX, MinAngleX;

    public enum MapCell
    {
        Null,
        Wall_Normal,Wall_Destroy, Player,
        Enemy_Normal,Enemy_Eat,Enemy_Throw
    }
    private MapCell[,] m_Map;
    private MapCell currentMapCellKind = (MapCell)1;

    // Use this for initialization
    void Start () {
        ObjectsCount = Objects.Length;
        m_CreateObject = Objects[0];

        m_cameraMoveController = Controller;
        m_cameraMoveController.transform.parent = null;
        m_cameraMoveController.transform.position = new Vector3(LENGTH_NUM / 2 - 0.5f, 0, WIDTH_NUM / 2 - 0.5f);
        m_Horizontal = 0;
        m_Vertical = 0;

        m_cam = gameObject.GetComponent<Camera>();
        gameObject.transform.parent = null;
        m_cam.transform.position = new Vector3(LENGTH_NUM / 2 - 0.5f, 5, -10);
        m_cam.transform.LookAt(m_cameraMoveController.transform);
        CreateDown();
        
        gameObject.transform.parent = m_cameraMoveController.transform;
    }

    // Update is called once per frame
    void Update () {
        if (Input.GetMouseButtonDown(0))
        {
            leftClickMouseCreate();
            //Debug.Log("左键");
        }
        if (Input.GetMouseButtonDown(1))
        {
            rightClickMouseCreate();
            //Debug.Log("右键");
        }

        m_Horizontal = Input.GetAxis("Horizontal");
        m_Vertical = Input.GetAxis("Vertical");
        if (m_Horizontal != 0 || m_Vertical != 0)
        {
            cameraMoveController(m_Horizontal, m_Vertical);
        }
    }

    void CreateDown()
    {
        m_Map = new MapCell[LENGTH_NUM, WIDTH_NUM];
        for(int x = 0; x < LENGTH_NUM; x++)
        {
            for(int z = 0; z < WIDTH_NUM; z++)
            {
                GameObject tempChild = Instantiate(m_CreateObject, new Vector3(x, 0, z), Quaternion.Euler(Vector3.zero)) as GameObject;
                tempChild.transform.parent = PARENT_LEVEL.transform;

                m_Map[x, z] = (MapCell)0;
            }
        }
    }

    void leftClickMouseCreate()
    {
        mMousePos = Input.mousePosition;
        mRayCam = m_cam.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(mRayCam, out mHit))
        {
            if (mHit.collider.gameObject != null)
            {
                if (mHit.collider.gameObject.GetComponent<createdObject>().get_canCreate() && mHit.collider.transform.position.y == 0)
                {
                    m_CreateObject = Objects[(int)currentMapCellKind - 1];

                    GameObject tempFather = mHit.collider.gameObject;
                    GameObject tempChild = Instantiate(m_CreateObject, tempFather.transform.position + new Vector3(0, 1, 0), tempFather.transform.rotation) as GameObject;

                    m_Map[(int)tempChild.transform.position.x, (int)tempChild.transform.position.z] = currentMapCellKind;

                    tempChild.transform.parent = tempFather.transform;
                    if (tempFather.GetComponent<createdObject>())
                    {
                        tempFather.GetComponent<createdObject>().SendMessage("set_canCreate", false);
                    }
                }
            }
        }
    }
    void rightClickMouseCreate()
    {
        mMousePos = Input.mousePosition;
        mRayCam = m_cam.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(mRayCam, out mHit))
        {
            if (mHit.collider.gameObject != null)
            {
                if (mHit.collider.gameObject.GetComponent<createdObject>().get_canCreate())
                {
                    GameObject tempChild = mHit.collider.gameObject;
                    GameObject tempFather = null;
                    if (tempChild.transform.parent.gameObject != PARENT_LEVEL && tempChild.transform.childCount == 0)
                    {
                        tempFather = tempChild.transform.parent.gameObject;

                        m_Map[(int)tempChild.transform.position.x, (int)tempChild.transform.position.z] = (MapCell)0;

                        Destroy(tempChild);
                        tempFather.GetComponent<createdObject>().SendMessage("set_canCreate", true);
                    }
                }
            }
        }
    }

    //控制摄像机移动
    void cameraMoveController(float Hor,float Ver)
    {
        Vector3 myRotation = m_cameraMoveController.transform.rotation.eulerAngles;

        if (myRotation.x >= MinAngleX && myRotation.x <= MaxAngleX)
        {
            myRotation += new Vector3(m_Vertical * Time.deltaTime * CameraMoveSpeed, -m_Horizontal * Time.deltaTime * CameraMoveSpeed, 0);
            m_cameraMoveController.transform.rotation = Quaternion.Euler(myRotation);
        }else if (myRotation.x < MinAngleX)
        {
            m_cameraMoveController.transform.rotation = Quaternion.Euler(new Vector3(MinAngleX + 0.1f, myRotation.y, myRotation.z));
        }
        else if (myRotation.x > MaxAngleX)
        {
            m_cameraMoveController.transform.rotation = Quaternion.Euler(new Vector3(MaxAngleX - 0.1f, myRotation.y, myRotation.z));
        }

        m_cam.transform.LookAt(m_cameraMoveController.transform);
    }

    //sendmessage
    void CreateNewLevelFinished()
    {
        string mapString = LENGTH_NUM.ToString() + ":" + WIDTH_NUM.ToString() + "=";

        for(int i = 0; i < LENGTH_NUM; i++)
        {
            for(int j = 0; j < WIDTH_NUM; j++)
            {
                mapString += "{" + i.ToString() + "," + j.ToString() + "," + ((int)m_Map[i, j]).ToString() + "}";
            }
        }
        
        //写入文件

        WriteFile("test.map", mapString);

        SceneManager.LoadScene(MainGameManager.getStartSceneName());
    }

    void setCurrentMapCellKind(int cell)
    {
        currentMapCellKind = (MapCell)cell;
    }

    void WriteFile(string fileName,string fileContect)
    {
        string FilePath = Application.dataPath;
        fileName = FilePath + (char)92 + fileName;
        if (!File.Exists(fileName))
        {
            File.CreateText(fileName);
        }
        string[] str = new string[1] { fileContect };
        File.WriteAllLines(fileName, str);
    }
}
