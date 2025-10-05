using UnityEngine;

public class MainMenuController : MonoBehaviour
{
    public float moveSpeed = 2f;
    public Transform startPoint;
    public Transform endPoint;
    private Animator animator;
    private SpriteRenderer sr;
    private Transform target;
    private bool movingToEnd = true;
    private bool waiting = false;
    private float waitTimer = 0f;
    private float waitDuration = 0f;

    void Start()
    {
        animator = GetComponent<Animator>();
        sr = GetComponent<SpriteRenderer>();
        animator.SetBool("isRunning", true);

        transform.position = startPoint.position;
        target = endPoint;
        sr.flipX = false;
    }

    void Update()
    {
        if (waiting)
        {
            waitTimer += Time.deltaTime;
            if (waitTimer >= waitDuration)
            {
                waiting = false;
                animator.SetBool("isRunning", true);
            }
            else
            {
                return; // Don't move while waiting
            }
        }

        // Move towards the target
        transform.position = Vector3.MoveTowards(transform.position, target.position, moveSpeed * Time.deltaTime);

        // Check if reached the target
        if (Vector3.Distance(transform.position, target.position) < 0.01f && !waiting)
        {
            movingToEnd = !movingToEnd;
            target = movingToEnd ? endPoint : startPoint;
            sr.flipX = !movingToEnd;

            // Start random wait
            waiting = true;
            waitTimer = 0f;
            waitDuration = Random.Range(3f, 6f); // Wait between 3 and 6 seconds
            animator.SetBool("isRunning", false);
        }
    }
}