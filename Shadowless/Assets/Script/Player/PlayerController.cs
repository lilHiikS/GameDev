using System.Collections;
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

    public Animator UI;

    private bool isGrounded;
    private float horizontalMovement;
    private float groundCheckRadius = 0.3f;
    private bool jump;
    private bool facingRight = true;
    private IInteractable interactable;
    private float jumpCooldown = 0.2f;
    private float jumpTimer = 0f;


    private bool isStunned = false;

    public PlayerHealth health;

    public PlayerAttack playerAttack;

    public static PlayerController2D Instance;

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
        if (health.isDead)
        {
            rb.linearVelocity = new Vector2(0, rb.linearVelocity.y);
            animator.SetBool("isRunning", false);
            animator.SetBool("isFalling", false);
            animator.SetBool("isJumping", false);
            UI.SetTrigger("Death");
            return;
        }

        if (isStunned)
        {
            return;
        }

        if (playerAttack.isAttacking && isGrounded)
        {
            rb.linearVelocity = new Vector2(0, rb.linearVelocity.y);
            return;
        }

        isGrounded = Physics2D.OverlapCircle(groundCheck.transform.position, groundCheckRadius, groundLayer);

        // If stunned, do not overwrite horizontal velocity — allow physics to play out
        if (!isStunned)
        {
            float currentSpeed = isGrounded ? moveSpeed : moveSpeed * 0.7f;
            Vector2 movement = new Vector2(horizontalMovement * currentSpeed, rb.linearVelocity.y);
            rb.linearVelocity = movement;
        }
        else
        {
            // keep animations consistent while stunned
            animator.SetBool("isRunning", false);
        }

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

    // Public API for external knockback callers (Slime)
    public void ApplyKnockback(Vector2 velocity)
    {
        if (rb == null) return;
        isStunned = true;
        rb.linearVelocity = velocity;
        StartCoroutine(WaitForUnstun(0.25f));
    }

    private IEnumerator WaitForUnstun(float time)
    {
        yield return new WaitForSeconds(time);
        isStunned = false;
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

    public void Attack(InputAction.CallbackContext context)
    {
        if (!context.started || health.isDead) return;
        playerAttack.Attack();
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
