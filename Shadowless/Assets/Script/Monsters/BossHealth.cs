using System.Collections;
using UnityEngine;

public class BossHealth : MonoBehaviour, IDamageable
{
    public int maxHealth = 5;
    public float currentHealth;
    public Animator animator;
    public Flash flash;
    private Knockback knockback;
    private bool isDead = false;
    public bool canTakeDamage = false;


    void Start()
    {
        currentHealth = maxHealth;
        knockback = GetComponent<Knockback>();
    }

    public void TakeDamage(int damage)
    {
        if (canTakeDamage)
        {
            currentHealth -= damage;
            //knockback.ApplyKnockback((transform.position.x - SceneManager.Instance.player.transform.position.x) > 0 ? Vector2.right : Vector2.left);
            flash.HitFlash();
    
        } 
        if (currentHealth <= 0)
        {
            GetComponent<Rigidbody2D>().excludeLayers = LayerMask.GetMask("Player", "Enemy");
            gameObject.layer = LayerMask.NameToLayer("Default");
            var rb = GetComponent<Rigidbody2D>();
            rb.linearVelocity = new Vector2(0f, rb.linearVelocity.y);
            StartCoroutine(PlayDeathAnimation());
        }
    }

    private IEnumerator PlayDeathAnimation()
    {
        // Start death animation
        animator.Play("death");

        // Vent på at animationen er færdig
        var deathClip = animator.runtimeAnimatorController.animationClips[0]; // antag death er første clip
        yield return new WaitForSeconds(2f);

        // Skift til idle_death
        animator.Play("idle_death");
    }



    public void OnDestroy()
    {
        Destroy(gameObject);
    }
}