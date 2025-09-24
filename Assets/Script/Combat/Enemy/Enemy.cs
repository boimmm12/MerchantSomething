using System.Collections.Generic;
using UnityEngine;

public enum EnemyState { Idle, Walking, Attack }

[RequireComponent(typeof(Rigidbody2D))]
public class EnemyBase : MonoBehaviour
{
    [Header("Target")]
    [SerializeField] protected Transform player;      // drag Player
    [SerializeField] protected string playerTag = "Player";

    [Header("Movement")]
    [SerializeField] protected float moveSpeed = 2.5f;
    [SerializeField] protected float stopDistance = 1.4f;     // jarak berhenti sebelum attack
    [SerializeField] protected float detectRange = 6f;        // jarak deteksi awal

    [Header("Attack")]
    [SerializeField] protected Collider2D hitbox;             // collider serangan (Trigger)
    [SerializeField] protected int damage = 10;
    [SerializeField] protected float attackWindup = 0.15f;    // jeda sebelum hit aktif
    [SerializeField] protected float attackActiveTime = 0.20f;// durasi hit aktif
    [SerializeField] protected float attackRecover = 0.35f;   // jeda setelah hit
    [SerializeField] protected float attackCooldown = 0.60f;  // CD antar serangan
    [SerializeField] protected GameObject hitEffect;

    [Header("Animation (opsional)")]
    [SerializeField] protected Animator animator;
    [SerializeField] protected string animIdle = "Idle";
    [SerializeField] protected string animWalk = "Walk";
    [SerializeField] protected string animAttack = "Attack";

    // runtime
    protected EnemyState state = EnemyState.Idle;
    protected Rigidbody2D rb;
    protected float timer;             // timer internal (windup/active/recover)
    protected float cooldownTimer;     // attack cooldown
    protected bool attackHitWindow;    // apakah sedang fase hit aktif
    protected readonly List<Collider2D> damaged = new();

    // ====== UNITY ======
    protected virtual void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        if (!player && !string.IsNullOrEmpty(playerTag))
        {
            var go = GameObject.FindGameObjectWithTag(playerTag);
            if (go) player = go.transform;
        }
        if (!animator) animator = GetComponentInChildren<Animator>();
        if (hitbox) hitbox.isTrigger = true;
        SetState(EnemyState.Idle);
    }

    protected virtual void Update()
    {
        float dt = Time.deltaTime;
        cooldownTimer -= dt;

        switch (state)
        {
            case EnemyState.Idle:
                TickIdle(dt);
                break;
            case EnemyState.Walking:
                TickWalking(dt);
                break;
            case EnemyState.Attack:
                TickAttack(dt);
                break;
        }
    }

    protected virtual void SetState(EnemyState next)
    {
        OnExitState(state);
        state = next;
        OnEnterState(state);
    }

    protected virtual void OnEnterState(EnemyState s)
    {
        timer = 0f;
        attackHitWindow = false;
        damaged.Clear();

        if (animator)
        {
            switch (s)
            {
                case EnemyState.Idle: animator.Play(animIdle, 0, 0f); break;
                case EnemyState.Walking: animator.Play(animWalk, 0, 0f); break;
                case EnemyState.Attack: animator.Play(animAttack, 0, 0f); break;
            }
        }
    }

    protected virtual void OnExitState(EnemyState s)
    {
        attackHitWindow = false;
        damaged.Clear();
    }

    protected virtual void TickIdle(float dt)
    {
        if (!player) return;

        float d = DistToPlayer();
        if (d <= detectRange)
        {
            SetState(EnemyState.Walking);
        }
    }

    protected virtual void TickWalking(float dt)
    {
        if (!player) { SetState(EnemyState.Idle); return; }

        float d = DistToPlayer();

        if (d > stopDistance)
        {
            Vector2 dir = (player.position - transform.position).normalized;
            rb.MovePosition(rb.position + dir * moveSpeed * dt);
            FaceTo(player.position);
        }

        if (d <= stopDistance && cooldownTimer <= 0f)
        {
            BeginAttack();
        }

        // kehilangan target
        if (d > detectRange * 1.5f)
        {
            SetState(EnemyState.Idle);
        }
    }

    protected virtual void TickAttack(float dt)
    {
        timer += dt;

        // fase WINDUP
        if (!attackHitWindow && timer >= attackWindup)
        {
            attackHitWindow = true;
            // mulai HIT AKTIF
        }

        // fase ACTIVE: cek tabrakan
        if (attackHitWindow)
        {
            DoAttackHit();
            // selesai active?
            if (timer >= attackWindup + attackActiveTime)
            {
                attackHitWindow = false; // masuk recover
            }
        }

        // selesai RECOVER?
        if (timer >= attackWindup + attackActiveTime + attackRecover)
        {
            cooldownTimer = attackCooldown;
            // kembali ke walking jika player masih dekat, else idle
            float d = player ? DistToPlayer() : Mathf.Infinity;
            SetState(d <= detectRange ? EnemyState.Walking : EnemyState.Idle);
        }
    }

    protected virtual void BeginAttack()
    {
        timer = 0f;
        attackHitWindow = false;
        damaged.Clear();
        FaceTo(player ? player.position : transform.position);
        SetState(EnemyState.Attack);
    }

    // ====== ATTACK HIT ======
    protected virtual void DoAttackHit()
    {
        if (!hitbox) return;

        // overlap semua collider di dalam hitbox trigger
        var results = new Collider2D[12];
        var filter = new ContactFilter2D { useTriggers = true };
        int count = Physics2D.OverlapCollider(hitbox, filter, results);

        for (int i = 0; i < count; i++)
        {
            var col = results[i];
            if (!col || damaged.Contains(col)) continue;

            // cari komponen Health di target (Player)
            var health = col.GetComponentInChildren<HealthComponent>();
            var team = col.GetComponentInChildren<TeamComponent>();
            if (health && team && team.teamIndex == TeamIndex.Player) // asumsi TeamIndex.Player tersedia di projectmu
            {
                health.TakeDamage(damage);
                damaged.Add(col);

                if (hitEffect)
                    Instantiate(hitEffect, col.bounds.center, Quaternion.identity);
            }
        }
    }

    // ====== HELPERS ======
    protected float DistToPlayer()
    {
        return player ? Vector2.Distance(transform.position, player.position) : Mathf.Infinity;
    }

    protected void FaceTo(Vector3 worldTarget)
    {
        Vector3 dir = worldTarget - transform.position;
        if (dir.x > 0.01f) transform.localScale = new Vector3(1, transform.localScale.y, transform.localScale.z);
        else if (dir.x < -0.01f) transform.localScale = new Vector3(-1, transform.localScale.y, transform.localScale.z);
    }

    // ====== PUBLIC API (bisa dipanggil dari luar) ======
    public virtual void SetPlayer(Transform t) => player = t;
    public EnemyState CurrentState => state;

    // Expose untuk override di turunan
    public virtual void SetStats(float move, float stop, float detect, int dmg,
                                 float windup, float active, float recover, float cd)
    {
        moveSpeed = move;
        stopDistance = stop;
        detectRange = detect;
        damage = dmg;
        attackWindup = windup;
        attackActiveTime = active;
        attackRecover = recover;
        attackCooldown = cd;
    }
}
