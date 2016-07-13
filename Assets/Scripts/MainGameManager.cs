using UnityEngine;
using System.Collections;

public class MainGameManager : MonoBehaviour
{
    //scene
<<<<<<< HEAD
    private const string CHOOSE_SCENE_NAME = "ChooseScene";
    private const string CREATE_SCENE_NAME = "EditScene";
    private const string START_SCENE_NAME = "StartScene";
    private const string GAME_SCENE_NAME = "GameScene";
=======
    private const string CHOOSE_SCENE_NAME = "chooseScene";
    private const string CREATE_SCENE_NAME = "createScene";
    private const string START_SCENE_NAME = "startScene";
    private const string GAME_SCENE_NAME = "Demo";
>>>>>>> parent of 3152d9f... Add map editor and UI

    //playerPrefs
    //CurrentLevelNumber,SumLevelNumber,

    public static int levelSum = 0;
    public static int currentLevelNumber = 0;
    public static string currentMapFile;
    public static string gameMode = "Normal";
    
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
