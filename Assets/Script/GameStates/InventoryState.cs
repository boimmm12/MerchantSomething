using GDE.GenericSelectionUI;
using GDEUtils.StateMachine;
using UnityEngine;
using UnityEngine.InputSystem;

public class InventoryState : State<GameController>
{
    public static InventoryState i { get; private set; }
    public ItemBase SelectedItem { get; private set; }
    PlayerInput playerInput;
    [SerializeField] InventoryUI inventoryUI;
    void Awake()
    {
        i = this;
        playerInput = GetComponent<PlayerInput>();
    }
    GameController gc;
    Inventory inventory;
    private void Start()
    {
        inventory = Inventory.GetInventory();
    }
    public override void Enter(GameController owner)
    {
        gc = owner;
        inventoryUI.gameObject.SetActive(true);
        inventoryUI.OnBack += OnBack;
        playerInput.SwitchCurrentActionMap("UI");
        inventoryUI.SetSelectionSettings(selectionType: SelectionType.Grid, 8);
    }
    public override void Execute()
    {
        inventoryUI.HandleUpdate();
    }
    public override void Exit()
    {
        inventoryUI.gameObject.SetActive(false);
        inventoryUI.OnBack -= OnBack;
    }
    void OnBack()
    {
        SelectedItem = null;
        gc.StateMachine.Pop();
        playerInput.SwitchCurrentActionMap("Player");
    }
}
