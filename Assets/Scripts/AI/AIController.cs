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
    public float viewDist = 5f;
    private FSMSystem fsm;

    private int flameDist = 0;

    private GameManager gameManager;
    private CONSTANT.ObjEnum[,] map;
    private int[,] grade;

    public int bombGrade = -5;
    public int dwallGrade = 5;
    public int bonusGrade = 3;
    public int trapGrade = -2;
    public int flameGrade = -10;
    public int playerGrade = 8;
    public int deadendGrade = -2;
    // Use this for initialization
    void Start()
    {
        gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();

        player = GameObject.Find("player");

        grade = new int[CONSTANT.ROW, CONSTANT.COL];

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
        map = gameManager.GetMap();
        flameDist = GetComponent<NPC>().GetFlameDist();
        UpdateGrade();

        fsm.CurrentState.Reason(player,gameObject);
        fsm.CurrentState.Act(player,gameObject);

        
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
                CONSTANT.ObjEnum obj = map[i, j];
                if (obj == CONSTANT.ObjEnum.Bomb)
                {
                    if (i - 1 >= 0) grade[i - 1, j] += bombGrade;
                    if (i + 1 <= CONSTANT.ROW - 1) grade[i + 1, j] += bombGrade;
                    if (i - 2 >= 0) grade[i - 2, j] += bombGrade / 2;
                    if (i + 2 <= CONSTANT.ROW - 1) grade[i + 2, j] += bombGrade / 2;
                    if (j - 1 >= 0) grade[i, j - 1] += bombGrade;
                    if (j + 1 <= CONSTANT.COL - 1) grade[i, j + 1] += bombGrade;
                    if (j - 2 >= 0) grade[i, j - 2] += bombGrade / 2;
                    if (j + 2 <= CONSTANT.COL - 1) grade[i, j + 2] += bombGrade / 2;
                }
                else if (obj == CONSTANT.ObjEnum.Flame)
                {
                    grade[i, j] += flameGrade;
                }
                else if (obj == CONSTANT.ObjEnum.Bonus)
                {
                    grade[i, j] += bonusGrade;
                }
                else if (obj == CONSTANT.ObjEnum.Trap)
                {
                    grade[i, j] += trapGrade;
                }
                else if (obj == CONSTANT.ObjEnum.Dwall)
                {
                    if (i - 1 >= 0) grade[i - 1, j] += dwallGrade;
                    if (i + 1 <= CONSTANT.ROW - 1) grade[i + 1, j] += dwallGrade;
                    if (i - 2 >= 0) grade[i - 2, j] += dwallGrade / 2;
                    if (i + 2 <= CONSTANT.ROW - 1) grade[i + 2, j] += dwallGrade / 2;
                    if (j - 1 >= 0) grade[i, j - 1] += dwallGrade;
                    if (j + 1 <= CONSTANT.COL - 1) grade[i, j + 1] += dwallGrade;
                    if (j - 2 >= 0) grade[i, j - 2] += dwallGrade / 2;
                    if (j + 2 <= CONSTANT.COL - 1) grade[i, j + 2] += dwallGrade / 2;
                }
                else if (obj == CONSTANT.ObjEnum.Player)
                {
                    grade[i, j] += playerGrade;
                }
                if (obj == CONSTANT.ObjEnum.Empty)
                {
                    int wayCnt = 0;
                    if (i - 1 >= 0 && CanWalkThrough(new Vector2(i - 1, j))) wayCnt++;
                    if (j - 1 >= 0 && CanWalkThrough(new Vector2(i, j - 1))) wayCnt++;
                    if (i + 1 <= CONSTANT.ROW - 1 && CanWalkThrough(new Vector2(i + 1, j))) wayCnt++;
                    if (j + 1 <= CONSTANT.COL - 1 && CanWalkThrough(new Vector2(i, j + 1))) wayCnt++;
                    grade[i, j] += deadendGrade * (4 - wayCnt);
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
        if (Vector3.Magnitude(player.transform.position - transform.position) <= viewDist) return true;
        return false;
    }
    public Vector2[] PathFindDwall()
    {
        //find the nearest dwall, goto the grid with highest grade
        int maxGrade=0;
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
                    if (npcX + i >= 0 && npcX+i <= CONSTANT.ROW-1 && npcY+j >= 0 && npcY+j < CONSTANT.COL-1 && map[npcX + i, npcY + j]==CONSTANT.ObjEnum.Dwall)
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
            if (maxGrade > 0) return PathFind(tempPos);
        }
        return null;
    }
    public Vector2[] PathFindPlayer()
    {
        return PathFind(Util.World2Map(player.transform.position));
    }
    public Vector2[] PathFindSafe()
    {
        //find the nearest safe grid
        int maxGrade = 0;
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
                    if (npcX + i >= 0 && npcX+i <= CONSTANT.ROW-1 && npcY+j >= 0 && npcY+j < CONSTANT.COL-1 && CanWalkThrough(new Vector2(npcX + i, npcY + j)) )
                    {
                        bool isSafe = true;
                       //according to itself bomb flame distance, estimate a flame distance
                        for (var d = -flameDist; d <= flameDist; d++)
                        {
                            if (map[npcX + i + d, npcY + j] == CONSTANT.ObjEnum.Bomb) isSafe = false;
                            if (map[npcX + i, npcY + j + d] == CONSTANT.ObjEnum.Bomb) isSafe = false;
                        }
                        if (isSafe || grade[npcX + i, npcY + j] > maxGrade)
                        {
                            tempPos = new Vector2(npcX + i, npcY + j);
                            maxGrade = grade[npcX + i, npcY + j];
                        }

                    }
                }

            }
            if (maxGrade > 0) return PathFind(tempPos);
        }
        return null;
    }
    public Vector2[] PathFindBonus()
    {
        //find the nearest safe grid
        int maxGrade = 0;
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
            if (maxGrade > 0) return PathFind(tempPos);
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
