using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController2D : MonoBehaviour
{
    [Header("Parameters")]
    public float jumpForce = 10f;
    public float moveSpeed = 5f;

    [Header("Components")]
    public Rigidbody2D rb;
    public GameObject groundCheck;
    public LayerMask groundLayer;
    public Animator animator;

    private bool isGrounded;
    private float horizontalMovement;
    private float groundCheckRadius = 0.3f;
    private bool jump;
    private bool facingRight = true;
    private IInteractable interactable;
    private float jumpCooldown = 0.2f;
    private float jumpTimer = 0f;

    public static PlayerController2D Instance;

    public float Health, MaxHealth;

    [SerializeField]
    private HP healthBar;

    void Start()
    {
        healthBar.SetMaxHealth(MaxHealth);
    }

    // Method for taking damage from enemies
    public void TakeDamage(float damage)
    {
        SetHealth(-damage);
    }

    public void SetHealth(float healthChange)
    {
        Health += healthChange;
        Health = Mathf.Clamp(Health, 0, MaxHealth);
        healthBar.SetHealth(Health);
    }

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject); // Destroy duplicate player
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject); // Persist across scenes
    }

    void FixedUpdate()
    {
        isGrounded = Physics2D.OverlapCircle(groundCheck.transform.position, groundCheckRadius, groundLayer);

        // Handle horizontal player movement
        float currentSpeed = isGrounded ? moveSpeed : moveSpeed * 0.7f;
        Vector2 movement = new Vector2(horizontalMovement * currentSpeed, rb.linearVelocity.y);
        rb.linearVelocity = movement;

        // Set running animation
        animator.SetBool("isRunning", isGrounded && horizontalMovement != 0);

        // Set falling animation
        animator.SetBool("isFalling", rb.linearVelocityY < 0 && !isGrounded);

        // Set Jumping animation
        animator.SetBool("isJumping", rb.linearVelocityY > 0 && !isGrounded);

        if (jumpTimer < jumpCooldown)
        {
            jumpTimer += Time.fixedDeltaTime;
        }

        if (jump && isGrounded && jumpTimer >= jumpCooldown)
        {
            rb.linearVelocityY = 0f;
            rb.AddForceY(jumpForce, ForceMode2D.Impulse);
            jump = false;
            jumpTimer = 0f;
        }

        // Handle character flipping
        if (facingRight && horizontalMovement < 0)
            Flip();
        else if (!facingRight && horizontalMovement > 0)
            Flip();
    }

    public void Move(InputAction.CallbackContext context)
    {
        horizontalMovement = context.ReadValue<Vector2>().x;
    }

    public void Jump(InputAction.CallbackContext context)
    {
        if (!context.performed) return;
        if (isGrounded)
            jump = true;
    }

    private void Flip()
    {
        facingRight = !facingRight;
        float yRotation = facingRight ? 0f : 180f;
        transform.rotation = Quaternion.Euler(0f, yRotation, 0f);
    }

    void OnDrawGizmos()
    {
        if (groundCheck != null)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawWireSphere(groundCheck.transform.position, groundCheckRadius);
        }
    }

    void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.CompareTag("Interactable") && interactable == null)
        {
            interactable = collision.GetComponent<IInteractable>();
        }
    }

    void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Interactable"))
        {
            interactable = null;
        }
    }

    public void Interact(InputAction.CallbackContext context)
    {
        if (!context.performed) return;
        if (interactable != null)
        {
            interactable.Interact();
        }
    }
}
