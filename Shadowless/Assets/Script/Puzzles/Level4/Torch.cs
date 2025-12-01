using UnityEngine;

public class Torch : MonoBehaviour
{
    public int torchID; 
    public Sprite unlitSprite;
    private SpriteRenderer sr;

    void Start()
    {
        sr = GetComponent<SpriteRenderer>();
        sr.sprite = unlitSprite;
    }

    public void ResetTorch()
    {
        sr.sprite = unlitSprite;
    }

    public void LightTorch()
    {
        sr.sprite = unlitSprite;
    }
}
