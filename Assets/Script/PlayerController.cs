using System.Collections;
using UnityEngine;

public class test : MonoBehaviour
{
    [SerializeField] private float speed = 5f;
    [SerializeField] private float dashSpeed = 12f;
    [SerializeField] private float dashDuration = 0.2f;
    [SerializeField] private float dashCooldownTime = 1f;

    private Rigidbody2D body;
    private Animator anim;
    private float horizontalInput;
    private float verticalInput;
    private float dashCooldown;
    private bool isDashing = false;
    private Vector2 lastDir = Vector2.down;

    private void Awake()
    {
        body = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
    }

    private void Update()
    {
        horizontalInput = Input.GetAxisRaw("Horizontal");
        verticalInput = Input.GetAxisRaw("Vertical");
        Vector2 input = new Vector2(horizontalInput, verticalInput).normalized;

        if (!isDashing)
            body.linearVelocity = input * speed;

        anim.SetFloat("Speed", input.sqrMagnitude);
        anim.SetFloat("MoveX", horizontalInput);
        anim.SetFloat("MoveY", verticalInput);

        if (input.sqrMagnitude > 0.01f)
        {
            lastDir = input;
            Vector2 snapped = new Vector2(Mathf.Round(lastDir.x), Mathf.Round(lastDir.y));
            anim.SetFloat("LastX", snapped.x);
            anim.SetFloat("LastY", snapped.y);
        }


        dashCooldown += Time.deltaTime;

        if (Input.GetKeyDown(KeyCode.Space) && dashCooldown >= dashCooldownTime && !isDashing)
        {
            StartCoroutine(Dash());
        }
    }

    private IEnumerator Dash()
    {
        isDashing = true;
        anim.SetBool("isDash", true);
        dashCooldown = 0f;

        body.linearVelocity = lastDir * dashSpeed;

        yield return new WaitForSeconds(dashDuration);

        isDashing = false;
        anim.SetBool("isDash", false);
    }
}
