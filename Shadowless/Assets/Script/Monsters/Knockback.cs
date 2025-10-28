using UnityEngine;

public class Knockback : MonoBehaviour
{
    public float knockbackForce = 50f;
    public float knockbackDuration = 0.25f;

    public Rigidbody2D rb;
    public bool isKnockedBack = false;
    private float knockbackTimer = 0f;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        if (isKnockedBack)
        {
            knockbackTimer += Time.deltaTime;

            if (knockbackTimer >= knockbackDuration)
            {
                rb.linearVelocity = new Vector2(0f, rb.linearVelocity.y);
                rb.angularVelocity = 0f;
                isKnockedBack = false;
            }
        }
    }

    public void ApplyKnockback(Vector2 direction)
    {
        isKnockedBack = true;
        knockbackTimer = 0f;
        rb.AddForce(direction * knockbackForce, ForceMode2D.Impulse);
    }
}