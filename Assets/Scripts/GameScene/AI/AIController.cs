using UnityEngine;
using System.Collections;
using System.Collections.Generic;
/// <summary>
/// store npc fsm, provide some help method
/// npc decision method
/// </summary>
public class AIController : MonoBehaviour
{
    public GameObject player;
    public float viewDist = 25f;
    private FSMSystem fsm;
    private bool isMoving=false;
    private Vector2[] currentPath;
    private int currentPos;

    private int flameDist = 0;

    private GameManager _gameManager;
    private BombermanMove _bombermanMove;
    private Bomberman _bomberman;
    private CONSTANT.ObjEnum[,] map;
    private float[,] grade;

    public float bombGrade = -5;
    public float dwallGrade = 5;
    public float bonusGrade = 3;
    public float trapGrade = -10;
    public float flameGrade = -10;
    public float playerGrade = 8;
    public float deadendGrade = -2;
    // Use this for initialization
    void Start()
    {
        _gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
        _bombermanMove = GetComponent<BombermanMove>();
        _bomberman = GetComponent<Bomberman>();

        player = _gameManager.GetPlayer();

        grade = new float[CONSTANT.ROW, CONSTANT.COL];

        //create fsm, add state and transition
        fsm = new FSMSystem();

        IdleState idleST = new IdleState();
        idleST.AddTransition(Transition.Idle2Clear,StateID.Clear);
        fsm.AddState(idleST);
        ClearState clearST = new ClearState();
        clearST.AddTransition(Transition.Clear2Chase,StateID.Chase);
        clearST.AddTransition(Transition.Clear2Dead, StateID.Dead);
        clearST.AddTransition(Transition.Clear2Teamwork, StateID.Teamwork);
        clearST.AddTransition(Transition.Clear2Wait, StateID.Wait);
        fsm.AddState(clearST);
        ChaseState chaseST = new ChaseState();
        chaseST.AddTransition(Transition.Chase2Clear, StateID.Clear);
        chaseST.AddTransition(Transition.Chase2Dead, StateID.Dead);
        chaseST.AddTransition(Transition.Chase2Teamwork, StateID.Teamwork);
        chaseST.AddTransition(Transition.Chase2Wait, StateID.Wait);
        fsm.AddState(chaseST);
        WaitSate waitST = new WaitSate();
        waitST.AddTransition(Transition.Wait2Chase, StateID.Chase);
        waitST.AddTransition(Transition.Wait2Clear, StateID.Clear);
        waitST.AddTransition(Transition.Wait2Dead, StateID.Dead);
        waitST.AddTransition(Transition.Wait2Teamwork, StateID.Teamwork);
        fsm.AddState(waitST);
        TeamworkSate teamworkST = new TeamworkSate();
        teamworkST.AddTransition(Transition.Teamwork2Dead, StateID.Dead);
        teamworkST.AddTransition(Transition.Teamwork2Wait, StateID.Wait);
        fsm.AddState(teamworkST);
        fsm.AddState(new DeadState());


    }

