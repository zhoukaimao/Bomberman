using UnityEngine;
using System.Collections;

public class Bomberman : MonoBehaviour {
    private float _hp=10.0f;
    private int _bombLeft=1;
    private float _flameDist=1f;
    private GameManager _gameManager;
    private float _walkSpeed=1.5f;
    public AudioClip HurtAudio;
    //public AudioClip RunAudio;
    private AudioSource _audioSource;
    private Animation _animation;
    private AudioManager _audioManager;
    public float walkSpeed
    {
        get
        {
            return _walkSpeed;
        }
        set
        {
            _walkSpeed=value;
        }
    }
    public float HP
    {
        get
        {
            return _hp;
        }
        set
        {
            _hp = value;
        }
    }
    public int bombLeft
    {
        get
        {
            return _bombLeft;
        }
        set
        {
            _bombLeft = value;
        }
    }
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
	// Use this for initialization
	void Start () {
        _gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
        _audioSource = GetComponent<AudioSource>();
        _animation = GetComponent<Animation>();
        _audioManager = _gameManager.GetComponent<AudioManager>();
	}
	
	// Update is called once per frame
	void Update () {
        //if (HP <= 0) Destroy(gameObject);
	}
    public void AddBonus(CONSTANT.BonusEnum bonus)
    {
        switch (bonus)
        {
            case CONSTANT.BonusEnum.AddBomb: bombLeft++; break;
            case CONSTANT.BonusEnum.AddFlame: flameDist++; break;
            case CONSTANT.BonusEnum.AddSpeed: walkSpeed++; break;
            case CONSTANT.BonusEnum.AddHP: HP++; break;
            default: break;
        }
    }
    public void GetTrapped(CONSTANT.TrapEnum trap)
    {
        switch (trap)
        {
            case CONSTANT.TrapEnum.Hurt: Hurt(); break;
            default: break;
        }
    }
    public void DecreaseBombLeft()
    {
        bombLeft--;
    }
    public void IncreaseBombLeft()
    {
        bombLeft++;
    }
    public void Hurt()
    {
        HP--;
        _audioManager.PlayHurtAudio();
        //_audioSource.PlayOneShot(HurtAudio);
        _animation.Play(AnimationPlayMode.Mix);//fail to play animation
    }
    public void PlaceBomb()
    {
        if (bombLeft > 0)
        {
            _gameManager.PlaceBomb(gameObject);
        }
            
    }

}
