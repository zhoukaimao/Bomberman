using UnityEngine;
using System.Collections;

public class MainGameManager : MonoBehaviour
{
    //scene
    private const string CHOOSE_SCENE_NAME = "chooseScene";
    private const string CREATE_SCENE_NAME = "createScene";
    private const string START_SCENE_NAME = "startScene";
    private const string GAME_SCENE_NAME = "Demo";

    //playerPrefs
    //CurrentLevelNumber,SumLevelNumber,

    static public int MY_LEVEL_NUMBER = PlayerPrefs.GetInt("LevelNumber");
    static public string CurrentLevelName = null;
    
    static public string getChooseSceneName()
    {
        return CHOOSE_SCENE_NAME;
    }
    static public string getCreateSceneName()
    {
        return CREATE_SCENE_NAME;
    }
    static public string getStartSceneName()
    {
        return START_SCENE_NAME;
    }
    static public string getGameSceneName()
    {
        return GAME_SCENE_NAME;
    }
}