    // Update is called once per frame
    void Update()
    {
        map = _gameManager.GetMap();
        flameDist = (int)GetComponent<Bomberman>().flameDist;

        UpdateGrade();

        if (isMoving)
        {
            ThinkForAWhile();
            MoveAlongPath();
        }
        else
        {
            fsm.CurrentState.Reason(player, gameObject);
            fsm.CurrentState.Act(player, gameObject);
            
        }
        Debug.Log(gameObject.name+" "+fsm.CurrentState.ToString());
        
    }
    public void ThinkForAWhile()
    {
        //if next grid is unsafe
        //if next grid is very dangerous(for wait state)
        if (currentPath!=null&&currentPos + 1 < currentPath.Length)
        {
            if (fsm.CurrentStateID == StateID.Wait && GridIsDead(currentPath[currentPos + 1]) || !GridIsSafe(currentPath[currentPos+1]) && fsm.CurrentStateID != StateID.Wait)
            {
                isMoving = false;
            }
        }
        if (CanSeePlayer())
        {
            fsm.CurrentState.Reason(player,gameObject);
        }
        

    }
    public void MoveAlongPath()
    {
        if (currentPath!=null&&currentPos<currentPath.Length&&_bombermanMove.MoveTo(currentPath[currentPos]))
        {
            currentPos++;
            if (currentPos >= currentPath.Length)
            {
                switch (fsm.CurrentStateID)
                {
                    case StateID.Chase: _bomberman.PlaceBomb(); break;
                    case StateID.Clear: _bomberman.PlaceBomb(); break;
                    case StateID.Wait: break;
                    case StateID.Teamwork: _bomberman.PlaceBomb(); break;
                    default: break;
                }
                isMoving = false;
            }
        }
    }
    public void SetPath(Vector2[] path)
    {
        this.currentPath = path;
        currentPos = 0;
        if(path!=null)
            isMoving = true;
    }
    void UpdateGrade()
    {
        for (var i = 0; i < CONSTANT.ROW; i++)
            for (var j = 0; j < CONSTANT.COL; j++)
                grade[i, j] = 0;
        //update grade, maybe can be calculated in gamemanager?
        //just calculate one time
        for (var i = 0; i < CONSTANT.ROW; i++)
        {
            for (var j = 0; j < CONSTANT.COL; j++)
            {
                switch (map[i, j])
                {
                    case CONSTANT.ObjEnum.Bomb: 
                        if (i - 1 >= 0) grade[i - 1, j] += bombGrade;
                        if (i + 1 <= CONSTANT.ROW - 1) grade[i + 1, j] += bombGrade;
                        if (i - 2 >= 0) grade[i - 2, j] += bombGrade / 2;
                        if (i + 2 <= CONSTANT.ROW - 1) grade[i + 2, j] += bombGrade / 2;
                        if (j - 1 >= 0) grade[i, j - 1] += bombGrade;
                        if (j + 1 <= CONSTANT.COL - 1) grade[i, j + 1] += bombGrade;
                        if (j - 2 >= 0) grade[i, j - 2] += bombGrade / 2;
                        if (j + 2 <= CONSTANT.COL - 1) grade[i, j + 2] += bombGrade / 2;
                        break;
                    case CONSTANT.ObjEnum.Flame: 
                        grade[i, j] += flameGrade;
                        break;
                    case CONSTANT.ObjEnum.Bonus: 
                        grade[i, j] += bonusGrade;
                        break;
                    case CONSTANT.ObjEnum.Trap: 
                        grade[i, j] += trapGrade;
                        break;
                    case CONSTANT.ObjEnum.Dwall: 
                        if (i - 1 >= 0) grade[i - 1, j] += dwallGrade;
                        if (i + 1 <= CONSTANT.ROW - 1) grade[i + 1, j] += dwallGrade;
                        if (i - 2 >= 0) grade[i - 2, j] += dwallGrade / 2;
                        if (i + 2 <= CONSTANT.ROW - 1) grade[i + 2, j] += dwallGrade / 2;
                        if (j - 1 >= 0) grade[i, j - 1] += dwallGrade;
                        if (j + 1 <= CONSTANT.COL - 1) grade[i, j + 1] += dwallGrade;
                        if (j - 2 >= 0) grade[i, j - 2] += dwallGrade / 2;
                        if (j + 2 <= CONSTANT.COL - 1) grade[i, j + 2] += dwallGrade / 2;
                        break;
                    case CONSTANT.ObjEnum.Player: 
                        grade[i, j] += playerGrade;
                        break;
                    case CONSTANT.ObjEnum.Empty: 
                        int wayCnt = 0;
                        if (i - 1 >= 0 && CanWalkThrough(new Vector2(i - 1, j))) wayCnt++;
                        if (j - 1 >= 0 && CanWalkThrough(new Vector2(i, j - 1))) wayCnt++;
                        if (i + 1 <= CONSTANT.ROW - 1 && CanWalkThrough(new Vector2(i + 1, j))) wayCnt++;
                        if (j + 1 <= CONSTANT.COL - 1 && CanWalkThrough(new Vector2(i, j + 1))) wayCnt++;
                        grade[i, j] += deadendGrade * (4 - wayCnt);
                        break;
                    case CONSTANT.ObjEnum.Udwall: break;
                    default: break;
                }
            }
        }
    }
    public void SetTransition(Transition trans)
    {
        fsm.PerformTransition(trans);
    }
    public bool CanSeePlayer()
    {
        if (player!=null&&Vector3.Magnitude(player.transform.position - transform.position) <= viewDist) return true;
        return false;
    }
    public Vector2[] PathFindDwall()
    {
        //find the nearest dwall, goto the grid with highest grade
        float maxGrade=0;
        Vector2 tempPos=new Vector2(0,0);
        Vector2 npcPos = Util.World2Map(transform.position);
        int npcX = Mathf.FloorToInt(npcPos.x);
        int npcY = Mathf.FloorToInt(npcPos.y);
        int radiu = Mathf.FloorToInt(Mathf.Sqrt(CONSTANT.ROW*CONSTANT.ROW + CONSTANT.COL*CONSTANT.COL));
        for (var r = 1; r <= radiu; r++)
        {
            for (var i = -r; i <= r; i++)
            {
                for (var j = -r; j <= r; j++)
                {
                    if (Util.ValidMapIndex(npcX+i,npcY+j) && map[npcX + i, npcY + j]==CONSTANT.ObjEnum.Dwall)
                    {
                        if (npcX + i - 1 >= 0 && CanWalkThrough(new Vector2(npcX + i - 1, npcY + j)) && grade[npcX + i - 1, npcY + j] > maxGrade)
                        {
                            tempPos = new Vector2(npcX + i - 1, npcY + j);
                            maxGrade = grade[npcX + i - 1, npcY + j];
                        }
                        else if (npcX + i + 1 <= CONSTANT.ROW-1 && CanWalkThrough(new Vector2(npcX + i + 1, npcY + j)) && grade[npcX + i + 1, npcY + j] > maxGrade)
                        {
                            tempPos = new Vector2(npcX + i + 1, npcY + j);
                            maxGrade = grade[npcX + i + 1, npcY + j];
                        }
                        else if (npcY + j - 1 >= 0 && CanWalkThrough(new Vector2(npcX + i, npcY + j - 1)) && grade[npcX + i, npcY + j - 1] > maxGrade)
                        {
                            tempPos = new Vector2(npcX + i, npcY + j - 1);
                            maxGrade = grade[npcX + i, npcY + j - 1];
                        }
                        else if (npcY + j + 1 <= CONSTANT.ROW-1 && CanWalkThrough(new Vector2(npcX + i, npcY + j+1)) && grade[npcX + i, npcY + j+1] > maxGrade)
                        {
                            tempPos = new Vector2(npcX + i, npcY + j + 1);
                            maxGrade = grade[npcX + i, npcY + j + 1];
                        }
                        
                    }
                }
                    
            }
            if (maxGrade >= 0) return PathFind(tempPos);
        }
        return null;
    }
    public Vector2[] PathFindPlayer()
    {
        var pos = Util.World2Map(player.transform.position);
        int indexX = (int)pos.x;
        int indexY = (int)pos.y;
        if (Util.ValidMapIndex(indexX + 1, indexY) && CanWalkThrough(new Vector2(indexX + 1, indexY))) return PathFind(new Vector2(indexX + 1, indexY));
        else if (Util.ValidMapIndex(indexX - 1, indexY) && CanWalkThrough(new Vector2(indexX - 1, indexY))) return PathFind(new Vector2(indexX - 1, indexY));
        else if (Util.ValidMapIndex(indexX, indexY + 1) && CanWalkThrough(new Vector2(indexX, indexY + 1))) return PathFind(new Vector2(indexX, indexY + 1));
        else if (Util.ValidMapIndex(indexX, indexY - 1) && CanWalkThrough(new Vector2(indexX, indexY - 1))) return PathFind(new Vector2(indexX, indexY - 1));
        return PathFind(Util.World2Map(player.transform.position));
    }
    public Vector2[] PathFindSafe()
    {
        //find the nearest safe grid
        Vector2 npcPos = Util.World2Map(transform.position);
        int npcX = Mathf.FloorToInt(npcPos.x);
        int npcY = Mathf.FloorToInt(npcPos.y);
        int radiu = Mathf.FloorToInt(Mathf.Sqrt(CONSTANT.ROW * CONSTANT.ROW + CONSTANT.COL * CONSTANT.COL));
        for (var r = 1; r <= radiu; r++)
        {
            for (var i = -r; i <= r; i++)
            {
                for (var j = -r; j <= r; j++)
                {
                    
                    if (Util.ValidMapIndex(npcX + i, npcY + j) && CanWalkThrough(new Vector2(npcX + i, npcY + j)))
                    {
                        if (GridIsSafe(new Vector2(npcX + i, npcY + j)))
                        {
                            return PathFind(new Vector2(npcX+i,npcY+j));
                        }

                    }
                }

            }
        }
        return null;
    }
    public bool IsSafe()
    {
        return GridIsSafe(Util.World2Map(transform.position));
    }
    public bool GridIsSafe(Vector2 pos)
    {
        int indexX = (int)pos.x;
        int indexY = (int)pos.y;
        if (Util.ValidMapIndex(indexX, indexY) && map[indexX, indexY] == CONSTANT.ObjEnum.Flame) return false;
        //according to itself bomb flame distance, estimate a flame distance
        for (var d = -flameDist; d <= flameDist; d++)
        {
            if (Util.ValidMapIndex(indexX + d, indexY) && map[indexX + d, indexY] == CONSTANT.ObjEnum.Bomb) return false;
            if (Util.ValidMapIndex(indexX, indexY+d) && map[indexX, indexY+d] == CONSTANT.ObjEnum.Bomb) return false;
        }
        return true;
    }
    public bool GridIsDead(Vector2 pos)//very dangerous
    {
        if (Util.ValidMapIndex((int)pos.x, (int)pos.y) && map[(int)pos.x, (int)pos.y] == CONSTANT.ObjEnum.Flame) return true;
        else return false;
    }
    public Vector2[] PathFindBonus()
    {
        //find the nearest safe grid
        float maxGrade = 0;
        Vector2 tempPos = new Vector2(0, 0);
        Vector2 npcPos = Util.World2Map(transform.position);
        int npcX = Mathf.FloorToInt(npcPos.x);
        int npcY = Mathf.FloorToInt(npcPos.y);
        int radiu = Mathf.FloorToInt(Mathf.Sqrt(CONSTANT.ROW * CONSTANT.ROW + CONSTANT.COL * CONSTANT.COL));
        for (var r = 1; r <= radiu; r++)
        {
            for (var i = -r; i <= r; i++)
            {
                for (var j = -r; j <= r; j++)
                {
                    if (npcX + i >= 0 && npcX+i <= CONSTANT.ROW-1 && npcY+j >= 0 && npcY+j < CONSTANT.COL-1 && map[npcX + i, npcY + j] == CONSTANT.ObjEnum.Bonus && grade[npcX + i, npcY + j] > maxGrade)
                    {
                        tempPos = new Vector2(npcX + i, npcY + j);
                        maxGrade = grade[npcX + i, npcY + j];

                    }
                }

            }
            if (maxGrade >= 0) return PathFind(tempPos);
        }
        return null;
    }

