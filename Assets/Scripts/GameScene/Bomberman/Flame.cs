using UnityEngine;
using System.Collections;

public class Flame : MonoBehaviour {
    private float leftTime;
    private float _lifeTime;
    private bool _destroyWall;
    private GameObject _takePlaceBonus;
    private GameObject _takePlaceTrap;
    public GameObject takePlaceBonus
    {
        get
        {
            return _takePlaceBonus;
        }
        set
        {
            _takePlaceBonus = value;
        }
    }
    public GameObject takePlaceTrap
    {
        get
        {
            return _takePlaceTrap;
        }
        set
        {
            _takePlaceTrap = value;
        }
    }
    public bool destroyWall
    {
        get
        {
            return _destroyWall;
        }
        set
        {
            _destroyWall = value;
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
	// Use this for initialization
	void Start () {
        leftTime = _lifeTime;
        var particleSystem = GetComponentInChildren<ParticleSystem>();
        particleSystem.startSpeed = 1000 / _lifeTime;
	}
    void Awake()
    {
        leftTime = _lifeTime;
    }
	// Update is called once per frame
	void Update () {
        leftTime -= Time.deltaTime*1000;
        if (leftTime <= 0)
        {
            GameObject.Find("GameManager").GetComponent<GameManager>().FlameDestroy(gameObject);
        }
	}
    void OnTriggerEnter(Collider collider)
    {
        if (collider != null)
        {
            var _bomberman = collider.GetComponent<Bomberman>();
            if (_bomberman)
            {
                _bomberman.Hurt();
            }
        }
    }
}
