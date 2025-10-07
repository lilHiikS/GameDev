using UnityEngine;

public class GorgonAI : MonoBehaviour
{
    public float y;
    public float speed = 2f;
    public float detectionRange = 20f;
    public float attackRange = 0.5f;
    public Transform pointA;
    public Transform pointB;
    public Transform player;
    public Transform groundCheck;
    public LayerMask groundLayer;

    private Vector3 targetPoint;
    private GorgonState currentState = GorgonState.Chase;

    private enum GorgonState { Idle, Walk, Chase, Attack }
    public Animator animator;

    void Start()
    {
        targetPoint = pointA.position;
        animator = GetComponent<Animator>();
        y = transform.position.y;

    }

    void Update()
    {
        switch (currentState)
        {
            case GorgonState.Walk:
                Patrol();
                if (PlayerInRange(detectionRange))
                    currentState = GorgonState.Chase;
                break;

            case GorgonState.Chase:
                ChasePlayer();
                if (PlayerInRange(attackRange))
                    currentState = GorgonState.Attack;
                break;

            case GorgonState.Attack:
                AttackPlayer();
                if (!PlayerInRange(attackRange))
                    currentState = GorgonState.Chase;
                break;

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

    void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            player = null;
            currentState = GorgonState.Walk;
        }
    }

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

    void ChasePlayer()
    {
        if (player != null)
        {
            // transform.position = Vector2.MoveTowards(transform.position, player.position, speed * Time.deltaTime);
            // transform.position = new Vector2(transform.position.x, transform.position.y); //her - kig

            Vector2 targetPosition = new Vector2(player.position.x, y);
            transform.position = Vector2.MoveTowards(transform.position, targetPosition, speed * Time.deltaTime);


            if ((player.position.x < transform.position.x && transform.localScale.x > 0) ||
                (player.position.x > transform.position.x && transform.localScale.x < 0))
            {
                Flip();
            }
        }
    }

    void AttackPlayer()
    {
        animator.SetTrigger("attack");
        Debug.Log("Gorgon attacks!");
        // Here you can trigger an animation or reduce the player's health
    }

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
