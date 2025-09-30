using GDEUtils.StateMachine;
using UnityEngine;
using GDE.GenericSelectionUI;

public class NegotiationState : State<GameController>
{
    [SerializeField] NegotiationUI negotiationUI;
    public static NegotiationState i { get; private set; }
    void Awake()
    {
        i = this;
    }
    GameController gc;
    public override void Enter(GameController owner)
    {
        gc = owner;

        negotiationUI.Show();
    }
    public override void Execute()
    {
        negotiationUI.HandleUpdate();
    }
}
