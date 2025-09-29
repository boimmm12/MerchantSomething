using UnityEngine;
using GDEUtils.StateMachine;
using UnityEngine.InputSystem;
public class GameController : MonoBehaviour
{
    public StateMachine<GameController> StateMachine { get; private set; }
    PlayerInput playerInput;

    public static GameController Instance { get; private set; }
    void Awake()
    {
        Instance = this;
        playerInput = GetComponent<PlayerInput>();
    }
    void Start()
    {
        StateMachine = new StateMachine<GameController>(this);
        StateMachine.Push(FreeRoamState.i);
        DialogManager.Instance.OnShowDialog += () =>
        {
            StateMachine.Push(DialogueState.i);
        };

        DialogManager.Instance.OnDialogFinished += () =>
        {
            StateMachine.Pop();
            playerInput.SwitchCurrentActionMap("Player");
        };
    }

    private void Update()
    {
        StateMachine.Execute();
    }
    private void OnGUI()
    {
        var style = new GUIStyle();
        style.fontSize = 24;
        foreach (var stack in StateMachine.StateStack)
        {
            GUILayout.Label(stack.GetType().ToString(), style);
        }
    }
}
