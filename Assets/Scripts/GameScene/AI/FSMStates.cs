using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

/// <summary>
/// This is the idle state, entering idle state after game starting.
/// Then transfer to Clear state.
/// </summary>
public class IdleState : FSMState
{
    public IdleState()
    {
        base.stateID = StateID.Idle;
    }
    public override void Reason(GameObject player, GameObject npc)
    {
        //maybe check whether game has starteds
        npc.GetComponent<AIController>().SetTransition(Transition.Idle2Clear);
    }
    public override void Act(GameObject player, GameObject npc)
    {
        //do nothing
    }
}
/// <summary>
/// This is Clear state, npc trying to clear path.
/// Transition to Dead, Chase, Wait, and teamwork states
/// </summary>
public class ClearState : FSMState
{
    Vector2[] path;
    int currentPos;
    bool arrived = false;

    GameManager gameManager;

    public ClearState()
    {
        base.stateID = StateID.Clear;
        gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
    }
    public override void DoBeforeEntering()
    {
        base.DoBeforeEntering();
        

        arrived = true;//set arrived to be true, and start path finding
    }
    public override void Reason(GameObject player, GameObject npc)
    {
        AIController _aicontroller = npc.GetComponent<AIController>();
        Bomberman _bomberman = npc.GetComponent<Bomberman>();
        //check whether npc dead
        if (_bomberman.HP < 0)
        {
            _aicontroller.SetTransition(Transition.Clear2Dead);
            return;
        }
        //check bombs left
        if (_bomberman.bombLeft <= 0)
        {
            _aicontroller.SetTransition(Transition.Clear2Wait);
            return;
        }
        //if no wall left, switch to teamwork
        if (gameManager.GetDwallLeft()<=0)
        {
            _aicontroller.SetTransition(Transition.Clear2Teamwork);
            return;
        }
        //check player pos, whether player is visible for this npc
        if (_aicontroller.CanSeePlayer())
        {
            _aicontroller.SetTransition(Transition.Clear2Chase);
            return;
        }
        
    }
    public override void Act(GameObject player, GameObject npc)
    {

        AIController _aicontroller = npc.GetComponent<AIController>();
        Bomberman _bomberman = npc.GetComponent<Bomberman>();
        BombermanMove _bombermanMove = npc.GetComponent<BombermanMove>();

        _aicontroller.SetPath(_aicontroller.PathFindDwall());
        
    }

}
/// <summary>
/// This is Chase state, npc trying to chase and attack player.
/// </summary>
public class ChaseState : FSMState
{
    Vector2[] path;
    int currentPos;
    bool arrived = false;

    GameManager gameManager;

    public ChaseState()
    {
        base.stateID = StateID.Chase;
        gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
    }
    public override void DoBeforeEntering()
    {
        base.DoBeforeEntering();

        arrived = true;
        //before entering, find a path to player position
    }
    public override void Reason(GameObject player, GameObject npc)
    {
        AIController _aicontroller = npc.GetComponent<AIController>();
        Bomberman _bomberman = npc.GetComponent<Bomberman>();
        //if dead
        if (_bomberman.HP < 0)
        {
            _aicontroller.SetTransition(Transition.Chase2Dead);
            return;
        }
        //check bombs left
        if (_bomberman.bombLeft <= 0)
        {
            _aicontroller.SetTransition(Transition.Chase2Wait);
            return;
        }
        //if no wall left, switch to teamwork
        if (gameManager.GetDwallLeft() <= 0)
        {
            _aicontroller.SetTransition(Transition.Chase2Teamwork);
            return;
        }
        //if lost player
        if (!_aicontroller.CanSeePlayer())
        {
            _aicontroller.SetTransition(Transition.Chase2Clear);
            return;
        }
    }
    public override void Act(GameObject player, GameObject npc)
    {
        AIController _aicontroller = npc.GetComponent<AIController>();
        Bomberman _bomberman = npc.GetComponent<Bomberman>();
        BombermanMove _bombermanMove = npc.GetComponent<BombermanMove>();
        _aicontroller.SetPath(_aicontroller.PathFindPlayer());

        
    }
    public override void DoBeforeLeaving()
    {
        base.DoBeforeLeaving();
    }
}
public class WaitSate : FSMState
{
    Vector2[] path;
    int currentPos = 0;
    bool arrived = false;

