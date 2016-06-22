using UnityEngine;
using System.Collections;

public class Player : MonoBehaviour {
    GameManager _gameManager;
    float HP;
	// Use this for initialization
	void Start () {
        _gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
	}
	
	// Update is called once per frame
	void Update () {
        HP = GetComponent<Bomberman>().HP;
        if (HP < 0) _gameManager.GameOver();
	}
    public float GetHP()
    {
        return HP;
    }
}
