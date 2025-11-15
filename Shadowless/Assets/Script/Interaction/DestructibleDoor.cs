using UnityEngine;

/// <summary>
/// Destructible door that takes damage from player attacks
/// Attach to door GameObject with a Collider2D
/// </summary>
public class DestructibleDoor : MonoBehaviour, IDamageable
{
    public int maxHealth = 3; // How many hits to break the door
    private int currentHealth;

    [Header("Visual Feedback")]
    public Sprite[] damageSprites; // Array of sprites showing progressive damage (0 = pristine, last = very damaged)
    public SpriteRenderer spriteRenderer;
    private DamageFlashEffect flashEffect;
    
    [Header("Effects")]
    public ParticleSystem breakEffect; // Optional: particle effect when destroyed
    public AudioClip breakSound; // Optional: sound when destroyed
    public AudioClip hitSound; // Optional: sound when hit

    void Start()
    {
        currentHealth = maxHealth;
        
        if (spriteRenderer == null)
            spriteRenderer = GetComponent<SpriteRenderer>();
            
        flashEffect = GetComponent<DamageFlashEffect>();
    }

    public void TakeDamage(int damage)
    {
        currentHealth -= damage;
        Debug.Log($"Door took {damage} damage! Health: {currentHealth}/{maxHealth}");

        // Optional: Play hit sound
        if (hitSound != null && currentHealth > 0)
        {
            AudioSource.PlayClipAtPoint(hitSound, transform.position);
        }

        // Update sprite to show damage
        UpdateDamageSprite();

        // Optional: Add flash effect
        if (flashEffect != null)
            flashEffect.Flash();

        // Optional: Add hit shake effect
        StartCoroutine(ShakeDoor());

        if (currentHealth <= 0)
        {
            BreakDoor();
        }
    }

    void UpdateDamageSprite()
    {
        if (damageSprites.Length == 0 || spriteRenderer == null) return;

        // Calculate which sprite to show based on health percentage
        float healthPercent = (float)currentHealth / maxHealth;
        int spriteIndex = Mathf.Clamp(damageSprites.Length - Mathf.CeilToInt(healthPercent * damageSprites.Length), 0, damageSprites.Length - 1);
        
        spriteRenderer.sprite = damageSprites[spriteIndex];
    }

    System.Collections.IEnumerator ShakeDoor()
    {
        Vector3 originalPosition = transform.position;
        float shakeDuration = 0.2f;
        float shakeMagnitude = 0.1f;
        float elapsed = 0f;

        while (elapsed < shakeDuration)
        {
            float x = originalPosition.x + Random.Range(-shakeMagnitude, shakeMagnitude);
            float y = originalPosition.y + Random.Range(-shakeMagnitude, shakeMagnitude);

            transform.position = new Vector3(x, y, originalPosition.z);
            elapsed += Time.deltaTime;
            yield return null;
        }

        transform.position = originalPosition;
    }

    void BreakDoor()
    {
        Debug.Log("Door destroyed!");

        // Optional: Play break effect
        if (breakEffect != null)
        {
            Instantiate(breakEffect, transform.position, Quaternion.identity);
        }

        // Optional: Play break sound
        if (breakSound != null)
        {
            AudioSource.PlayClipAtPoint(breakSound, transform.position);
        }

        // Destroy the door
        Destroy(gameObject);
    }
}
