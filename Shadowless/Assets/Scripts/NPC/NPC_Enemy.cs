using UnityEngine;

public class EnemyAI : MonoBehaviour
{
    // === General Settings ===
    public float speed = 2f;
    public float detectionRange = 5f;
    public float attackRange = 1f;
    public Transform pointA;
    public Transform pointB;
    public Transform player;
    public Transform groundCheck;
    public LayerMask groundLayer;

    private Vector3 targetPoint;
    private EnemyState currentState = EnemyState.Patrol;

    private enum EnemyState { Idle, Patrol, Chase, Attack }

    void Start()
    {
        targetPoint = pointA.position;
    }

    void Update()
    {
        switch (currentState)
        {
            case EnemyState.Patrol:
                Patrol();
                if (PlayerInRange(detectionRange))
                    currentState = EnemyState.Chase;
                break;

            case EnemyState.Chase:
                ChasePlayer();
                if (PlayerInRange(attackRange))
                    currentState = EnemyState.Attack;
                else if (!PlayerInRange(detectionRange))
                    currentState = EnemyState.Patrol;
                break;

            case EnemyState.Attack:
                AttackPlayer();
                if (!PlayerInRange(attackRange))
                    currentState = EnemyState.Chase;
                break;

            case EnemyState.Idle:
                // Could add idle behavior (waiting, animation, etc.)
                break;
        }
    }

    // === PATROL ===
    void Patrol()
    {
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

    // === ATTACK PLAYER ===
    void AttackPlayer()
    {
        Debug.Log("Enemy attacks!");
        // Here you can trigger an animation or reduce the player's health
    }

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
