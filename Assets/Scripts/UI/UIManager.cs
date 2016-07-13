using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class UIManager : MonoBehaviour {
    public GameObject UIplayerHP;
    public GameObject UIbombLeft;
    public GameObject UIflameDist;
    public GameObject UIPause;
    public UILabel UITimer;
    public GameObject gamePauseLayer;
    public GameObject gameWinLayer;
    public GameObject gameLoseLayer;

    bool paused=true;
    bool initComp = false;
    float bombLeft = 0;
    float flameDist = 0;
    public GameObject player;
    public GameObject npcHpPrefab;
	// Use this for initialization
	void Start () {
        player = GetComponent<GameManager>().GetPlayer();
        UIEventListener.Get(UIPause).onClick = Pause;

	}
	
	// Update is called once per frame
	void Update () {
        if (!initComp)
        {
            var npcs = GetComponent<GameManager>().GetNPC();
            foreach (GameObject npc in npcs)
            {
                AttachHPBar(npc);
            }
            initComp = true;
        }
        
        if (player != null)
        {
            UIplayerHP.GetComponent<UISlider>().value = player.GetComponent<Player>().GetHP() / player.GetComponent<Player>().GetMaxHP();
            UIplayerHP.GetComponentInChildren<UILabel>().text = player.GetComponent<Player>().GetHP().ToString();
            UIflameDist.GetComponent<UILabel>().text = "x "+player.GetComponent<Bomberman>().flameDist.ToString();
            UIbombLeft.GetComponent<UILabel>().text = "x "+player.GetComponent<Bomberman>().bombLeft.ToString();
            
        }
            
        else
        {
            UIplayerHP.GetComponent<UISlider>().value = 0;
            UIplayerHP.GetComponentInChildren<UILabel>().text = "0";
            player = GetComponent<GameManager>().GetPlayer();
        }
        var timer = GetComponent<GameManager>().GetTimer();
        var minute = (int)timer / 60000;
        var second = (int)(timer / 1000)%60;
        var timeFormat = minute + ":" + second;
        UITimer.text = timeFormat;
	}
    public void Pause(GameObject button)
    {
        if (!paused)
        {
            Time.timeScale = 0;
            paused = true;
            UIPause.GetComponentInChildren<UILabel>().text = "Start";
        }
        else
        {
            Time.timeScale = 1;
            paused = false;
            UIPause.GetComponentInChildren<UILabel>().text = "Pause";
        }
        
    }
    public void AttachHPBar(GameObject bomberman)
    {
        var hpbar = Instantiate(npcHpPrefab);
        hpbar.GetComponent<MyHPSlider>().owner = bomberman;
        NGUITools.AddChild<UILabel>(hpbar);
    }
    public void Win()
    {
        Pause();
        gameWinLayer.SetActive(true);
    }
    public void Lose()
    {
        Pause();
        gameLoseLayer.SetActive(true);
    }
    public void Pause()
    {
        Time.timeScale = 0;
        gamePauseLayer.SetActive(true);
    }
    public void PlayAgain()
    {
        Time.timeScale = 1;
        SceneManager.LoadScene(MainGameManager.getGameSceneName());
    }
    public void ReturnMenu()
    {
        Time.timeScale = 1;
        SceneManager.LoadScene(MainGameManager.getStartSceneName());
    }
}
