using UnityEngine;
using GDEUtils.StateMachine;
using UnityEngine.InputSystem;
public class GameController : MonoBehaviour
{
    public StateMachine<GameController> SM { get; private set; }
    PlayerInput playerInput;

    public static GameController Instance { get; private set; }
    void Awake()
    {
        Instance = this;
        SM = new StateMachine<GameController>(this);
        playerInput = GetComponent<PlayerInput>();
        SM.ChangeState(FreeRoamState.i);
    }
    void Start()
    {
        DialogManager.Instance.OnShowDialog += () =>
        {
            SM.Push(DialogueState.i);
        };

        DialogManager.Instance.OnDialogFinished += () =>
        {
            SM.Pop();
            playerInput.SwitchCurrentActionMap("Player");
        };
    }

    private void OnGUI()
    {
        var style = new GUIStyle();
        style.fontSize = 24;
        foreach (var stack in SM.StateStack)
        {
            GUILayout.Label(stack.GetType().ToString(), style);
        }
    }
}
