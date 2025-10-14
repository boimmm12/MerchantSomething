using GDEUtils.StateMachine;
using UnityEngine;

public class TransferState : State<GameController>
{
    [SerializeField] TransferUI transferUI;
    public static TransferState i { get; private set; }
    private void Awake()
    {
        i = this;
    }
    GameController gc;
    public override void Enter(GameController owner)
    {
        gc = owner;
        transferUI.gameObject.SetActive(true);
        transferUI.SetupActionButtons();
        transferUI.SetDataInInventorySlots();
        transferUI.SetDataInMarketSlots();

        transferUI.OnBack += OnBack;
    }

    public override void Execute()
    {
        transferUI.HandleUpdate();
    }

    public override void Exit()
    {
        transferUI.gameObject.SetActive(false);
        transferUI.OnBack -= OnBack;
    }

    void OnBack()
    {
        gc.StateMachine.Pop();
    }
}
