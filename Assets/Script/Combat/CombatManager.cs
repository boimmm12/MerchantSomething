using UnityEngine;
using GDEUtils.StateMachine;
using UnityEngine.InputSystem;

[DisallowMultipleComponent]
public class CombatManager : MonoBehaviour
{
    [Header("Refs")]
    public Animator animator;
    public Collider2D hitbox;
    public GameObject hitEffect;

    [Header("State Machine")]
    public StateMachine<CombatManager> SM { get; private set; }

    [Header("States (assign on same GameObject)")]
    public IdleCombatMB IdleCombat;
    public GroundEntryMB GroundEntry;
    public GroundComboMB GroundCombo;
    public GroundFinisherMB GroundFinisher;

    [SerializeField] private PlayerInput playerInput;
    private InputAction attackAction;

    void Awake()
    {
        if (!animator) animator = GetComponentInChildren<Animator>();
        SM = new StateMachine<CombatManager>(this);
    }
    void OnEnable()
    {
        attackAction = playerInput.actions["Attack"];
        attackAction.performed += OnAttackPerformed;
    }

    void OnDisable()
    {
        if (attackAction != null) attackAction.performed -= OnAttackPerformed;
    }
    void Start()
    {
        SM.Push(IdleCombat);
    }

    void Update()
    {
        SM.Execute();
    }

    public void OnAttackButtonDown()
    {
        if (playerInput.currentActionMap.name != "Player") return;

        if (SM.CurrentState == IdleCombat) SM.ChangeState(GroundEntry);
        else MeleeBaseMB.BufferAttackInputStatic(); // biar masuk ke window berikutnya
    }
    private void OnAttackPerformed(InputAction.CallbackContext ctx)
    {
        if (playerInput.currentActionMap.name != "Player") return;

        if (SM.CurrentState == IdleCombat)
            SM.ChangeState(GroundEntry);
        else
            MeleeBaseMB.BufferAttackInputStatic(); // untuk combo window
    }
}
