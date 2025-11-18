using UnityEngine;

public class Torch : MonoBehaviour
{
    public int torchID; 
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

    public void ResetTorch()
    {
        isLit = false;
        sr.sprite = unlitSprite;
    }

    public void LightTorch()
    {
        isLit = true;
        sr.sprite = litSprite;
    }
}
