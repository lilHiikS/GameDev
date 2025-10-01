using UnityEngine;

public class WeightItem : MonoBehaviour
{
    public ScalePuzzle scalePuzzle;
    public bool isGrounded = false;
    public LayerMask supportMask;

    private Rigidbody2D rb;
    private float checkTimer = 0f;
    private float checkInterval = 0.1f;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    void FixedUpdate()
    {
        checkTimer -= Time.fixedDeltaTime;
        if (checkTimer > 0f || rb.IsSleeping()) return;

        var col = GetComponent<Collider2D>();
        Vector2 origin = new Vector2(col.bounds.center.x, col.bounds.min.y + 0.01f);
        float radius = col.bounds.extents.x * 0.9f;

        Collider2D[] hits = Physics2D.OverlapCircleAll(origin, radius, supportMask);
        bool wasGrounded = isGrounded;
        isGrounded = false;

        foreach (var hit in hits)
        {
            if (hit == col) continue; // Ignore self
            if (hit.CompareTag("Plate"))
            {
                isGrounded = true;
                break;
            }
            else if (hit.CompareTag("Weight"))
            {
                var otherWeight = hit.GetComponent<WeightItem>();
                if (otherWeight != null && otherWeight.isGrounded)
                {
                    isGrounded = true;
                    break;
                }
            }
        }

        if (wasGrounded != isGrounded)
        {
            if (isGrounded)
                scalePuzzle.AddLeftObject(GetComponent<Rigidbody2D>());
            else
                scalePuzzle.RemoveLeftObject(GetComponent<Rigidbody2D>());
        }

        checkTimer = checkInterval;
    }

    void OnDrawGizmos()
    {
        var col = GetComponent<Collider2D>();
        if (col != null)
        {
            Vector2 origin = new Vector2(col.bounds.center.x, col.bounds.min.y + 0.01f);
            float radius = col.bounds.extents.x * 0.9f;
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(origin, radius);
        }
    }
}