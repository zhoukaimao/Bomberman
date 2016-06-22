using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;
public class startManager : MonoBehaviour {
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    public void StartScene()
    {
        SceneManager.LoadScene(MainGameManager.getChooseSceneName());
    }

    public void CreateScene()
    {
        SceneManager.LoadScene(MainGameManager.getCreateSceneName());
    }

    public void Exit()
    {
        Application.Quit();
    }
}
