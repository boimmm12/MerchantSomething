using UnityEngine;
using UnityEngine.InputSystem;
using GDEUtils.StateMachine;

public class DialogueState : State<GameController>
{
    PlayerInput input;
    public static DialogueState i { get; private set; }
    void Awake()
    {
        input = GetComponent<PlayerInput>();
        i = this;
    }
    public override void Enter(GameController owner)
    {
        input.SwitchCurrentActionMap("UI");
    }
}
