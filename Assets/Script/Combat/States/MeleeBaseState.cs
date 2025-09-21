using System.Collections.Generic;
using UnityEngine;

public class MeleeBaseState : State
{
    public float duration;
    public int damage = 10;
    protected Animator animator;
    protected bool shouldCombo;
    protected int attackIndex;

    protected Collider2D hitCollider;
    private readonly List<Collider2D> collidersDamaged = new List<Collider2D>();
    // private GameObject HitEffectPrefab;

    private float attackPressedTimer = 0f;
    private bool comboQueuedThisState = false;
    // private float stateEnterTime = 0f;

    public override void OnEnter(StateMachine _stateMachine)
    {
        base.OnEnter(_stateMachine);

        animator = GetComponent<Animator>();
        hitCollider = GetComponent<ComboCharacter>().hitbox;
        //HitEffectPrefab = GetComponent<ComboCharacter>().Hiteffect;

        shouldCombo = false;
        comboQueuedThisState = false;
        attackPressedTimer = 0f;
        collidersDamaged.Clear();
        // stateEnterTime = Time.time;
    }

    public override void OnUpdate()
    {
        base.OnUpdate();

        if (attackPressedTimer > 0f)
            attackPressedTimer = Mathf.Max(0f, attackPressedTimer - Time.deltaTime);

        if (Input.GetMouseButtonDown(0))
        {
            attackPressedTimer = 0.25f;
        }

        if (animator.GetFloat("Weapon.Active") > 0f)
        {
            Attack();
        }

        bool windowOpen = animator.GetFloat("AttackWindow.Open") > 0f;

        // Gunakan buffer sekali saja di state ini
        if (windowOpen && attackPressedTimer > 0f && !comboQueuedThisState)
        {
            // if (Time.time >= stateEnterTime + 0.05f)

            shouldCombo = true;
            comboQueuedThisState = true;
            attackPressedTimer = 0f;
        }
    }

    public override void OnExit()
    {
        base.OnExit();
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
