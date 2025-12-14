using UnityEngine;

public class SpikeDamager : MonoBehaviour
{
    [Header("Damage Settings")] public int damage = 5; public float hitCooldown = 0.4f;
    [Tooltip("Tag used to identify the player.")] public string playerTag = "Player";

    private float lastHitTime = -999f;

    void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag(playerTag)) return;

        if (Time.time - lastHitTime < hitCooldown) return;

        var damageTarget = other.GetComponent<IDamageable>();
        if (damageTarget != null)
        {
            damageTarget.TakeDamage(damage);
            lastHitTime = Time.time;
            return;
        }

        // Fallbacks: common health component names or SendMessage
        var health = other.GetComponent<PlayerHealth>();
        if (health != null)
        {
            health.TakeDamage(damage);
            lastHitTime = Time.time;
            return;
        }

        // If no known health component found, try SendMessage (optional)
        other.SendMessage("TakeDamage", damage, SendMessageOptions.DontRequireReceiver);
        lastHitTime = Time.time;
    }
}
