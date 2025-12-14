using System;
using UnityEngine;
using UnityEngine.UI;

public class BringerHealth : MonoBehaviour, IDamageable
{
    public int maxHealth = 5;
    public float currentHealth;
    public Animator animator;
    public Flash flash;
    public Slider healthBar;
    public GameObject jail;
    public Bringer bringer;

    void Start()
    {
        currentHealth = maxHealth;
        healthBar.maxValue = 1f;
        healthBar.value = 1f;
    }

    public void TakeDamage(int damage)
    {
        currentHealth -= damage;
        flash.HitFlash();
        healthBar.value = currentHealth / maxHealth;
        if (currentHealth <= 0)
        {
            healthBar.value = 0;
            var rb = GetComponent<Rigidbody2D>();
            rb.linearVelocity = new Vector2(0f, rb.linearVelocity.y);
            animator.SetBool("dead", true);
            jail.SetActive(false);
        }
    }

    public void OnDestroy()
    {
        Destroy(gameObject);
    }
}