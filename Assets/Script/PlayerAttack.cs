using Unity.VisualScripting;
using UnityEngine;

public class PlayerAttack : MonoBehaviour
{
    private float timeBAtt;
    public float startTimeBAtt;
    private Animator anim;
    public Transform attackPos;
    [SerializeField] float attackOffset = 0.6f;
    public LayerMask enemies;
    public float attackRange;
    public int damage;

    public float cooldownTime = 2f;
    private float nextTime = 0f;
    public static int numClicks = 0;
    float lastClickedTime = 0;
    float maxComboDelay = 1;
    void Awake()
    {
        anim = GetComponent<Animator>();
    }
    void Update()
    {
        // Vector2 dir = GetFacingDir();
        // attackPos.localPosition = dir * attackOffset;
        // attackPos.right = dir;

        // if (timeBAtt > 0f)
        //     timeBAtt -= Time.deltaTime;

        // if (Input.GetMouseButtonDown(0) && timeBAtt <= 0f)
        // {
        //     Attack();
        //     timeBAtt = startTimeBAtt;
        // }

        if (anim.GetCurrentAnimatorStateInfo(0).normalizedTime > 0.7f && anim.GetCurrentAnimatorStateInfo(0).IsName("Combo1Left"))
        {
            anim.SetBool("hit1", false);
        }
        if (anim.GetCurrentAnimatorStateInfo(0).normalizedTime > 0.7f && anim.GetCurrentAnimatorStateInfo(0).IsName("Combo2Left"))
        {
            anim.SetBool("hit2", false);
        }
        if (anim.GetCurrentAnimatorStateInfo(0).normalizedTime > 0.7f && anim.GetCurrentAnimatorStateInfo(0).IsName("Combo3Left"))
        {
            anim.SetBool("hit3", false);
            numClicks = 0;
        }
        if (Time.time - lastClickedTime > maxComboDelay)
        {
            numClicks = 0;
        }
        if (Time.time > nextTime)
        {
            if (Input.GetMouseButtonDown(0))
            {
                OnClick();
            }
        }
    }

    void Attack()
    {
        anim.SetTrigger("Attack");
        Collider2D[] enemiesToDamage = Physics2D.OverlapCircleAll(attackPos.position, attackRange, enemies);
        for (int i = 0; i < enemiesToDamage.Length; i++)
        {
            enemiesToDamage[i].GetComponent<Enemy>().TakeDamage(damage);
        }
    }
    Vector2 GetFacingDir()
    {
        // pakai LastX/LastY (idle pun benar), fallback ke MoveX/MoveY
        float x = anim.GetFloat("LastX");
        float y = anim.GetFloat("LastY");
        if (Mathf.Abs(x) < 0.01f && Mathf.Abs(y) < 0.01f)
        {
            x = anim.GetFloat("MoveX");
            y = anim.GetFloat("MoveY");
        }

        var d = new Vector2(x, y);
        if (d.sqrMagnitude < 0.01f) d = Vector2.down;       // default hadap bawah
        // snap ke 8 arah (hilangkan kalau mau bebas)
        d = new Vector2(Mathf.Round(d.x), Mathf.Round(d.y));
        return d.normalized;
    }

    void OnDrawGizmosSelected()
    {
        if (attackPos == null) return;
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(attackPos.position, attackRange);
    }

    void OnClick()
    {
        lastClickedTime = Time.time;
        numClicks++;
        if (numClicks == 1)
        {
            anim.SetBool("hit1", true);
        }
        numClicks = Mathf.Clamp(numClicks, 0, 3);

        if (numClicks >= 2 && anim.GetCurrentAnimatorStateInfo(0).normalizedTime > 0.7f && anim.GetCurrentAnimatorStateInfo(0).IsName("Combo1Left"))
        {
            anim.SetBool("hit1", false);
            anim.SetBool("hit2", true);
        }
        if (numClicks >= 3 && anim.GetCurrentAnimatorStateInfo(0).normalizedTime > 0.7f && anim.GetCurrentAnimatorStateInfo(0).IsName("Combo2Left"))
        {
            anim.SetBool("hit2", false);
            anim.SetBool("hit3", true);
        }
    }
}
