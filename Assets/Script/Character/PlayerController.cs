using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    [Header("Move & Dash")]
    [SerializeField] private float speed = 5f;
    [SerializeField] private float dashSpeed = 12f;
    [SerializeField] private float dashDuration = 0.2f;
    [SerializeField] private float dashCooldownTime = 1f;

    [Header("Interact")]
    [SerializeField] private float interactDistance = 0.6f;
    [SerializeField] private float interactRadius = 0.3f;

    [SerializeField] InventoryUI inventoryUI;

    private Rigidbody2D body;
    private Animator anim;

    private Vector2 moveInput;
    private Vector2 lastDir = Vector2.down;
    private Vector2 desiredVelocity;

    private float dashCooldown;
    private bool isDashing = false;
    private bool canInput = true;
    private InputSystem_Actions input;
    public static PlayerController i { get; private set; }

    private void Awake()
    {
        i = this;
        body = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        input = new InputSystem_Actions();

        body.bodyType = RigidbodyType2D.Dynamic;
        body.collisionDetectionMode = CollisionDetectionMode2D.Continuous;
        body.interpolation = RigidbodyInterpolation2D.Interpolate;
        body.freezeRotation = true;
    }

    private void OnEnable()
    {
        input.Enable();

        input.Player.Move.performed += OnMovePerformed;
        input.Player.Move.canceled += OnMoveCanceled;

        input.Player.Roll.performed += OnRollPerformed;
        input.Player.Interact.performed += OnInteractPerformed;
    }

    private void OnDisable()
    {
        input.Player.Move.performed -= OnMovePerformed;
        input.Player.Move.canceled -= OnMoveCanceled;

        input.Player.Roll.performed -= OnRollPerformed;
        input.Player.Interact.performed -= OnInteractPerformed;

        input.Disable();
    }

    private void Update()
    {
        if (!canInput)
        {
            desiredVelocity = Vector2.zero;
            anim.SetFloat("Speed", 0);
            return;
        }

        if (!isDashing)
            desiredVelocity = moveInput * speed;

        anim.SetFloat("Speed", moveInput.sqrMagnitude);
        anim.SetFloat("MoveX", moveInput.x);
        anim.SetFloat("MoveY", moveInput.y);

        if (moveInput.sqrMagnitude > 0.01f)
        {
            lastDir = new Vector2(Mathf.Round(moveInput.x), Mathf.Round(moveInput.y));
            anim.SetFloat("LastX", lastDir.x);
            anim.SetFloat("LastY", lastDir.y);
        }

        if (Input.GetKeyDown(KeyCode.Tab))
            inventoryUI.gameObject.SetActive(true);
        dashCooldown += Time.deltaTime;
    }
    void FixedUpdate()
    {
        body.linearVelocity = desiredVelocity;
    }
    private void OnMovePerformed(InputAction.CallbackContext ctx)
    {
        if (!canInput) return;
        moveInput = ctx.ReadValue<Vector2>().normalized;
    }

    private void OnMoveCanceled(InputAction.CallbackContext ctx)
    {
        moveInput = Vector2.zero;
    }

    private void OnRollPerformed(InputAction.CallbackContext ctx)
    {
        if (!canInput) return;
        TryDash();
    }

    private void OnInteractPerformed(InputAction.CallbackContext ctx)
    {
        if (!canInput || isDashing) return;
        StartCoroutine(Interact());
    }

    private void TryDash()
    {
        if (dashCooldown >= dashCooldownTime && !isDashing)
            StartCoroutine(Dash());
    }

    private IEnumerator Dash()
    {
        isDashing = true;
        anim.SetBool("isDash", true);
        dashCooldown = 0f;

        var dir = (moveInput.sqrMagnitude > 0.01f) ? moveInput : lastDir;
        body.linearVelocity = dir.normalized * dashSpeed;

        yield return new WaitForSeconds(dashDuration);

        isDashing = false;
        anim.SetBool("isDash", false);
    }

    private IEnumerator Interact()
    {
        Vector2 origin = (Vector2)transform.position + lastDir * interactDistance;
        Debug.DrawLine(transform.position, origin, Color.red, 1f);
        var col = Physics2D.OverlapCircle(origin, interactRadius, GameLayer.i.InteractableLayer);
        if (col != null)
        {
            var interactable = col.GetComponent<Interactable>();
            if (interactable != null)
            {
                EnableInput(false);
                yield return interactable.Interact(transform);
                EnableInput(true);
            }
        }
    }

    public void EnableInput(bool enable)
    {
        canInput = enable;
        if (!enable)
        {
            moveInput = Vector2.zero;
            body.linearVelocity = Vector2.zero;
            anim.SetFloat("Speed", 0);
        }
    }
    private void OnCollisionEnter2D(Collision2D c)
    {
        Debug.Log($"Collide with {c.collider.name}");
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Vector2 origin = Application.isPlaying
            ? (Vector2)transform.position + lastDir * interactDistance
            : (Vector2)transform.position + Vector2.down * interactDistance;
        Gizmos.DrawWireSphere(origin, interactRadius);
    }
}
