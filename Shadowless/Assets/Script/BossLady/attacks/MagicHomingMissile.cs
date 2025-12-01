using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class MagicHomingMissile : MonoBehaviour
{
    public float speedX = 6f;
    public float direction = 1f; // 1 = højre, -1 = venstre

    public float yFollowStrength = 5f;
    public float maxYVelocity = 10f;

    public float lifetime = 8f;
    public int damage = 1;
    
    private Animator animator;

    Rigidbody2D rb;
    Transform player;
    // Start is called once before the first execution of Update after the MonoBehaviour is created

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    void OnEnable()
    {
        CancelInvoke(nameof(Disable));
        ApplyDirection();
    }
    void Start()
    {
        animator = GetComponent<Animator>();
        if(animator != null)
            animator.Play("Idle"); // eller hvad animationen hedder
        GameObject p = GameObject.FindGameObjectWithTag("Player");
        if (p != null)
            player = p.transform;
        else
            Debug.LogError("Missile kan ikke finde Player! Tjek at spilleren har tag 'Player'.");
        
        Destroy(gameObject, lifetime);
    }
    
    public void Initialize(float dir)
    {
        direction = dir;

        // Flip missilet baseret på direction
        Vector3 scale = transform.localScale;
        scale.x = Mathf.Abs(scale.x) * direction;
        transform.localScale = scale;
    }

    void FixedUpdate()
    {
        float vx = speedX * direction;

        float vy = rb.linearVelocity.y;

        if (player)
        {
            float yDelta = player.position.y - transform.position.y;
            float targetVy = yDelta * yFollowStrength;

            vy = Mathf.MoveTowards(rb.linearVelocity.y, Mathf.Clamp(targetVy, -maxYVelocity, maxYVelocity),
                yFollowStrength * Time.fixedDeltaTime);
        }
        
        rb.linearVelocity = new Vector2(vx, vy);
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            var playerHealth = other.GetComponent<PlayerHealth>();
            if (playerHealth != null)
            {   
                playerHealth.TakeDamage(damage);
                Debug.Log("Missile ramte spilleren og gjorde skade!");
            }
            Disable();

        } 
        else if (other.gameObject.layer == LayerMask.NameToLayer("Ground"))
        {
            Disable();
        }
    }
    void Disable()
    {
        gameObject.SetActive(false);
    }

    private void ApplyDirection()
    {
        Vector3 scale = transform.localScale;
        scale.x = Mathf.Abs(scale.x) * Mathf.Sign(direction); // 1 = højre, -1 = venstre
        transform.localScale = scale;
    }
}
