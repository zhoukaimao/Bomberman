using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;
public class startManager : MonoBehaviour {
    public UIToggle SFX;
    public UISlider soundVolume;
	// Use this for initialization
	void Start () {
        PlayerPrefs.SetInt("SFX",0);
        PlayerPrefs.SetFloat("SoundVolume",0.3f);
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
    public void ChangeSFX()
    {
        if (SFX.value)
        {
            PlayerPrefs.SetInt("SFX", 1);
        }
        else
        {
            PlayerPrefs.SetInt("SFX", 0);
        }
    }
    public void ChangeSoundVolume()
    {
        PlayerPrefs.SetFloat("SoundVolume",soundVolume.value);
    }
}