    public Vector2[] PathFind(Vector2 dest)
    {
        Vector2 src = Util.World2Map(transform.position);
        bool[,] walked = new bool[CONSTANT.ROW, CONSTANT.COL];
        for (var i = 0; i < CONSTANT.ROW; i++)
            for (var j = 0; j < CONSTANT.COL; j++)
                walked[i, j] = false;

        Stack<Vector2> path = new Stack<Vector2>();
        path.Push(src);
        walked[Mathf.FloorToInt(src.x), Mathf.FloorToInt(src.y)] = true;
        while (path.Peek()!=dest)
        {
            Vector2 currentpos = path.Peek();
            float minDist = 100f;
            Vector2 upPos = currentpos + new Vector2(0,1);
            Vector2 downPos = currentpos + new Vector2(0,-1);
            Vector2 leftPos = currentpos + new Vector2(-1,0);
            Vector2 rightPos = currentpos + new Vector2(1,0);
            int direction = -1;
            if (CanWalkThrough(upPos) && Util.BlockDist(upPos, dest) < minDist && !walked[Mathf.FloorToInt(upPos.x), Mathf.FloorToInt(upPos.y)])
            {
                direction = 1;
                minDist = Util.BlockDist(upPos, dest);
            }
            if (CanWalkThrough(downPos) && Util.BlockDist(downPos, dest) < minDist && !walked[Mathf.FloorToInt(downPos.x), Mathf.FloorToInt(downPos.y)])
            {
                direction = 2;
                minDist = Util.BlockDist(downPos, dest);
            }
            if (CanWalkThrough(leftPos) && Util.BlockDist(leftPos, dest) < minDist && !walked[Mathf.FloorToInt(leftPos.x), Mathf.FloorToInt(leftPos.y)])
            {
                direction = 3;
                minDist = Util.BlockDist(leftPos, dest);
            }
            if (CanWalkThrough(rightPos) && Util.BlockDist(rightPos, dest) < minDist && !walked[Mathf.FloorToInt(rightPos.x), Mathf.FloorToInt(rightPos.y)])
            {
                direction = 4;
                minDist = Util.BlockDist(rightPos, dest);
            }
            if(direction==-1)
            {
                path.Pop();
                if (path.Count == 0) return null;
            }
            else
            {
                switch (direction)
                {
                    case 1:
                        path.Push(upPos);
                        walked[(int)upPos.x, (int)upPos.y] = true;
                        break;
                    case 2:
                        path.Push(downPos);
                        walked[(int)downPos.x, (int)downPos.y] = true;
                        break;
                    case 3:
                        path.Push(leftPos);
                        walked[(int)leftPos.x, (int)leftPos.y] = true;
                        break;
                    case 4:
                        path.Push(rightPos);
                        walked[(int)rightPos.x, (int)rightPos.y] = true;
                        break;
                    default:
                        break;
                }
            }
        }
        int pathLength = path.Count;
        Vector2[] pathArr = new Vector2[pathLength];
        for (var i = 0; i < pathLength; i++) pathArr[pathLength-i-1] = path.Pop();
        return pathArr;
    }
    public bool CanWalkThrough(Vector2 pos)
    {
        if (pos.x < 0 || pos.x > CONSTANT.ROW - 1 || pos.y < 0 || pos.y > CONSTANT.COL - 1) return false;
        if (map[Mathf.FloorToInt(pos.x), Mathf.FloorToInt(pos.y)] == CONSTANT.ObjEnum.Empty ||
            map[Mathf.FloorToInt(pos.x), Mathf.FloorToInt(pos.y)] == CONSTANT.ObjEnum.Bonus ||
            map[Mathf.FloorToInt(pos.x), Mathf.FloorToInt(pos.y)] == CONSTANT.ObjEnum.Npc ||
            map[Mathf.FloorToInt(pos.x), Mathf.FloorToInt(pos.y)] == CONSTANT.ObjEnum.Player)
            return true;
        return false;
    }


}
