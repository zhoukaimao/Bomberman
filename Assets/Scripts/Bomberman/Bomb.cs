using UnityEngine;
using System.Collections;

public class Bomb : MonoBehaviour {
    private GameObject _owner;
    private float _flameDist;
    private float _lifeTime;
    private float leftTime;
    private GameManager _gameManager;
    public float flameDist
    {
        get
        {
            return _flameDist;
        }
        set
        {
            _flameDist = value;
        }
    }
    public float lifeTime
    {
        get
        {
            return _lifeTime;
        }
        set
        {
            _lifeTime = value;
        }
    }
    public GameObject owner
    {
        get
        {
            return _owner;
        }
        set
        {
            _owner = value;
        }
    }
	// Use this for initialization
	void Start () {
        leftTime = _lifeTime;
        _gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
	}
	// Update is called once per frame
	void Update () {
        leftTime -= Time.deltaTime*1000;
        if (leftTime <= 0)
        {
            Blast();
        }
	}
    /// <summary>
    /// when bomberman leave the bomb grid, open collider
    /// Defaultly, the collider isTrigger = true
    /// </summary>
    /// <param name="bomberman"></param>
    void OnTriggerExit(Collider bomberman)
    {
        //should detect if there any bomberman in the grid
        GetComponent<Collider>().isTrigger = false;
    }
    public void Blast()
    {
        _gameManager.BombBlast(gameObject);
    }
}
