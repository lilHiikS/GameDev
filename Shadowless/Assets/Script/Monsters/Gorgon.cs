using UnityEngine;

public class GorgonAI : MonoBehaviour, IDamageable
{
    public float speed = 2f;
    public float detectionRange = 20f;
    public float attackRange = 1f;
    public int attackDamage = 1;
    public float attackCooldown = 1f;
    public Transform pointA;
    public Transform pointB;
    public Transform player;
    public Transform groundCheck;
    public LayerMask groundLayer;
    private Vector3 targetPoint;
    private float lastAttackTime = 0f;

    [Header("Health Settings")]
    public int maxHealth = 3;
    private int currentHealth;
    private bool isDead = false;

    [SerializeField]
    private GorgonState currentState = GorgonState.Idle;

    private enum GorgonState { Idle, Chase, Attack }

    [SerializeField]
    private Animator animator;

    [Header("System Management")]
    public GorgonManager manager;

    void Start()
    {
        player = GameObject.FindWithTag("Player").transform;
        targetPoint = pointA.position;
        animator = GetComponent<Animator>();
        currentHealth = maxHealth;
    }

    void Update() //her - måske fiks så den ik kører 60 gerne per sekund
    {
        if (isDead) return;
        switch (currentState)
        {
            case GorgonState.Idle:
                if (PlayerInRange(detectionRange))
                    currentState = GorgonState.Chase;
                break;

            case GorgonState.Chase:
                ChasePlayer();
                if (PlayerInRange(attackRange))
                    currentState = GorgonState.Attack;
                else if (!PlayerInRange(detectionRange))
                    currentState = GorgonState.Chase;
                break;

            case GorgonState.Attack:
                AttackPlayer();
                if (!PlayerInRange(attackRange))
                    currentState = GorgonState.Chase;
                break;
        }
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            player = collision.transform;
            currentState = GorgonState.Chase;
        }
    }

    void Idle()
    {
        animator.SetBool("isIdle", true);
    }

    void ChasePlayer()
    {
        if (player == null)
            return;

        animator.SetBool("isWalking", true);


        var newPosition = Vector2.MoveTowards(transform.position, player.position, speed * Time.deltaTime);
        transform.position = new Vector2(newPosition.x, transform.position.y);

        if ((player.position.x < transform.position.x && transform.localScale.x > 0) ||
            (player.position.x > transform.position.x && transform.localScale.x < 0))
        {
            Flip();
        }
    }

    void AttackPlayer()
    {
        // Check if we can attack (cooldown)
        if (Time.time - lastAttackTime < attackCooldown)
            return;

        animator.SetTrigger("attack");

        // Deal damage to player if in range
        if (PlayerInRange(attackRange) && player != null)
        {
            // Try to get PlayerHealth component
            if (player.TryGetComponent<PlayerHealth>(out var playerHealth))
            {
                playerHealth.TakeDamage(attackDamage);
            }

            // Optional: Apply knockback to player if desired
            var pc = player.GetComponent<PlayerController2D>();
            if (pc != null)
            {
                float dir = Mathf.Sign(player.position.x - transform.position.x);
                if (dir == 0f) dir = 1f;

                Vector2 knockback = new Vector2(dir * 5f, 0f); // tweak force as needed
                pc.ApplyKnockback(knockback);
            }

            lastAttackTime = Time.time;
        }
    }

    public void TakeDamage(int damage)
    {
        if (isDead) return;

        currentHealth -= damage;

        animator.SetBool("isWalking", false);
        animator.ResetTrigger("attack");

        animator.SetTrigger("hurt"); // play hurt animation if available

        if (currentHealth <= 0)
        {
            Die();
        }
    }

 private void Die()
    {
        isDead = true;
        animator.SetTrigger("dead"); 

        if (GorgonManager.Instance != null)
        {
            GorgonManager.Instance.ReportGorgonDefeated(); 
            Debug.Log("Gorgon reported death successfully."); 
        }

        var col = GetComponent<Collider2D>();
        if (col != null) col.enabled = false;

        var rb = GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            rb.linearVelocity = Vector2.zero;
            rb.bodyType = RigidbodyType2D.Kinematic;
        }

        Destroy(gameObject, 2f);
    }


    bool PlayerInRange(float range)
    {
        var inRange = player != null && Vector2.Distance(transform.position, player.position) <= range;
        return inRange;
    }

    bool IsGroundAhead()
    {
        return Physics2D.Raycast(groundCheck.position, Vector2.down, 1f, groundLayer);
    }

    void Flip()
    {
        Vector3 scale = transform.localScale;
        scale.x *= -1;
        transform.localScale = scale;
    }
}
