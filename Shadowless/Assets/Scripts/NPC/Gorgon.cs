using UnityEngine;

public class GorgonAI : MonoBehaviour
{
    // === General Settings ===
    public float speed = 2f;
    public float detectionRange = 20f;
    public float attackRange = 1f;
    public Transform pointA;
    public Transform pointB;
    public Transform player;
    public Transform groundCheck;
    public LayerMask groundLayer;

    private Vector3 targetPoint;
    private GorgonState currentState = GorgonState.Chase;

    private enum GorgonState { Idle, Patrol, Chase, Attack }
    private Animator animator;

    void Start()
    {
        targetPoint = pointA.position;
        animator = GetComponent<Animator>();
    }

    void Update()
    {
        switch (currentState)
        {
            case GorgonState.Patrol:
                Patrol();
                if (PlayerInRange(detectionRange))
                    currentState = GorgonState.Chase;
                break;

            case GorgonState.Chase:
                ChasePlayer();
                // if (PlayerInRange(attackRange))
                //     currentState = GorgonState.Attack;
                // else if (!PlayerInRange(detectionRange))
                //     currentState = GorgonState.Patrol;
                break;

            // case GorgonState.Attack:
            //     AttackPlayer();
            //     if (!PlayerInRange(attackRange))
            //         currentState = GorgonState.Chase;
            //     break;

            case GorgonState.Idle:
                // Could add idle behavior (waiting, animation, etc.)
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

    // === IDLE ===
    void Idle()
    {
        animator.SetBool("isIdle", true);
    }

    // === PATROL ===
    void Patrol()
    {
        animator.SetBool("isWalking", true);

        transform.position = Vector2.MoveTowards(transform.position, targetPoint, speed * Time.deltaTime);

        if (Vector2.Distance(transform.position, targetPoint) < 0.1f)
        {
            targetPoint = targetPoint == pointA.position ? pointB.position : pointA.position;
            Flip();
        }

        // Prevent walking off ledges
        if (!IsGroundAhead())
        {
            targetPoint = targetPoint == pointA.position ? pointB.position : pointA.position;
            Flip();
        }
    }

    // === CHASE PLAYER ===
    void ChasePlayer()
    {
        // animator.SetBool("isWalking", true);
        if (player != null)
        {
            transform.position = Vector2.MoveTowards(transform.position, player.position, speed * Time.deltaTime);

            if ((player.position.x < transform.position.x && transform.localScale.x > 0) ||
                (player.position.x > transform.position.x && transform.localScale.x < 0))
            {
                Flip();
            }
        }
    }

    // // === ATTACK PLAYER ===
    // void AttackPlayer()
    // {
    //     animator.SetBool("isAttacking", true);
    //     Debug.Log("Gorgon attacks!");
    //     // Here you can trigger an animation or reduce the player's health
    // }

    // === HELPERS ===
    bool PlayerInRange(float range)
    {
        return player != null && Vector2.Distance(transform.position, player.position) <= range;
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
