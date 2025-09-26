using System.Collections.Generic;
using UnityEngine;

public enum EnemyState { Idle, Walking, Attack }

public class EnemyBase : MonoBehaviour
{
    [SerializeField] float moveSpeed;
    [SerializeField] float stopDistance;

    [Header("Attack")]
    [SerializeField] float attackRadius;
    [SerializeField] Collider2D hitbox;
    [SerializeField] int damage;
    [SerializeField] float attackWindup;
    [SerializeField] float attackActive;
    [SerializeField] float attackRecover;
    [SerializeField] float attackCooldown;

    float atkTimer;
    float cdTimer;
    bool hitWindow;
    readonly List<Collider2D> damaged = new();

    Animator anim;
    EnemyState state;
    Transform player;
    Rigidbody2D rb;
    private Vector2 lastDir = Vector2.down;

    void Awake()
    {
        var p = GameObject.FindGameObjectWithTag("Player");
        if (p) player = p.transform;

        anim = GetComponent<Animator>();
        if (!anim) anim = GetComponentInChildren<Animator>();

        rb = GetComponent<Rigidbody2D>();
        state = EnemyState.Idle;

        if (hitbox) hitbox.isTrigger = true;
    }

    void Update()
    {
        if (!player) return;

        cdTimer -= Time.deltaTime;

        Vector2 toTarget = player.position - transform.position;
        float dist = toTarget.magnitude;

        if (dist <= attackRadius && cdTimer <= 0f)
        {
            Attack(toTarget);
        }
        else if (dist > stopDistance)
        {
            Walk(toTarget);
        }
        else
        {
            anim.SetFloat("MoveX", lastDir.x);
            anim.SetFloat("MoveY", lastDir.y);
            anim.SetFloat("Speed", 0f);

            if (state == EnemyState.Attack && atkTimer <= 0f) state = EnemyState.Idle;
        }

        if (state == EnemyState.Attack)
            TickAttack();
    }

    void Walk(Vector2 toTarget)
    {
        state = EnemyState.Walking;

        Vector2 dir = toTarget.normalized;
        rb.MovePosition(rb.position + dir * moveSpeed * Time.deltaTime);

        lastDir = dir;
        anim.SetFloat("MoveX", dir.x);
        anim.SetFloat("MoveY", dir.y);
        anim.SetFloat("Speed", moveSpeed);

        state = EnemyState.Idle;
    }

    void Attack(Vector2 toTarget)
    {
        if (state != EnemyState.Attack)
        {
            state = EnemyState.Attack;

            Vector2 dir = toTarget.sqrMagnitude > 0.0001f ? toTarget.normalized : lastDir;
            lastDir = dir;
            anim.SetFloat("MoveX", lastDir.x);
            anim.SetFloat("MoveY", lastDir.y);
            anim.SetFloat("Speed", 0f);

            anim.ResetTrigger("Attack");
            anim.SetTrigger("Attack");

            atkTimer = attackWindup + attackActive + attackRecover;
            hitWindow = false;
            damaged.Clear();
        }
    }

    void TickAttack()
    {
        float dt = Time.deltaTime;
        float elapsed = attackWindup + attackActive + attackRecover - atkTimer;
        atkTimer -= dt;

        if (!hitWindow && elapsed >= attackWindup)
            hitWindow = true;

        if (hitWindow)
        {
            DoAttackHit();
            if (elapsed >= attackWindup + attackActive)
                hitWindow = false;
        }

        if (atkTimer <= 0f)
        {
            state = EnemyState.Idle;
            cdTimer = attackCooldown;
        }
    }

    void DoAttackHit()
    {
        if (!hitbox) return;

        var results = new Collider2D[12];
        var filter = new ContactFilter2D { useTriggers = true };
        int count = Physics2D.OverlapCollider(hitbox, filter, results);

        for (int i = 0; i < count; i++)
        {
            var col = results[i];
            if (!col || damaged.Contains(col)) continue;

            var health = col.GetComponentInChildren<HealthComponent>();
            var team = col.GetComponentInChildren<TeamComponent>();
            if (health && team && team.teamIndex == TeamIndex.Player)
            {
                health.TakeDamage(damage);
                damaged.Add(col);
            }
        }

    }
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, attackRadius);
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(transform.position, stopDistance);
    }
}
