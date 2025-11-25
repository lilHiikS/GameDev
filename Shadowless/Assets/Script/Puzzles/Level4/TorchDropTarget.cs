// TorchDropTarget.cs - FINAL CORRECT VERSION

using UnityEngine;

public class TorchDropTarget : MonoBehaviour
{
    public TorchManager torchManager;

    void Start()
    {
        // Auto-assign the manager
        if (torchManager == null)
        {
            torchManager = FindObjectOfType<TorchManager>();
        }
    }

// In TorchDropTarget.cs

void OnTriggerEnter2D(Collider2D collision)
{
    Torch torch = collision.GetComponent<Torch>(); 

    if (torch != null)
    {
        // OLD: if (!torch.isLit) <--- REMOVE THIS LINE ENTIRELY
        // The check is now unnecessary because:
        // 1. If correct, the torch object is consumed (disabled).
        // 2. If wrong, the torch object is rejected (spit out).
        // The player should not be able to drop the same torch onto the crystal twice.
        
        // 1. Ask the manager if this torch is correct.
        bool success = torchManager.TorchActivated(torch, collision.gameObject); 
        
        if (success)
        {
            // 2. If TRUE, the torch was correct. ABSORB IT.
            collision.gameObject.SetActive(false); 
            Debug.Log($"[TorchDropTarget] Torch ID {torch.torchID} correctly placed and consumed.");
        }
        else
        {
            // 3. If FALSE, the torch was wrong. DO NOT ABSORB IT.
            Debug.Log($"[TorchDropTarget] Incorrect placement. Torch ID {torch.torchID} rejected.");
        }
    }
}
// In TorchDropTarget.cs

}