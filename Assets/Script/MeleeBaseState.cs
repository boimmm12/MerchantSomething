using System.Collections.Generic;
using UnityEngine;

public class MeleeBaseState : State
{
    public float duration;
    protected Animator animator;
    protected bool shouldCombo;
    protected int attackIndex;

    protected Collider2D hitCollider;
    private readonly List<Collider2D> collidersDamaged = new List<Collider2D>();
    private GameObject HitEffectPrefab;

    // Input buffer
    private float attackPressedTimer = 0f;
    // Pastikan buffer hanya dipakai sekali per state
    private bool comboQueuedThisState = false;
    // Untuk menandai kapan state mulai (opsional filter klik awal)
    private float stateEnterTime = 0f;

    public override void OnEnter(StateMachine _stateMachine)
    {
        base.OnEnter(_stateMachine);

        animator = GetComponent<Animator>();
        hitCollider = GetComponent<ComboCharacter>().hitbox;
        HitEffectPrefab = GetComponent<ComboCharacter>().Hiteffect;

        // --- Penting: reset semua saat masuk state baru ---
        shouldCombo = false;
        comboQueuedThisState = false;
        attackPressedTimer = 0f;          // buang "sisa" klik dari state sebelumnya
        collidersDamaged.Clear();
        stateEnterTime = Time.time;
    }

    public override void OnUpdate()
    {
        base.OnUpdate();

        // Kurangi & clamp
        if (attackPressedTimer > 0f)
            attackPressedTimer = Mathf.Max(0f, attackPressedTimer - Time.deltaTime);

        // Baca input -> buffer pendek saja (hindari auto-chain)
        if (Input.GetMouseButtonDown(0))
        {
            attackPressedTimer = 0.25f; // 250ms cukup untuk human combo timing
        }

        // Serang hanya ketika weapon aktif (sesuai anim param)
        if (animator.GetFloat("Weapon.Active") > 0f)
        {
            Attack();
        }

        // Buka jendela combo?
        bool windowOpen = animator.GetFloat("AttackWindow.Open") > 0f;

        // Gunakan buffer sekali saja di state ini
        if (windowOpen && attackPressedTimer > 0f && !comboQueuedThisState)
        {
            // (Opsional) pastikan klik terjadi setelah masuk state,
            // agar klik pemicu attack1 tidak auto-mengantrikan combo.
            // if (Time.time >= stateEnterTime + 0.05f)

            shouldCombo = true;
            comboQueuedThisState = true;  // konsumsi buffer
            attackPressedTimer = 0f;      // habiskan buffer agar tidak auto-berantai
        }
    }

    public override void OnExit()
    {
        base.OnExit();
        // Tidak perlu membawa flag ke state berikutnya
        // (semua sudah di-reset di OnEnter state baru)
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
                if (hitTeamComponent && hitTeamComponent.teamIndex == TeamIndex.Enemy)
                {
                    GameObject.Instantiate(HitEffectPrefab, col.transform);
                    Debug.Log("Enemy Has Taken:" + attackIndex + "Damage");
                    collidersDamaged.Add(col);
                }
            }
        }
    }
}
