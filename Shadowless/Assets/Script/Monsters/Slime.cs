using UnityEngine;
using System.Collections; // added

public class Slime : MonoBehaviour
{
    private enum State { Idle, Chase, Attack }

    [Header("Detection Settings")]
    public float detectionRange = 5f; // Range to detect the player
    public float attackRange = 1.5f; // Range to attack the player

    [Header("Movement Settings")]
    public float moveSpeed = 2f; // Speed when moving toward the player

    [Header("References")]
    public LayerMask playerLayer; // Layer to detect the player

    [Header("Attack Jump")]
    public float attackJumpYForce = 8f;     // fixed upward speed for attack
    public float maxAttackXSpeed = 6f;      // clamp X speed during attack
    public float attackCooldown = 1f;       // time between attacks

    [Header("Attack Timing")]
    public float preAttackPause = 0.2f;       // delay before jumping

    [Header("Grounding")]
    public Transform groundCheck;
    public float groundCheckRadius = 0.1f;
    public LayerMask groundLayer;           // set in inspector

    private float attackCooldownTimer = 0f;
    private Rigidbody2D rb;
    private Transform player;
    private bool isAttacking = false;
    private bool isWindingUp = false;       // NEW
    private Vector2 attackTargetPosition; // The locked position for the attack
    private Animator animator;

    // FSM
    private State state = State.Idle;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
    }

    void Update()
    {
        // cooldown tick
        if (attackCooldownTimer > 0f) attackCooldownTimer -= Time.deltaTime;

        // Sense player
        UpdatePlayerReference();

        // State update
        switch (state)
        {
            case State.Idle:
                animator.SetBool("move", false);
                TryTransitionFromIdleOrChase();
                break;

            case State.Chase:
                TryTransitionFromIdleOrChase();
                if (state == State.Chase) // still in chase after transition check
                {
                    MoveTowardPlayer();
                    animator.SetBool("move", player != null);
                }
                break;

            case State.Attack:
                // Do not apply chase movement while attacking
                if (isWindingUp)
                    rb.linearVelocity = new Vector2(0f, rb.linearVelocity.y);
                animator.SetBool("move", false);
                break;
        }
    }

    private void UpdatePlayerReference()
    {
        // Keep or clear player based on detection range
        if (player == null)
        {
            var hit = Physics2D.OverlapCircle(transform.position, detectionRange, playerLayer);
            if (hit != null) player = hit.transform;
        }
        else
        {
            // If player exists but moved out of detection range, drop reference
            float dist = Vector2.Distance(transform.position, player.position);
            if (dist > detectionRange) player = null;
        }
    }

    private void TryTransitionFromIdleOrChase()
    {
        if (player == null)
        {
            state = State.Idle;
            return;
        }

        float dist = Vector2.Distance(transform.position, player.position);
        bool canAttack = dist <= attackRange && attackCooldownTimer <= 0f && IsGrounded();

        if (canAttack && !isAttacking && !isWindingUp)
        {
            // Enter attack'
            state = State.Attack;
            isAttacking = true;
            isWindingUp = true;

            // Stop horizontal movement during wind-up
            rb.linearVelocity = new Vector2(0f, rb.linearVelocity.y);

            StartCoroutine(BeginAttackAfterPause());
        }
        else
        {
            // Chase even when inside attackRange if cooldown prevents attacking
            state = State.Chase;
        }
    }

    private void MoveTowardPlayer()
    {
        if (player == null) return;

        // Face direction based on velocity
        if (rb.linearVelocity.x < 0)
        {
            transform.localScale = new Vector3(1, 1, 1); // Face left
        }
        else
        {
            transform.localScale = new Vector3(-1, 1, 1); // Face right
        }

        Vector2 direction = (player.position - transform.position).normalized;
        rb.linearVelocity = new Vector2(direction.x * moveSpeed, rb.linearVelocity.y);
    }

    // NEW: small pause before we play the attack animation
    private IEnumerator BeginAttackAfterPause()
    {
        if (preAttackPause > 0f)
            yield return new WaitForSeconds(preAttackPause);
        isWindingUp = false;

        // Start attack animation; the animation event will call AnimEvent_ExecuteJump()
        animator.SetBool("attack", true);
    }

    public void PrepareAttack()
    {
        if (player == null) { state = State.Idle; animator.SetBool("attack", false); isAttacking = false; return; }

        isAttacking = true;

        // Lock the player's X position at the moment of attack
        attackTargetPosition = new Vector2(player.position.x, transform.position.y);

        // Compute ballistic initial velocity: fixed Y speed, X chosen to reach targetX over airtime
        float g = Mathf.Abs(Physics2D.gravity.y * rb.gravityScale);
        float vy = attackJumpYForce; // Upward speed
        float airTime = g > 0.0001f ? 2f * vy / g : 0.5f; // up+down time
        float dx = attackTargetPosition.x - transform.position.x;
        float vx = Mathf.Clamp(dx / Mathf.Max(airTime, 0.01f), -maxAttackXSpeed, maxAttackXSpeed);

        // Set initial velocities (no mid-air tracking)
        rb.linearVelocity = new Vector2(vx, vy);

        // Wait until grounded to finish attack and start cooldown
        StartCoroutine(WaitForLanding());
    }

    private IEnumerator WaitForLanding()
    {
        // wait until we leave ground first (avoid instant finish if already grounded)
        yield return new WaitWhile(IsGrounded);
        // then wait until we land
        yield return new WaitUntil(IsGrounded);

        animator.SetTrigger("landing");
        animator.SetBool("attack", false);

        isAttacking = false;
        attackCooldownTimer = attackCooldown;

        // Resume behavior
        if (player != null)
        {
            state = State.Chase;
            animator.SetBool("move", true);
        }
        else
        {
            state = State.Idle;
            animator.SetBool("move", false);
        }
    }

    private bool IsGrounded()
    {
        if (groundCheck == null) return true; // if not set, allow attacks
        return Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, groundLayer);
    }

    private void OnDrawGizmosSelected()
    {
        // Draw detection range
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, detectionRange);

        // Draw attack range
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);

        // Ground check
        if (groundCheck != null)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawWireSphere(groundCheck.position, groundCheckRadius);
        }
    }
}