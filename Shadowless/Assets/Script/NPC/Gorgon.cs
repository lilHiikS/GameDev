using UnityEngine;

public class GorgonAI : MonoBehaviour
{
    public float speed = 2f;
    public float detectionRange = 20f;
    public float attackRange = 1f;
    public int attackDamage = 5;
    public float attackCooldown = 1f;
    public Transform pointA;
    public Transform pointB;
    public Transform player;
    public Transform groundCheck;
    public LayerMask groundLayer;
    private Vector3 targetPoint;
    private float lastAttackTime = 0f;

    [SerializeField]
    private GorgonState currentState = GorgonState.Idle;

    private enum GorgonState { Idle, Chase, Attack }

    [SerializeField]
    private Animator animator;

    void Start()
    {
        targetPoint = pointA.position;
        animator = GetComponent<Animator>();
    }

    void Update() //her - måske fiks så den ik kører 60 gerne per sekund
    {
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
        if (PlayerInRange(attackRange) && PlayerController2D.Instance != null)
        {
            PlayerController2D.Instance.GetComponent<Health>().TakeDamage(attackDamage);
            lastAttackTime = Time.time;
        }
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