    GameManager gameManager;

    public WaitSate()
    {
        base.stateID = StateID.Wait;
        gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
    }
    public override void DoBeforeEntering()
    {
        base.DoBeforeEntering();

        arrived = true;
    }
    public override void Reason(GameObject player, GameObject npc)
    {
        AIController _aicontroller = npc.GetComponent<AIController>();
        Bomberman _bomberman = npc.GetComponent<Bomberman>();
        if (_bomberman.HP < 0)
        {
            _aicontroller.SetTransition(Transition.Wait2Dead);
            return;
        }
        if (_bomberman.bombLeft > 0)
        {
            if (_aicontroller.CanSeePlayer())
            {
                _aicontroller.SetTransition(Transition.Wait2Chase);
                return;
            }
            else
            {
                _aicontroller.SetTransition(Transition.Wait2Clear);
                return;
            }
        }
        //if can see player
        //if (npc.GetComponent<AIController>().CanSeePlayer()&&npc.GetComponent<AIController>().GridIsSafe(Util.World2Map(player.transform.position)))
        //{
        //    npc.GetComponent<AIController>().SetTransition(Transition.Wait2Chase);
        //    return;
        //}
        //if no wall left, switch to teamwork
        if (gameManager.GetDwallLeft() <= 0)
        {
            _aicontroller.SetTransition(Transition.Wait2Teamwork);
            return;
        }

    }
    public override void Act(GameObject player, GameObject npc)
    {
        AIController _aicontroller = npc.GetComponent<AIController>();
        Bomberman _bomberman = npc.GetComponent<Bomberman>();
        BombermanMove _bombermanMove = npc.GetComponent<BombermanMove>();
        _aicontroller.SetPath(_aicontroller.PathFindSafe());


    }
}
public class TeamworkSate : FSMState
{
    int npcLeft = 0;

    Vector2[] path;
    int currentPos = 0;
    bool arrived = false;

    public TeamworkSate()
    {
        base.stateID = StateID.Teamwork;
    }
    public override void DoBeforeEntering()
    {
        base.DoBeforeEntering();
        npcLeft = GameObject.Find("GameManager").GetComponent<GameManager>().GetNPCLeft();

        arrived = true;
    }
    public override void Reason(GameObject player, GameObject npc)
    {
        AIController _aicontroller = npc.GetComponent<AIController>();
        Bomberman _bomberman = npc.GetComponent<Bomberman>();
        if (_bomberman.HP < 0)
        {
            _aicontroller.SetTransition(Transition.Teamwork2Dead);
            return;
        }
        if (_bomberman.bombLeft <= 0)
        {
            _aicontroller.SetTransition(Transition.Teamwork2Wait);
            return;
        }
    }
    public override void Act(GameObject player, GameObject npc)
    {
        AIController _aicontroller = npc.GetComponent<AIController>();
        Bomberman _bomberman = npc.GetComponent<Bomberman>();
        BombermanMove _bombermanMove = npc.GetComponent<BombermanMove>();
        if (npcLeft == 1)
        {

        }
        //else if (npcLeft == 2)
        //{

        //}
        //else if (npcLeft == 3)
        //{

        //}
        //if (arrived)
        //{
        //    path = _aicontroller.PathFindPlayer();
        //    if (path == null) return;
        //    currentPos = 0;
        //    arrived = false;

        //}
        //if (_bombermanMove.MoveTo(path[currentPos]))
        //{
        //    currentPos++;
        //    if (currentPos >= path.Length)
        //    {
        //        _bomberman.PlaceBomb();
        //        arrived = true;
        //        return;
        //    }
        //    if (!_aicontroller.GridIsSafe(path[currentPos]))//if the next grid is unsafe
        //    {
        //        _aicontroller.SetTransition(Transition.Teamwork2Wait);//refind path
        //    }
        //}

    }
}
public class DeadState : FSMState
{
    public DeadState()
    {
        base.stateID = StateID.Dead;
    }
   
    public override void Reason(GameObject player, GameObject npc)
    {
        GameObject.Find("GameManager").GetComponent<GameManager>().DestroyNPC(npc);
    }
    public override void Act(GameObject player, GameObject npc)
    {
        //throw new NotImplementedException();
    }
}
