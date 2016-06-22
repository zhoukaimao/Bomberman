using UnityEngine;

public class ButtonManager : MonoBehaviour {
    public TweenScale[] ButtonScale;

    private int currentChoose = -1;
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
    //button Manager
    public void chooseWall()
    {
        if (currentChoose != 0)
        {
            if (currentChoose != -1)
                ButtonScale[currentChoose].PlayReverse();
            currentChoose = 0;
            ButtonScale[currentChoose].PlayForward();
            GetComponent<CreateMapManager>().SendMessage("pressed_MapCellModel",(CreateMapManager.MapCellChooseModel)currentChoose);
        }
    }
    public void chooseWall_destroy()
    {
        if (currentChoose != 1)
        {
            if (currentChoose != -1)
                ButtonScale[currentChoose].PlayReverse();
            currentChoose = 1;
            ButtonScale[currentChoose].PlayForward();
            GetComponent<CreateMapManager>().SendMessage("pressed_MapCellModel", (CreateMapManager.MapCellChooseModel)currentChoose);
        }
    }
    public void chooseBomber_man()
    {
        if (currentChoose != 2)
        {
            if (currentChoose != -1)
                ButtonScale[currentChoose].PlayReverse();
            currentChoose = 2;
            ButtonScale[currentChoose].PlayForward();
            GetComponent<CreateMapManager>().SendMessage("pressed_MapCellModel", (CreateMapManager.MapCellChooseModel)currentChoose);
        }
    }

    public void CreateMap()
    {
        GetComponent<CreateMapManager>().SendMessage("OUTput");
    }
}
