using UnityEngine;
using System.Collections;

public class Bomberman : MonoBehaviour {
    public GameObject ragdollPrefab;
    private float maxHP = 30f;
    private float _hp;
    private int _bombLeft=1;
    private float _flameDist=1f;
    private GameManager _gameManager;
    private float _walkSpeed=3f;
    private Animation _animation;
    private AudioManager _audioManager;
    private bool canTrigger = false;
    public bool CanTrigger { get { return canTrigger; } }
    private bool getDropBombTrapped = false;
    private float getDropBombTrapTimer = 0;
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
        HP = maxHP;
        _gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
        _animation = GetComponent<Animation>();
        _audioManager = _gameManager.GetComponent<AudioManager>();
	}
	
	// Update is called once per frame
	void Update () {
        //if (HP <= 0) Destroy(gameObject);
        if (HP <= 0)
        {
            _gameManager.DestroyBomberman(gameObject);
        }
        if (getDropBombTrapped)
        {
            getDropBombTrapTimer -= Time.deltaTime;
            if (getDropBombTrapTimer <= 0) getDropBombTrapped = false;
            PlaceBomb();
        }
	}
    public GameObject GetRagdollPrefab()
    {
        return ragdollPrefab;
    }
    public float GetMaxHP()
    {
        return maxHP;
    }
    public void AddBonus(CONSTANT.BonusEnum bonus)
    {
        switch (bonus)
        {
            case CONSTANT.BonusEnum.AddBomb: bombLeft++; break;
            case CONSTANT.BonusEnum.AddFlame: flameDist++; break;
            case CONSTANT.BonusEnum.AddSpeed: walkSpeed++; break;
            case CONSTANT.BonusEnum.AddHP: if (HP < maxHP) HP+=5; break;
            default: break;
        }
    }
    public void GetTrapped(CONSTANT.TrapEnum trap)
    {
        switch (trap)
        {
            case CONSTANT.TrapEnum.Hurt: Hurt(); break;
            case CONSTANT.TrapEnum.Slow: SlowDown(); break;
            case CONSTANT.TrapEnum.DropBomb: 
                getDropBombTrapped=true;
                getDropBombTrapTimer = 3f;
                break;
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
    public void SlowDown()
    {
        if(walkSpeed>=4)
            walkSpeed--;
    }
    public void Hurt()
    {
        HP-=10;
        _audioManager.PlayHurtAudio();
        //_audioSource.PlayOneShot(HurtAudio);
        //_animation.Play(AnimationPlayMode.Mix);//fail to play animation
    }
    public void PlaceBomb()
    {
        if (bombLeft > 0)
        {
            _gameManager.PlaceBomb(gameObject);
        }
            
    }

}
