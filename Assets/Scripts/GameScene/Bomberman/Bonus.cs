using UnityEngine;
using System.Collections;

public class Bonus : MonoBehaviour {
    public CONSTANT.BonusEnum bonusType;
	// Use this for initialization
    private GameManager _gameManager;
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
            var _bomberman = collider.GetComponent<Bomberman>();
            if (_bomberman)
            {
                _bomberman.AddBonus(bonusType);
                _gameManager.PickupBonus(gameObject);
            }
        }
    }
}
