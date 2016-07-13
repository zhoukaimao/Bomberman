using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;
using System.IO;
using System.Collections.Generic;
public class chooseManager : MonoBehaviour
{

    public UILabel currentLevelLabel;
    public UIPopupList selectMap;
    public UIPopupList selectMode;
    private int levelSum = 0;
    private int currentLevelInt = 0;
    private string[] files;
    private string gameMode = "Normal";
    private string mapFolder = "Levels";
    // Use this for initialization
    void Start()
    {
        //levelSum = MainGameManager.levelSum;
        DirectoryInfo folder = new DirectoryInfo(CONSTANT.SaveDataPath + mapFolder);
        
        List<string> filenames = new List<string>();
        foreach (FileInfo file in folder.GetFiles("*.map"))
        {
            filenames.Add(file.Name);
        }
        levelSum = filenames.Count;
        files = filenames.ToArray();
        if (levelSum <= 0)
        {
            return;
        }
        currentLevelLabel.text = files[currentLevelInt];

    }

    // Update is called once per frame
    void Update()
    {

    }

    public void backScene()
    {
        SceneManager.LoadScene(MainGameManager.getStartSceneName());
    }
    public void goScene()
    {
        if (levelSum <= 0)
        {
            return;
        }
        MainGameManager.currentMapFile = CONSTANT.SaveDataPath+mapFolder+@"/"+files[currentLevelInt%levelSum];
        SceneManager.LoadScene(MainGameManager.getGameSceneName());
    }
    public void addLevel()
    {
        if (levelSum <= 0) return;
        currentLevelInt++;
        currentLevelLabel.text = files[currentLevelInt%levelSum];
    }
    public void reduceLevel()
    {
        if (levelSum <= 0) return;
        currentLevelInt--;
        currentLevelLabel.text = files[currentLevelInt%levelSum];
    }
    public void SelectMode()
    {
        MainGameManager.gameMode = selectMode.value;
    }
    public void SelectMap()
    {
        mapFolder = selectMap.value;
        DirectoryInfo folder = new DirectoryInfo(CONSTANT.SaveDataPath + @mapFolder);

        List<string> filenames = new List<string>();
        foreach (FileInfo file in folder.GetFiles("*.map"))
        {
            filenames.Add(file.Name);
        }
        levelSum = filenames.Count;
        files = filenames.ToArray();
        if (levelSum <= 0) return;
        currentLevelLabel.text = files[currentLevelInt];
    }
}
