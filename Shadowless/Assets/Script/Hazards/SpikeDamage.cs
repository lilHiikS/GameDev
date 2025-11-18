using UnityEngine;

/// <summary>
/// Attach to a Tilemap GameObject with spikes
/// Requires: Tilemap Collider 2D (with Is Trigger checked)
/// </summary>
public class SpikeDamage : MonoBehaviour
{
    public int damage = 1;
    public float damageCooldown = 0.5f; // Cooldown between damage instances
    
    private float lastDamageTime = -999f;

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            // Prevent damage if cooldown hasn't passed
            if (Time.time - lastDamageTime < damageCooldown)
                return;

            PlayerHealth playerHealth = collision.GetComponent<PlayerHealth>();
            if (playerHealth != null)
            {
                playerHealth.TakeDamage(damage);
                lastDamageTime = Time.time;
                Debug.Log($"Spike dealt {damage} damage. Next damage available at: {lastDamageTime + damageCooldown}");
            }
        }
    }
}
