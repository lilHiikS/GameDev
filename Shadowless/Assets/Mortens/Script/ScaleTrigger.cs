using UnityEngine;

public class ScaleTrigger : MonoBehaviour
{
    public ScalePuzzle scalePuzzle; // Assign in Inspector

    void OnTriggerEnter2D(Collider2D other)
    {
        var rb = other.attachedRigidbody;
        if (rb != null)
            scalePuzzle.AddLeftObject(rb);
    }

    void OnTriggerExit2D(Collider2D other)
    {
        var rb = other.attachedRigidbody;
        if (rb != null)
            scalePuzzle.RemoveLeftObject(rb);
    }
}