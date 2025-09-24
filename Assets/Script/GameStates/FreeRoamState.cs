using GDEUtils.StateMachine;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class FreeRoamState : State<GameController>
{
    public static FreeRoamState i { get; private set; }
    GameController gc;
    PlayerInput input;
    void Awake()
    {
        input = GetComponent<PlayerInput>();
        i = this;
    }
    public override void Enter(GameController owner)
    {
        gc = owner;
    }

    public override void Execute()
    {
        input.SwitchCurrentActionMap("Player");
        PlayerController.i.EnableInput(true);
    }
}
