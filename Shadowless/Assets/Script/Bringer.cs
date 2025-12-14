using System.Collections;
using Unity.Mathematics;
using UnityEngine;

public class Bringer : MonoBehaviour
{
    private enum State { Idle, Chase, MeleeAttack }

    [Header("Detection Settings")]
    public float detectionRange = 10f; // Range to detect the player
    public float meleeAttackRange = 8f; // Range to perform melee attack

    [Header("Movement Settings")]
    public float moveSpeed = 3f; // Speed when moving toward the player

    [Header("Melee Attack Settings")]
    public float attackCooldown = 1.5f; // Cooldown between attacks
    public int meleeDamage = 2; // Damage dealt during melee attack
    public float attackDuration = 0.5f; // How long the attack animation takes

    [Header("References")]
    public LayerMask playerLayer; // Layer to detect the player

    private Rigidbody2D rb;
    private Transform player;
    public Animator animator;

    private State state = State.Idle;
    private float attackCooldownTimer = 0f;
    private Vector3 originalScale;
    private bool isAttacking = false; // lock turning/movement during attack

    public GameObject attackPoint;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        originalScale = transform.localScale;
    }

    void Update()
    {
        // Cooldown timer
        if (attackCooldownTimer > 0f) attackCooldownTimer -= Time.deltaTime;

        // Sense player
        UpdatePlayerReference();

        // State update
        switch (state)
        {
            case State.Idle:
                TryTransitionFromIdle();
                break;

            case State.Chase:
                TryTransitionFromChase();
                if (state == State.Chase) // Still in chase after transition check
                {
                    MoveTowardPlayer();
                }
                break;

            case State.MeleeAttack:
                // Stop movement and keep stopped during attack
                rb.linearVelocity = Vector2.zero;
                break;
        }

        // Update move animation based on state/attack
        animator.SetBool("move", state == State.Chase && !isAttacking);
    }

    private void UpdatePlayerReference()
    {
        if (player == null)
        {
            var hit = Physics2D.OverlapCircle(transform.position, detectionRange, playerLayer);
            if (hit != null) player = hit.transform;
        }
        else
        {
            float dist = Vector2.Distance(transform.position, player.position);
            if (dist > detectionRange) player = null;
        }
    }

    private void TryTransitionFromIdle()
    {
        if (player != null)
        {
            state = State.Chase;
        }
    }

    private void TryTransitionFromChase()
    {
        if (player == null)
        {
            state = State.Idle;
            return;
        }

        float dist = Vector2.Distance(transform.position, player.position);

        // Check if player is in melee range and attack is off cooldown
        if (dist <= meleeAttackRange && attackCooldownTimer <= 0f)
        {
            state = State.MeleeAttack;
            isAttacking = true;
            PerformMeleeAttack();
        }
    }

    private void MoveTowardPlayer()
    {
        if (player == null) return;

        Vector2 direction = (player.position - transform.position).normalized;
        if (!isAttacking)
        {
            rb.linearVelocity = new Vector2(direction.x * moveSpeed, rb.linearVelocity.y);
        }

        // Face the player (only flip during chase, not during attack)
        if (state == State.Chase && !isAttacking)
        {
            float flipMultiplier = direction.x > 0 ? 1 : -1;
            transform.localScale = new Vector3(originalScale.x * flipMultiplier, originalScale.y, originalScale.z);
        }
    }

    private void PerformMeleeAttack()
    {
        // Stop movement during attack
        isAttacking = true;
        rb.linearVelocity = Vector2.zero;
        animator.SetBool("move", false);

        // Trigger attack animation
        animator.SetTrigger("attack");
    }

    // Animation Event hook: call this at the end of the attack animation
    public void OnMeleeAttackEnd()
    {
        isAttacking = false;
        attackCooldownTimer = attackCooldown;
        state = State.Chase;
    }
    public void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, detectionRange);

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, meleeAttackRange);

        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(attackPoint.transform.position, 5f);
    }

    public void CheckDmg()
    {
        // Check for player in melee range
        var hit = Physics2D.OverlapCircle(attackPoint.transform.position, 5f, playerLayer);
        if (hit != null)
        {
            if (hit.gameObject.TryGetComponent<PlayerHealth>(out var damageable))
            {
                damageable.TakeDamage(meleeDamage);
            }
        }
    }
}
