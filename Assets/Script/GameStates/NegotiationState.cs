using GDEUtils.StateMachine;
using UnityEngine;
using GDE.GenericSelectionUI;

public class NegotiationState : State<GameController>
{
    [SerializeField] NegotiationUI negotiationUI;
    [SerializeField] CustomerNegotiator negotiator;
    [SerializeField] Market market;
    [SerializeField, Range(0f, 1f)] float concessionRate = 0.6f;
    public static NegotiationState i { get; private set; }
    void Awake()
    {
        i = this;
    }
    GameController gc;
    Customer cust;
    float baseCost;
    float sellerOffer;
    public override void Enter(GameController owner)
    {
        gc = owner;

        baseCost = market.SelectedItem.Price;     // HPP
        sellerOffer = market.SelectedItem.NegoPrice;
        cust = market.SelectedCustomer;

        negotiator.ResetSession();
        negotiator.StartNegotiation(baseCost, sellerOffer);

        negotiationUI.Show();
        negotiationUI.SetPrice(Mathf.RoundToInt(sellerOffer));
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
        if (index == 2)
        {
            sellerOffer = negotiationUI.CurrentPrice;
            float counter = negotiator.GetCounterOffer(baseCost, sellerOffer);

            if (counter < 0f)
            {
                Debug.Log("Customer walkaway.");
                market?.CancelSale(cust);
                gc.StateMachine.Pop();
                return;
            }

            if (Mathf.Approximately(counter, sellerOffer))
            {
                Debug.Log($"DEAL @ {sellerOffer:0}");
                market?.CommitSale(cust);
                gc.StateMachine.Pop();
                return;
            }

            sellerOffer = Mathf.Lerp(sellerOffer, counter, concessionRate);
            negotiationUI.SetPrice(Mathf.RoundToInt(sellerOffer));
            Debug.Log($"Counter got {counter:0} → Seller offer now {sellerOffer:0}");
        }
    }
    void OnBack()
    {
        gc.StateMachine.Pop();
        market?.CancelSale(cust);
    }
}
