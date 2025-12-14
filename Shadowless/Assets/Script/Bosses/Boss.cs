using UnityEngine;
using System.Collections;

public class Boss : MonoBehaviour
{
    private enum State { Idle, Chase, ChargeAttack, SpellAttack }

    [Header("Detection Settings")]
    public float detectionRange = 10f; // Range to detect the player
    public float chargeAttackRange = 3f; // Range to perform charge attack

    [Header("Movement Settings")]
    public float moveSpeed = 3f; // Speed when moving toward the player

    [Header("Charge Attack Settings")]
    public float chargeSpeed = 8f; // Speed during charge attack
    public float chargeCooldown = 2f; // Cooldown between charge attacks
    public int chargeDamage = 2; // Damage dealt during charge attack

    [Header("Spell Attack Settings")]
    public GameObject spellPrefab; // Placeholder for the spell object
    public float spellSpawnInterval = 1f; // Time between spell spawns
    public float spellDuration = 5f; // Duration the spell stays active
    public float spellCooldown = 5f; // Cooldown between spell attacks

    [Header("References")]
    public LayerMask playerLayer; // Layer to detect the player

    private Rigidbody2D rb;
    private Transform player;
    private Animator animator;

    private State state = State.Idle;
    private float chargeCooldownTimer = 0f;
    private float spellCooldownTimer = 0f;
    private bool isCharging = false;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
    }

    void Update()
    {
        // Cooldown timers
        if (chargeCooldownTimer > 0f) chargeCooldownTimer -= Time.deltaTime;
        if (spellCooldownTimer > 0f) spellCooldownTimer -= Time.deltaTime;

        // Sense player
        UpdatePlayerReference();

        // State update
        switch (state)
        {
            case State.Idle:
                animator.SetBool("move", false);
                TryTransitionFromIdle();
                break;

            case State.Chase:
                TryTransitionFromChase();
                if (state == State.Chase) // Still in chase after transition check
                {
                    MoveTowardPlayer();
                    animator.SetBool("move", true);
                }
                break;

            case State.ChargeAttack:
                // Charge logic handled in coroutine
                break;

            case State.SpellAttack:
                // Spell logic handled in coroutine
                break;
        }
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

        if (dist <= chargeAttackRange && chargeCooldownTimer <= 0f)
        {
            state = State.ChargeAttack;
            StartCoroutine(PerformChargeAttack());
        }
        else if (spellCooldownTimer <= 0f)
        {
            state = State.SpellAttack;
            StartCoroutine(PerformSpellAttack());
        }
    }

    private void MoveTowardPlayer()
    {
        if (player == null) return;

        Vector2 direction = (player.position - transform.position).normalized;
        rb.linearVelocity = new Vector2(direction.x * moveSpeed, rb.linearVelocity.y);

        // Face the player
        transform.localScale = new Vector3(direction.x > 0 ? -1 : 1, 1, 1);
    }

    private IEnumerator PerformChargeAttack()
    {
        isCharging = true;
        animator.SetTrigger("charge");

        Vector2 direction = (player.position - transform.position).normalized;
        rb.linearVelocity = direction * chargeSpeed;

        yield return new WaitForSeconds(0.5f); // Duration of the charge

        rb.linearVelocity = Vector2.zero;
        isCharging = false;
        chargeCooldownTimer = chargeCooldown;

        state = State.Chase;
    }

    private IEnumerator PerformSpellAttack()
    {
        animator.SetTrigger("castSpell");

        float elapsed = 0f;
        while (elapsed < spellDuration)
        {
            if (player != null)
            {
                Vector3 spawnPosition = player.position + Vector3.up * 2f; // Spawn above the player
                Instantiate(spellPrefab, spawnPosition, Quaternion.identity);
            }

            yield return new WaitForSeconds(spellSpawnInterval);
            elapsed += spellSpawnInterval;
        }

        spellCooldownTimer = spellCooldown;
        state = State.Chase;
    }

    public void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, detectionRange);

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, chargeAttackRange);
    }
}