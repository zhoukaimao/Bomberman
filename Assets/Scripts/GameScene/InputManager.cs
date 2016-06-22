using UnityEngine;
using System.Collections;

public class InputManager : MonoBehaviour {
    GameObject player;
	// Use this for initialization
	void Start () {
        //player = GetComponent<GameManager>().GetPlayer();
	}
	
	// Update is called once per frame
	void Update () {
        player = GetComponent<GameManager>().GetPlayer();
        Vector3 moveDir = new Vector3(Input.GetAxis("Vertical"), 0, -Input.GetAxis("Horizontal"));
        if (player!=null)
        {
            player.GetComponent<BombermanMove>().Move(moveDir);
            if (Input.GetKeyDown(KeyCode.Space))
            {
                player.GetComponent<Bomberman>().PlaceBomb();
            }
        }

	}
}
