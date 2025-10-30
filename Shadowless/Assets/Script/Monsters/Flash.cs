using UnityEngine;

public class Flash : MonoBehaviour
{
    private SpriteRenderer spriteRenderer;
    public Color flashColorSelect;
    public float flashTimer = 0.25f;

    private bool isFlashing = false;
    private float timer;
    private int flashAmount = Shader.PropertyToID("_FlashAmount");
    private int flashColor = Shader.PropertyToID("_FlashColor");

    void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    void Update()
    {
        if (isFlashing)
        {
            timer += Time.deltaTime;

            float lerpedAmount = Mathf.Lerp(1f, 0f, timer / flashTimer);
            spriteRenderer.material.SetFloat(flashAmount, lerpedAmount);

            if (timer >= flashTimer)
            {
                isFlashing = false;
                timer = 0f;
            }
        }
    }

    public void HitFlash()
    {
        isFlashing = true;
        timer = 0f;

        spriteRenderer.material.SetFloat(flashAmount, 1f);
        spriteRenderer.material.SetColor(flashColor, flashColorSelect);
    }
}
