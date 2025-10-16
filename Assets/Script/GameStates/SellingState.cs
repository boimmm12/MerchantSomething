using System.Collections;
using GDE.GenericSelectionUI;
using GDEUtils.StateMachine;
using UnityEngine;
using UnityEngine.InputSystem;

public class SellingState : State<GameController>
{
    public static SellingState i { get; private set; }

    [SerializeField] ShopUI shopUI;
    [SerializeField] Market market;
    PlayerInput input;
    GameController gc;
    Customer cust;
    bool _leavingForNegotiation = false;


    void Awake()
    {
        i = this;
        input = GetComponent<PlayerInput>();
    }

    public override void Enter(GameController owner)
    {
        gc = owner;
        TimeManager.Instance?.AdvanceTime();
        input.SwitchCurrentActionMap("UI");
        shopUI.OnSelected += OnSelectedMenu;
        shopUI.OnBack += OnBack;

        var item = market ? market.SelectedItem : null;
        if (item == null)
        {
            gc.StateMachine.Pop();
            return;
        }

        cust = market.SelectedCustomer;
        cust?.OnSellingMenuOpened();

        shopUI.ShowNamePrice(item.Name, item.Price, item.Icon, item.NegoPrice);
    }
    public override void Execute()
    {
        shopUI.HandleUpdate();
    }
    public override void Exit()
    {
        shopUI.Hide();
        cust?.OnSellingMenuClosed();

        if (cust != null) market?.CancelSale(cust);

        cust = null;
        input.SwitchCurrentActionMap("Player");

        shopUI.OnBack -= OnBack;
        shopUI.OnSelected -= OnSelectedMenu;

    }
    void OnBack()
    {
        if (cust != null) market?.CancelSale(cust);
        ExitSelling();
    }
    void OnSelectedMenu(int index)
    {
        if (index == 0) // Sell
        {
            if (cust != null) market?.CommitSale(cust);
            ExitSelling();
        }
        else if (index == 1) // Counter
        {
            StartCoroutine(OpenNegotiationAndReturn());
        }
        else // Refuse
        {
            if (cust != null) market?.CancelSale(cust);
            ExitSelling();
        }
    }
    IEnumerator OpenNegotiationAndReturn()
    {
        _leavingForNegotiation = true;                  // <- flag cegah cancel di Exit
        yield return gc.StateMachine.PushAndWait(NegotiationState.i);
        _leavingForNegotiation = false;

        ExitSelling();
    }
    void ExitSelling()
    {
        shopUI.Hide();
        cust?.OnSellingMenuClosed();
        if (!_leavingForNegotiation && cust != null)
            market?.CancelSale(cust);
        cust = null;
        input.SwitchCurrentActionMap("Player");
        gc.StateMachine.Pop();
    }
}
