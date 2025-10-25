using UnityEngine;
using UnityEngine.UI;

public class PlayerHealth : MonoBehaviour
{
    public int maxHealth = 5;
    public int currentHealth;
    public Animator animator;
    public bool isDead = false;
    public GameObject healthParent;
    public Image[] heartObjects;
    public Sprite heart;
    public Sprite emptyHeart;

    void Start()
    {
        currentHealth = maxHealth;

        heartObjects = new Image[healthParent.transform.childCount];
        for (int i = 0; i < healthParent.transform.childCount; i++)
        {
            heartObjects[i] = healthParent.transform.GetChild(i).GetComponent<Image>();
        }

        UpdateHealthDisplay();
    }

    void UpdateHealthDisplay()
    {
        for (int i = 0; i < heartObjects.Length; i++)
        {
            if (i < currentHealth)
            {
                heartObjects[i].sprite = heart;
            }
            else
            {
                heartObjects[i].sprite = emptyHeart;
            }
        }
    }

    public void TakeDamage(int damage)
    {
        currentHealth -= damage;
        UpdateHealthDisplay();
        if (currentHealth <= 0 && !isDead)
        {
            isDead = true;
            Die();
        }
    }

    private void Die()
    {
        animator.SetTrigger("Die");
    }

    public void ReviveAnimation()
    {
        animator.ResetTrigger("Die");
        animator.Play("Idle");
    }

    public void ReviveStats()
    {
        isDead = false;
        currentHealth = maxHealth;
        UpdateHealthDisplay();
    }

    public void Heal(int amount)
    {
        currentHealth += amount;
        if (currentHealth > maxHealth)
        {
            currentHealth = maxHealth;
        }
    }

    public int GetCurrentHealth()
    {
        return currentHealth;
    }
}