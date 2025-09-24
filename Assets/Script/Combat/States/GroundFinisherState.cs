using UnityEngine;
using GDEUtils.StateMachine;

public class GroundFinisherMB : MeleeBaseMB
{
    public override void Enter(CombatManager owner)
    {
        base.Enter(owner);
        attackIndex = 3;
        duration = 0.5f;
        anim.SetTrigger("Attack" + attackIndex);
        Debug.Log("Player Attack " + attackIndex + " Fired!");
    }

    public override void Execute()
    {
        base.Execute();
        if (time >= duration)
        {
            owner.SM.ChangeState(owner.IdleCombat);
        }
    }
}
