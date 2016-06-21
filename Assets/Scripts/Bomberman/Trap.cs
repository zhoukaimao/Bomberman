using UnityEngine;
using System.Collections;

public class Trap : MonoBehaviour {
    GameManager _gameManager;
    public CONSTANT.TrapEnum trapType;
	// Use this for initialization
	void Start () {
        _gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
	}
	
	// Update is called once per frame
	void Update () {
	
	}
    void OnTriggerEnter(Collider collider)
    {
        if (collider.gameObject != null)
        {
            if (collider.GetComponent<Bomberman>())
            {
                collider.GetComponent<Bomberman>().GetTrapped(trapType);
            }
        }
    }
}
