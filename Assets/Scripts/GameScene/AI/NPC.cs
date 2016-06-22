using UnityEngine;
using System.Collections;
/// <summary>
/// store npc information
/// npc action method
/// </summary>
public class NPC : MonoBehaviour
{
    public CONSTANT.NPCEnum npcType;
    GameObject _gameManager;
    // Use this for initialization
    void Start()
    {
        _gameManager = GameObject.Find("GameManager");
    }

    // Update is called once per frame
    void Update()
    {

    }
    
}
