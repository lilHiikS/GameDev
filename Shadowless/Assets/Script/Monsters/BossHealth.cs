using UnityEngine;

public class BossHealth : MonoBehaviour, IDamageable
{
    public int maxHealth = 5;
    public float currentHealth;
    public Animator animator;
    public Flash flash;
    private Knockback knockback;

    void Start()
    {
        currentHealth = maxHealth;
        knockback = GetComponent<Knockback>();
    }

    public void TakeDamage(int damage)
    {
        currentHealth -= damage;
        //knockback.ApplyKnockback((transform.position.x - SceneManager.Instance.player.transform.position.x) > 0 ? Vector2.right : Vector2.left);
        flash.HitFlash();
        if (currentHealth <= 0)
        {
            GetComponent<Rigidbody2D>().excludeLayers = LayerMask.GetMask("Player", "Enemy");
            gameObject.layer = LayerMask.NameToLayer("Default");
            var rb = GetComponent<Rigidbody2D>();
            rb.linearVelocity = new Vector2(0f, rb.linearVelocity.y);
            animator.SetBool("death", true);
        }
    }

    public void OnDestroy()
    {
        Destroy(gameObject);
    }
}