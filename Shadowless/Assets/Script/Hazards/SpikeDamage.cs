using UnityEngine;

/// <summary>
/// Attach to a Tilemap GameObject with spikes
/// Requires: Tilemap Collider 2D (with Is Trigger checked)
/// </summary>
public class SpikeDamage : MonoBehaviour
{
    public int damage = 1;

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            PlayerHealth playerHealth = collision.GetComponent<PlayerHealth>();
            if (playerHealth != null)
            {
                playerHealth.TakeDamage(damage);
            }
        }
    }
}
