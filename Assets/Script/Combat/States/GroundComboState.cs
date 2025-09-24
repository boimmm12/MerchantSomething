using UnityEngine;
using GDEUtils.StateMachine;

public class GroundComboMB : MeleeBaseMB
{
    public override void Enter(CombatManager owner)
    {
        base.Enter(owner);
        attackIndex = 2;
        duration = 0.5f;
        anim.SetTrigger("Attack" + attackIndex);
        Debug.Log("Player Attack " + attackIndex + " Fired!");
    }

    public override void Execute()
    {
        base.Execute();
        if (time >= duration)
        {
            if (shouldCombo)
                owner.SM.ChangeState(owner.GroundFinisher);
            else
                owner.SM.ChangeState(owner.IdleCombat);
        }
    }
}
