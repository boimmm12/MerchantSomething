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
        negotiationUI.OnSelected += OnSelectedMenu;
        negotiationUI.OnBack += OnBack;
    }
    public override void Execute()
    {
        negotiationUI.HandleUpdate();
    }
    public override void Exit()
    {
        negotiationUI.gameObject.SetActive(false);
        negotiationUI.OnSelected -= OnSelectedMenu;

        negotiationUI.OnBack -= OnBack;
    }
    void OnSelectedMenu(int index)
    {
        if (index == 2) // Counter
        {
            Debug.Log("pop");
        }
    }
    void OnBack()
    {
        gc.StateMachine.Pop();
    }
}
