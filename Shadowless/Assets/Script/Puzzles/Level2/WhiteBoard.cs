// ...existing code...
using UnityEngine;

[ExecuteAlways]
public class WhiteBoard : MonoBehaviour
{
    public int width = 512;
    public int height = 512;
    public Color backgroundColor = Color.black;
    public Color drawColor = Color.white;
    public int brushSize = 4;

    // control texture -> world scaling without forcing transform scale
    public float pixelsPerUnit = 100f;

    private Texture2D texture;
    private SpriteRenderer spriteRenderer;
    private Sprite createdSprite;

    // cache previous values so we only rebuild when something meaningful changed at runtime
    private int prevWidth;
    private int prevHeight;
    private Color prevBackground;
    private float prevPPU;

    // Deferred rebuild flag to avoid doing editor-bound operations inside OnValidate
    private bool needsRebuild = false;

    void Start()
    {
        EnsureRenderer();
        Rebuild();
    }

    void OnValidate()
    {
        // Called in editor when inspector values change
        EnsureRenderer();

        // schedule a rebuild instead of running it immediately (avoids Editor SendMessage errors)
        needsRebuild = true;
    }

    void Update()
    {
        // In edit mode: perform scheduled rebuild once outside OnValidate context
        if (!Application.isPlaying)
        {
            if (needsRebuild)
            {
                needsRebuild = false;
                Rebuild();
            }
            return;
        }

        // If properties are changed at runtime via script, detect and rebuild
        if (width != prevWidth || height != prevHeight || backgroundColor != prevBackground || pixelsPerUnit != prevPPU)
        {
            Rebuild();
        }
    }

    void EnsureRenderer()
    {
        if (spriteRenderer == null)
            spriteRenderer = GetComponent<SpriteRenderer>();
    }

    void Rebuild()
    {
        if (width <= 0) width = 1;
        if (height <= 0) height = 1;
        EnsureRenderer();

        // preserve designer scale
        Vector3 originalScale = transform.localScale;

        // cleanup previous assets
        Cleanup();

        // create texture
        texture = new Texture2D(width, height, TextureFormat.RGBA32, false);
        texture.filterMode = FilterMode.Point;

        Color[] fill = new Color[width * height];
        for (int i = 0; i < fill.Length; i++)
            fill[i] = backgroundColor;
        texture.SetPixels(fill);
        texture.Apply();

        // create sprite
        createdSprite = Sprite.Create(texture, new Rect(0, 0, width, height), new Vector2(0.5f, 0.5f), pixelsPerUnit);
        if (spriteRenderer != null)
            spriteRenderer.sprite = createdSprite;

        // restore designer scale
        transform.localScale = originalScale;

        // cache current values
        prevWidth = width;
        prevHeight = height;
        prevBackground = backgroundColor;
        prevPPU = pixelsPerUnit;

        // initial test draw
        DrawCircle(width / 2, height / 2);
    }

    void Cleanup()
    {
        if (spriteRenderer != null && spriteRenderer.sprite == createdSprite)
            spriteRenderer.sprite = null;

#if UNITY_EDITOR
        if (createdSprite != null)
        {
            DestroyImmediate(createdSprite);
            createdSprite = null;
        }
        if (texture != null)
        {
            DestroyImmediate(texture);
            texture = null;
        }
#else
        if (createdSprite != null)
        {
            Destroy(createdSprite);
            createdSprite = null;
        }
        if (texture != null)
        {
            Destroy(texture);
            texture = null;
        }
#endif
    }

    void OnDestroy()
    {
        Cleanup();
    }

    // Public helper to clear the board
    public void ClearBoard()
    {
        if (texture == null) return;
        Color[] fill = new Color[width * height];
        for (int i = 0; i < fill.Length; i++)
            fill[i] = backgroundColor;
        texture.SetPixels(fill);
        texture.Apply();
    }

    void DrawCircle(int cx, int cy)
    {
        if (texture == null) return;

        for (int x = -brushSize; x <= brushSize; x++)
        {
            for (int y = -brushSize; y <= brushSize; y++)
            {
                if (x * x + y * y <= brushSize * brushSize)
                {
                    int px = cx + x;
                    int py = cy + y;
                    if (px >= 0 && px < width && py >= 0 && py < height)
                        texture.SetPixel(px, py, drawColor);
                }
            }
        }
        texture.Apply();
    }
}
// ...existing code...