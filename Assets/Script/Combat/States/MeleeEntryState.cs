using GDEUtils.StateMachine;

public class MeleeEntryMB : State<CombatManager>
{
    public override void Enter(CombatManager owner)
    {
        base.Enter(owner);
        // langsung lempar ke GroundEntry (attack 1)
        owner.SM.ChangeState(owner.GroundEntry);
    }
}
