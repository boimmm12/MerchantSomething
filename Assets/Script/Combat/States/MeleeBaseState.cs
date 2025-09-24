using System.Collections.Generic;
using UnityEngine;
using GDEUtils.StateMachine;

public abstract class MeleeBaseMB : State<CombatManager>
{
    [Header("Attack Common")]
    public float duration = 0.5f;
    public int damage = 10;

    protected int attackIndex;
    protected bool shouldCombo;
    protected Animator anim;
    protected Collider2D hitCollider;

    // Timers & buffer
    protected float time;
    protected float attackPressedTimer = 0f;
    protected bool comboQueuedThisState = false;
    private readonly List<Collider2D> collidersDamaged = new List<Collider2D>();

    // static buffer helper (buat dipanggil dari UI button)
    private static float staticPressedTimer = 0f;

    public static void BufferAttackInputStatic()
    {
        staticPressedTimer = 0.25f;
    }

    public override void Enter(CombatManager owner)
    {
        base.Enter(owner);
        anim = owner.animator;
        hitCollider = owner.hitbox;
        shouldCombo = false;
        comboQueuedThisState = false;
        attackPressedTimer = 0f;
        time = 0f;
        collidersDamaged.Clear();
    }

    public override void Execute()
    {
        time += Time.deltaTime;

        // merge static buffer (jika dipicu dari UI)
        if (staticPressedTimer > 0f)
        {
            attackPressedTimer = Mathf.Max(attackPressedTimer, staticPressedTimer);
            staticPressedTimer = 0f;
        }

        // Buffer input mouse
        if (attackPressedTimer > 0f)
            attackPressedTimer = Mathf.Max(0f, attackPressedTimer - Time.deltaTime);

        // if (owner.InputEnabled && owner.CombatEnabled && Input.GetMouseButtonDown(0))
        //     attackPressedTimer = 0.25f;

        // Apply damage saat Weapon.Active
        if (anim.GetFloat("Weapon.Active") > 0f)
            Attack();

        // Combo window
        bool windowOpen = anim.GetFloat("AttackWindow.Open") > 0f;
        if (windowOpen && attackPressedTimer > 0f && !comboQueuedThisState)
        {
            shouldCombo = true;
            comboQueuedThisState = true;
            attackPressedTimer = 0f;
        }
    }

    protected void Attack()
    {
        var collidersToDamage = new Collider2D[10];
        var filter = new ContactFilter2D { useTriggers = true };
        int colliderCount = Physics2D.OverlapCollider(hitCollider, filter, collidersToDamage);

        for (int i = 0; i < colliderCount; i++)
        {
            var col = collidersToDamage[i];
            if (!collidersDamaged.Contains(col))
            {
                var hitTeamComponent = col.GetComponentInChildren<TeamComponent>();
                var health = col.GetComponentInChildren<HealthComponent>();
                if (hitTeamComponent && hitTeamComponent.teamIndex == TeamIndex.Enemy && health != null)
                {
                    //GameObject.Instantiate(HitEffectPrefab, col.transform);
                    Debug.Log("Enemy Has Taken:" + attackIndex + "Damage");
                    health.TakeDamage(damage);
                    collidersDamaged.Add(col);
                }
            }
        }
    }
}
