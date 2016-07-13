using UnityEngine;
using System.Collections;

public class AIBrain : MonoBehaviour {
    
    GameObject player;
    GameManager _gameManager;
    BombermanMove _bombermanMove;
    Bomberman _bomberman;
    
    FSMSystem fsm;
    bool isMoving = false;
    public bool IsMoving { get { return isMoving; } set { isMoving = value; } }

    Vector2[] path;
    int currentPos;

    CONSTANT.ObjEnum[,] map;
    float[,] intersetMap;
    float[,] dangerMap;

	// Use this for initialization
	void Start () {
        fsm = new FSMSystem();
        ClearPathState clearPath = new ClearPathState();
        clearPath.AddTransition(Transition.ClearPath2Attack,StateID.Attack);
        clearPath.AddTransition(Transition.ClearPath2Protect, StateID.Protect);
        clearPath.AddTransition(Transition.ClearPath2SearchBonus, StateID.SearchBonus);
        ProtectState protectState = new ProtectState();
        protectState.AddTransition(Transition.Protect2Attack, StateID.Attack);
        protectState.AddTransition(Transition.Protect2ClearPath, StateID.ClearPath);
        protectState.AddTransition(Transition.Protect2SearchBonus, StateID.SearchBonus);
        AttackState attackState = new AttackState();
        attackState.AddTransition(Transition.Attack2ClearPath, StateID.ClearPath);
        attackState.AddTransition(Transition.Attack2Protect, StateID.Protect);
        attackState.AddTransition(Transition.Attack2SearchBonus, StateID.SearchBonus);
        SearchBonus searchBonus = new SearchBonus();
        searchBonus.AddTransition(Transition.SearchBonus2Attack, StateID.Attack);
        searchBonus.AddTransition(Transition.SearchBonus2ClearPath, StateID.ClearPath);
        searchBonus.AddTransition(Transition.SearchBonus2Protect, StateID.Protect);
        fsm.AddState(clearPath);
        fsm.AddState(protectState);
        fsm.AddState(attackState);
        fsm.AddState(searchBonus);

        _gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
        player = _gameManager.GetPlayer();
        _bomberman = GetComponent<Bomberman>();
        _bombermanMove = GetComponent<BombermanMove>();
	}
	
	// Update is called once per frame
	void Update () {
        map = _gameManager.GetMap();
        UpdateInterestMap();
        UpdateDangerMap();
        if (isMoving)
        {
            ThinkForAWhile();
            if (_bombermanMove.MoveTo(path[currentPos]))
            {
                isMoving = false;
            }
            return;
        }
        if (player != null)
        {
            fsm.CurrentState.Reason(player, gameObject);
            fsm.CurrentState.Act(player, gameObject);
        }
	}
    void ThinkForAWhile()
    {

    }
    public void SetPath(Vector2[] path)
    {
        this.path = path;
        currentPos = 0;
        isMoving = true;
    }
    void UpdateInterestMap()
    {

    }
    void UpdateDangerMap()
    {

    }
}

