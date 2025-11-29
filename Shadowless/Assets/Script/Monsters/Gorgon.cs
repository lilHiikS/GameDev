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

    private Flash flashScript;
    private Knockback knockbackScript;
    private Rigidbody2D rb; 

    void Start()
    {
        player = GameObject.FindWithTag("Player").transform;
        targetPoint = pointA.position;
        animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>(); 
        currentHealth = maxHealth;
        flashScript = GetComponent<Flash>();
        knockbackScript = GetComponent<Knockback>();
        
        if (rb != null)
        {
            rb.freezeRotation = true; 
            rb.gravityScale = 0f; 
            rb.bodyType = RigidbodyType2D.Dynamic; 
            
            Debug.Log($"[Gorgon {gameObject.name}] Rigidbody2D configured: bodyType={rb.bodyType}, gravityScale={rb.gravityScale}");
        }
        
        Vector3 currentPos = transform.position;
        transform.position = new Vector3(currentPos.x, -51.5f, currentPos.z); 
        
        Debug.Log($"[Gorgon {gameObject.name}] Positioned at {transform.position}");
    }

    void Update() //her - måske fiks så den ik kører 60 gerne per sekund
    {
        if (isDead) return;
        
        // Debug.Log($"[Gorgon {gameObject.name}] State: {currentState}, Player in detection range: {PlayerInRange(detectionRange)}, Player position: {(player ? player.position.ToString() : "NULL")}");
        
        switch (currentState)
        {
            case GorgonState.Idle:
                if (PlayerInRange(detectionRange))
                {
                    Debug.Log($"[Gorgon {gameObject.name}] Switching to Chase state");
                    currentState = GorgonState.Chase;
                }
                break;

            case GorgonState.Chase:
                ChasePlayer();
                if (PlayerInRange(attackRange))
                    currentState = GorgonState.Attack;
                else if (!PlayerInRange(detectionRange))
                    currentState = GorgonState.Idle; 
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
    if (player == null) return;

    // Stop if knocked back
    if (knockbackScript != null && knockbackScript.isKnockedBack)
    {
        animator.SetBool("isWalking", false);
        return;
    }

    animator.SetBool("isWalking", true);

    Vector2 direction = (player.position - transform.position).normalized;

    // Horizontal only
    float horizontal = direction.x * speed;

    rb.linearVelocity = new Vector2(horizontal, rb.linearVelocity.y);

    // Flip
    if ((horizontal < 0 && transform.localScale.x > 0) ||
        (horizontal > 0 && transform.localScale.x < 0))
    {
        Flip();
    }
}

    void AttackPlayer()
    {
        if (Time.time - lastAttackTime < attackCooldown)
            return;

        animator.SetTrigger("attack");

        if (PlayerInRange(attackRange) && player != null)
        {
            if (player.TryGetComponent<PlayerHealth>(out var playerHealth))
            {
                playerHealth.TakeDamage(attackDamage);
            }

            var pc = player.GetComponent<PlayerController2D>();
            if (pc != null)
            {
                float dir = Mathf.Sign(player.position.x - transform.position.x);
                if (dir == 0f) dir = 1f;

                Vector2 knockback = new Vector2(dir * 5f, 0f); 
                pc.ApplyKnockback(knockback);
            }

            lastAttackTime = Time.time;
        }
    }

    public void TakeDamage(int damage)
    {
        if (isDead) return;

        currentHealth -= damage;

        if (flashScript != null)
        {
            flashScript.HitFlash(); 
        }

        if (knockbackScript != null && player != null)
        {
            float horizontalDirection = Mathf.Sign(transform.position.x - player.position.x);
            if (horizontalDirection == 0f) horizontalDirection = 1f; 
            
            Vector2 knockbackDirection = new Vector2(horizontalDirection, 0f);
            
            knockbackScript.ApplyKnockback(knockbackDirection);
        }

        animator.SetBool("isWalking", false);
        animator.ResetTrigger("attack");

        animator.SetTrigger("hurt"); 

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    private void Die()
    {
        if (isDead) 
        {
            Debug.LogWarning($"[Gorgon {gameObject.name}] Die() called but already dead - skipping");
            return; 
        }
        
        isDead = true;
        
        Debug.Log($"[Gorgon {gameObject.name}] Starting death sequence...");
        animator.SetTrigger("dead"); 

        if (GorgonManager.Instance != null) 
        {
            Debug.Log($"[Gorgon {gameObject.name}] Reporting death to GorgonManager...");
            GorgonManager.Instance.ReportGorgonDefeated(); 
            Debug.Log($"[Gorgon {gameObject.name}] Death reported successfully."); 
        }
        else
        {
            Debug.LogError($"[Gorgon {gameObject.name}] GorgonManager.Instance is NULL! Portal may not open. This could be a race condition.");
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
