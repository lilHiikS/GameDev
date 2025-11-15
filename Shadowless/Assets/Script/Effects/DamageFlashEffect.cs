using System.Collections;
using UnityEngine;

/// <summary>
/// Add visual effects to sprites when hit (flash, shake, etc.)
/// Can be used on any damageable object
/// </summary>
public class DamageFlashEffect : MonoBehaviour
{
    public SpriteRenderer spriteRenderer;
    public Material flashMaterial; // White flash material
    private Material originalMaterial;
    
    [Header("Flash Settings")]
    public Color flashColor = Color.white;
    public float flashDuration = 0.1f;

    void Start()
    {
        if (spriteRenderer == null)
            spriteRenderer = GetComponent<SpriteRenderer>();
            
        if (spriteRenderer != null)
            originalMaterial = spriteRenderer.material;
    }

    public void Flash()
    {
        StartCoroutine(FlashRoutine());
    }

    IEnumerator FlashRoutine()
    {
        if (spriteRenderer == null) yield break;

        // Method 1: Material swap (if you have a flash material)
        if (flashMaterial != null)
        {
            spriteRenderer.material = flashMaterial;
            yield return new WaitForSeconds(flashDuration);
            spriteRenderer.material = originalMaterial;
        }
        // Method 2: Color tint
        else
        {
            Color originalColor = spriteRenderer.color;
            spriteRenderer.color = flashColor;
            yield return new WaitForSeconds(flashDuration);
            spriteRenderer.color = originalColor;
        }
    }

    public IEnumerator FadeOut(float duration)
    {
        if (spriteRenderer == null) yield break;

        Color color = spriteRenderer.color;
        float elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            color.a = Mathf.Lerp(1f, 0f, elapsed / duration);
            spriteRenderer.color = color;
            yield return null;
        }
    }
}
