using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;
public class chooseManager : MonoBehaviour {
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    public void backScene()
    {
        SceneManager.LoadScene(MainGameManager.getStartSceneName());
    }
    public void goScene()
    {
        SceneManager.LoadScene(MainGameManager.getGameSceneName());
    }
}
