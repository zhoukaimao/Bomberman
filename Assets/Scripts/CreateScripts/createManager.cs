using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;
public class createManager : MonoBehaviour {

    public GameObject Creater;

    public GameObject[] buttons;
	// Use this for initialization
	void Start () {
        chooseWall_Normal();
	}
	
	// Update is called once per frame
	void Update () {
	
	}
    //button controller
    public void startScene()
    {
        if (Creater.GetComponent<createObjectByMouse>())
        {
            //完成
            Creater.GetComponent<createObjectByMouse>().SendMessage("CreateNewLevelFinished");
        }
    }
    public void backScene()
    {
        SceneManager.LoadScene("startScene");
    }

    public void choose_Player()
    {
        if (Creater.GetComponent<createObjectByMouse>())
        {
            Creater.GetComponent<createObjectByMouse>().SendMessage("setCurrentMapCellKind", 3);
        }
        for(int i = 0; i < buttons.Length; i++)
        {
            if (i == 2 && buttons[2].GetComponent<TweenScale>())
            {
                buttons[2].GetComponent<TweenScale>().PlayForward();
            }else
            {
                buttons[i].GetComponent<TweenScale>().PlayReverse();
            }
        }
    }
    public void chooseWall_Normal()
    {
        if (Creater.GetComponent<createObjectByMouse>())
        {
            Creater.GetComponent<createObjectByMouse>().SendMessage("setCurrentMapCellKind", 1);
        }
        for (int i = 0; i < buttons.Length; i++)
        {
            if (i == 0 && buttons[0].GetComponent<TweenScale>())
            {
                buttons[0].GetComponent<TweenScale>().PlayForward();
            }
            else
            {
                buttons[i].GetComponent<TweenScale>().PlayReverse();
            }
        }
    }
    public void chooseWall_Destroy()
    {
        if (Creater.GetComponent<createObjectByMouse>())
        {
            Creater.GetComponent<createObjectByMouse>().SendMessage("setCurrentMapCellKind", 2);
        }
        for (int i = 0; i < buttons.Length; i++)
        {
            if (i == 1 && buttons[1].GetComponent<TweenScale>())
            {
                buttons[1].GetComponent<TweenScale>().PlayForward();
            }
            else
            {
                buttons[i].GetComponent<TweenScale>().PlayReverse();
            }
        }
    }
    public void chooseEnemy_Normal()
    {
        if (Creater.GetComponent<createObjectByMouse>())
        {
            Creater.GetComponent<createObjectByMouse>().SendMessage("setCurrentMapCellKind", 4);
        }
        for (int i = 0; i < buttons.Length; i++)
        {
            if (i == 3 && buttons[3].GetComponent<TweenScale>())
            {
                buttons[3].GetComponent<TweenScale>().PlayForward();
            }
            else
            {
                buttons[i].GetComponent<TweenScale>().PlayReverse();
            }
        }
    }
    public void chooseEnemy_Eat()
    {
        if (Creater.GetComponent<createObjectByMouse>())
        {
            Creater.GetComponent<createObjectByMouse>().SendMessage("setCurrentMapCellKind", 5);
        }
        for (int i = 0; i < buttons.Length; i++)
        {
            if (i == 4 && buttons[4].GetComponent<TweenScale>())
            {
                buttons[4].GetComponent<TweenScale>().PlayForward();
            }
            else
            {
                buttons[i].GetComponent<TweenScale>().PlayReverse();
            }
        }
    }
    public void chooseEnemy_Throw()
    {
        if (Creater.GetComponent<createObjectByMouse>())
        {
            Creater.GetComponent<createObjectByMouse>().SendMessage("setCurrentMapCellKind", 6);
        }
        for (int i = 0; i < buttons.Length; i++)
        {
            if (i == 5 && buttons[5].GetComponent<TweenScale>())
            {
                buttons[5].GetComponent<TweenScale>().PlayForward();
            }
            else
            {
                buttons[i].GetComponent<TweenScale>().PlayReverse();
            }
        }
    }
}
