using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(SpriteRenderer))]
[RequireComponent(typeof(Animator))]
public class PlayerMovement : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] private float speed = 5f;

    [Header("Jump")]
    [SerializeField] private float jumpForce = 7f;
    [SerializeField] private float negativeJumpForce = 5f;

    [Header("Ground Check")]
    [SerializeField] private Transform groundCheck;          // assign a small empty child at feet
    [SerializeField] private float groundCheckRadius = 0.15f;
    [SerializeField] private LayerMask groundLayer;          // set to your ground layers

    // Animator parameter names (match these in your Animator)
    private static readonly int AnimIsRunning   = Animator.StringToHash("isRunning");
    private static readonly int AnimIsGrounded  = Animator.StringToHash("isGrounded");
    private static readonly int AnimYVelocity   = Animator.StringToHash("yVelocity");
    private static readonly int AnimJumpTrigger = Animator.StringToHash("jump");     // optional
    private static readonly int AnimNegJumpTrig = Animator.StringToHash("negJump");  // optional

    // Components
    private Rigidbody2D body;
    private SpriteRenderer sprite;
    private Animator animator;

    // Input System
    private PlayerControls controls;
    private Vector2 moveInput;

    // State
    private bool isGrounded = false;
    private bool canJump => isGrounded; // single-jump: only when grounded

    private void Awake()
    {
        body = GetComponent<Rigidbody2D>();
        sprite = GetComponent<SpriteRenderer>();
        animator = GetComponent<Animator>();

        controls = new PlayerControls();
    }

    private void OnEnable()
    {
        controls.Player.Enable();

        // Movement input
        controls.Player.Move.performed += ctx => moveInput = ctx.ReadValue<Vector2>();
        controls.Player.Move.canceled  += ctx => moveInput = Vector2.zero;

        // Jump inputs
        controls.Player.Jump.performed         += OnJump;
        controls.Player.NegativeJump.performed += OnNegativeJump;
    }

    private void OnDisable()
    {
        controls.Player.Disable();
    }

    private void Update()
    {
        // Horizontal movement (keep vertical from physics)
        body.linearVelocity = new Vector2(moveInput.x * speed, body.linearVelocity.y);

        // Face sprite based on direction
        if (moveInput.x > 0.01f)  sprite.flipX = false;        // facing right
        else if (moveInput.x < -0.01f) sprite.flipX = true;    // facing left

        // Ground check
        isGrounded = Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, groundLayer);

        // Animator params
        animator.SetBool(AnimIsRunning, Mathf.Abs(moveInput.x) > 0.01f);
        animator.SetBool(AnimIsGrounded, isGrounded);
        animator.SetFloat(AnimYVelocity, body.linearVelocity.y);
    }

    private void OnJump(InputAction.CallbackContext context)
    {
        if (!context.performed) return;

        if (canJump)
        {
            body.linearVelocity = new Vector2(body.linearVelocity.x, jumpForce);
            animator.SetTrigger(AnimJumpTrigger); // optional
        }
    }

    private void OnNegativeJump(InputAction.CallbackContext context)
    {
        if (!context.performed) return;

        if (canJump)
        {
            body.linearVelocity = new Vector2(body.linearVelocity.x, -negativeJumpForce);
            animator.SetTrigger(AnimNegJumpTrig); // optional
        }
    }

    // Visualize ground check in Scene view
    private void OnDrawGizmosSelected()
    {
        if (groundCheck == null) return;
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(groundCheck.position, groundCheckRadius);
    }
}
