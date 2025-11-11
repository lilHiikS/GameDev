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

    [Header("Audio Sources")]
    [SerializeField] private AudioSource footstepSource;
    [SerializeField] private AudioSource jumpSource;

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
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
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

        bool wasGrounded = isGrounded;
        isGrounded = Physics2D.OverlapCircle(groundCheck.transform.position, groundCheckRadius, groundLayer);

        if (!isStunned)
        {
            float currentSpeed = isGrounded ? moveSpeed : moveSpeed * 0.7f;
            Vector2 movement = new Vector2(horizontalMovement * currentSpeed, rb.linearVelocity.y);
            rb.linearVelocity = movement;
        }
        else
        {
            animator.SetBool("isRunning", false);
        }

        animator.SetBool("isRunning", isGrounded && horizontalMovement != 0);
        animator.SetBool("isFalling", rb.linearVelocityY < 0 && !isGrounded);
        animator.SetBool("isJumping", rb.linearVelocityY > 0 && !isGrounded);

        // Footstep sound logic
        if (isGrounded && horizontalMovement != 0)
        {
            if (!footstepSource.isPlaying)
                footstepSource.Play();
        }
        else
        {
            if (footstepSource.isPlaying)
                footstepSource.Stop();
        }

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

            if (jumpSource != null)
                jumpSource.Play();
        }

        if (facingRight && horizontalMovement < 0)
            Flip();
        else if (!facingRight && horizontalMovement > 0)
            Flip();
    }

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
