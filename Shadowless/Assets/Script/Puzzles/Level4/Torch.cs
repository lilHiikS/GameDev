using UnityEngine;

public class Torch : MonoBehaviour
{
    public int torchID; // order number
    public Sprite unlitSprite;
    public Sprite litSprite;
    public bool isLit = false;
    public TorchManager torchManager;

    private SpriteRenderer sr;

    void Start()
    {
        sr = GetComponent<SpriteRenderer>();
        sr.sprite = unlitSprite;
    }

    void OnMouseDown()
    {
        // Only works if this GameObject has a Collider2D
        Debug.Log("Torch clicked: " + torchID);

        if (!isLit)
        {
            LightTorch();
        }
    }

    public void LightTorch()
    {
        isLit = true;
        sr.sprite = litSprite;
        torchManager.TorchActivated(torchID);
    }

    public void ResetTorch()
    {
        isLit = false;
        sr.sprite = unlitSprite;
    }
}
