using GDEUtils.StateMachine;
using UnityEngine;

public class TransferState : State<GameController>
{
    [SerializeField] TransferUI transferUI;
    [SerializeField] Market market;
    public static TransferState i { get; private set; }
    bool isMoving = false;
    int selectedSlotToMove = 0;
    ItemBase selectedItemToMove = null;
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

        transferUI.OnSelected += OnSlotSelected;
        transferUI.OnBack += OnBack;
        market.OnUpdated += RefreshUI;
    }

    public override void Execute()
    {
        transferUI.HandleUpdate();
    }

    public override void Exit()
    {
        transferUI.gameObject.SetActive(false);
        transferUI.OnSelected -= OnSlotSelected;
        transferUI.OnBack -= OnBack;
        market.OnUpdated -= RefreshUI;
    }

    void OnBack()
    {
        gc.StateMachine.Pop();
    }

    void OnSlotSelected(int slotIndex)
    {
        if (!isMoving)
        {
            var item = transferUI.TakeItemFromSlot(slotIndex);
            if (item != null)
            {
                isMoving = true;
                selectedSlotToMove = slotIndex;
                selectedItemToMove = item;
            }
        }
        else
        {
            isMoving = false;

            int firstSlotIndex = selectedSlotToMove;
            int secondSlotIndex = slotIndex;

            var secondItem = transferUI.TakeItemFromSlot(slotIndex);

            if (secondItem == null && transferUI.IsInventorySlot(firstSlotIndex) && transferUI.IsInventorySlot(secondSlotIndex))
            {
                transferUI.PutItemIntoSlot(selectedItemToMove, selectedSlotToMove);
                transferUI.SetDataInMarketSlots();
                transferUI.SetDataInInventorySlots();
                transferUI.HideMovingImage();
                transferUI.ResetIndex();
                return;
            }

            transferUI.PutItemIntoSlot(selectedItemToMove, secondSlotIndex);

            if (secondItem != null)
                transferUI.PutItemIntoSlot(secondItem, firstSlotIndex);

            // party.Pockies.RemoveAll(p => p == null);
            // party.PartyUpdated();

            transferUI.SetDataInMarketSlots();
            transferUI.SetDataInInventorySlots();
            transferUI.HideMovingImage();
            transferUI.ResetIndex();
        }
    }

    void RefreshUI()
    {
        transferUI.SetDataInMarketSlots();
        transferUI.SetDataInInventorySlots();
        transferUI.HideMovingImage();
        transferUI.ResetIndex();
    }
}
