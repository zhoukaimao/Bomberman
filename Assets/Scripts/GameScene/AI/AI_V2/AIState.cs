using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ClearPathState : FSMState
{
    public ClearPathState()
    {
        stateID = StateID.ClearPath;
    }
    public override void Act(GameObject player, GameObject npc)
    {
        AIBrain brain = npc.GetComponent<AIBrain>();

    }
    public override void Reason(GameObject player, GameObject npc)
    {
        throw new System.NotImplementedException();
    }
}
public class ProtectState : FSMState
{
    public ProtectState()
    {
        stateID = StateID.Protect;
    }
    public override void Act(GameObject player, GameObject npc)
    {
        throw new System.NotImplementedException();
    }
    public override void Reason(GameObject player, GameObject npc)
    {
        throw new System.NotImplementedException();
    }
}
public class AttackState : FSMState
{
    public AttackState()
    {
        stateID = StateID.Attack;
    }
    public override void Act(GameObject player, GameObject npc)
    {
        throw new System.NotImplementedException();
    }
    public override void Reason(GameObject player, GameObject npc)
    {
        throw new System.NotImplementedException();
    }
}
public class SearchBonus : FSMState
{
    public SearchBonus()
    {
        stateID = StateID.SearchBonus;
    }
    public override void Act(GameObject player, GameObject npc)
    {
        throw new System.NotImplementedException();
    }
    public override void Reason(GameObject player, GameObject npc)
    {
        throw new System.NotImplementedException();
    }
}