using UnityEngine;
using System.Collections;

public class UIManager : MonoBehaviour {
    public GameObject UIplayerHP;
    public GameObject UINPCHP;
    public GameObject UIbombLeft;
    public GameObject UIflameDist;
    public GameObject UIPause;

    bool paused=false;

    public GameObject player;
	// Use this for initialization
	void Start () {
        player = GetComponent<GameManager>().GetPlayer();
        UIEventListener.Get(UIPause).onClick = Pause;
	}
	
	// Update is called once per frame
	void Update () {
        if(player!=null)
            UIplayerHP.GetComponent<UISlider>().value = player.GetComponent<Player>().GetHP()/100f;
        else player = GetComponent<GameManager>().GetPlayer();
	}
    public void Pause(GameObject button)
    {
        if (!paused)
        {
            Time.timeScale = 0;
            paused = true;
            UIPause.GetComponentInChildren<UILabel>().text = "Play";
        }
        else
        {
            Time.timeScale = 1;
            paused = false;
            UIPause.GetComponentInChildren<UILabel>().text = "Pause";
        }
        
    }
}
