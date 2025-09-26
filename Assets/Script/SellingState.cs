using System.Collections;
using GDEUtils.StateMachine;
using UnityEngine;

public class SellingState : State<GameController>
{
    [SerializeField] ShopUI shopUI;
    public static SellingState i { get; private set; }
    private void Awake()
    {
        i = this;
    }
    GameController gc;

    public override void Enter(GameController owner)
    {
        gc = owner;

        StartCoroutine(StartMenuState());
    }
    IEnumerator StartMenuState()
    {
        shopUI.Show();
        yield return ("o");
    }
}
