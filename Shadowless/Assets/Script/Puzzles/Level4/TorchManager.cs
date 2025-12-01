using UnityEngine;
using TMPro;

public class TorchManager : MonoBehaviour
{
    public Torch[] torches; // drag them in Inspector
    public int[] correctOrder = { 0, 1, 2, 3 }; // Matches the visual order (Blue, Pink, Yellow, Green)
    private int currentStep = 0;
    private System.Collections.Generic.List<GameObject> placedTorches = new System.Collections.Generic.List<GameObject>();

    public GameObject crystal;
    public Mover doorMover; // The script component on the DoorSprite
    public TMP_Text textWrong;
    public Transform[] torchSpawnPoints = new Transform[4]; // Array to hold the 4 distinct spawn points
    public bool TorchActivated(Torch droppedTorch, GameObject droppedTorchObject) // <-- CHANGE RETURN TYPE TO BOOL
    {
        Debug.Log($"[TorchManager] Torch {droppedTorch.torchID} dropped. Current step: {currentStep}."); 

        if (droppedTorch.torchID == correctOrder[currentStep])
        {
            // SUCCESS!
            Debug.Log($"[TorchManager] Correct ID: {droppedTorch.torchID}. Advancing to step {currentStep + 1}.");
            droppedTorch.LightTorch();
            currentStep++;

            placedTorches.Add(droppedTorchObject); 
            
            if (currentStep >= correctOrder.Length)
            {
                PuzzleSolved();
            }
            return true;
        }
        else
        {
            // FAILURE!
            Debug.Log($"[TorchManager] WRONG ID: {droppedTorch.torchID}. Expected: {correctOrder[currentStep]}. Resetting.");
            ResetPuzzle();
            
            // **CRITICAL CHANGE:**
            return false; // <-- Tell the target NOT to absorb the torch
        }
    }


    void ResetPuzzle()
    {
        Debug.Log("[TorchManager] Resetting Puzzle. All torches unlit.");
        currentStep = 0;

        foreach (GameObject placedTorch in placedTorches)
        {
            if (placedTorch != null)
            {
                // Get the Torch component to find its ID
                Torch torchComponent = placedTorch.GetComponent<Torch>();

                if (torchComponent != null && torchComponent.torchID >= 0 && torchComponent.torchID < torchSpawnPoints.Length && torchSpawnPoints[torchComponent.torchID] != null)
                {
                    // Use the torch's ID (0, 1, 2, or 3) to select the corresponding Transform
                    Vector3 spawnPosition = torchSpawnPoints[torchComponent.torchID].position;
                    
                    // CRITICAL FIX: Reset position and parent
                    placedTorch.transform.parent = null; // Detach from player/crystal
                    placedTorch.transform.position = spawnPosition; // Move to the dedicated spot
                    
                    // Re-enable the object
                    placedTorch.SetActive(true); 
                    
                    Debug.Log($"[TorchManager] Restoring consumed torch: {placedTorch.name} (ID: {torchComponent.torchID}) at its dedicated spawn point."); 
                }
                else 
                {
                    // Fallback (in case the ID is wrong or the spawn point isn't assigned)
                    placedTorch.SetActive(true);
                    Debug.LogError($"[TorchManager] Failed to find spawn point for {placedTorch.name} (ID: {torchComponent?.torchID}). Re-enabled at original position.");
                }
            }
        }
        placedTorches.Clear();

  
        foreach (var t in torches)
        {
            t.ResetTorch();
            Debug.Log($"[TorchManager] Resetting Torch ID: {t.torchID}. "); 
        }
        
        if (textWrong != null)
        {
            textWrong.text = "FLAMES REJECTED! Start the sequence anew.";
            textWrong.color = Color.red; 
            Invoke("ClearTextWrong", 2f); 
        }
    }
    
    void ClearTextWrong() 
    {
        if (textWrong != null)
        {
            textWrong.text = "";
        }
    }

  void PuzzleSolved()
    {        
        // 1. Crystal Activation/Glow (Visual change)
        if (crystal != null)
        {
            crystal.GetComponent<SpriteRenderer>().color = Color.cyan; 
        }
        
        // 2. Open the door
        if (doorMover != null)
        {
            doorMover.TriggerMove();
        }
        
        // 3. Show Success Feedback (and clear it after 4 seconds)
        if (textWrong != null) 
        {
            textWrong.text = "SEQUENCE COMPLETE! The ancient seal is broken.";
            textWrong.color = Color.green;
            // Invoke the same clearing method, perhaps for a slightly longer duration (e.g., 4s)
            Invoke("ClearTextWrong", 4f); 
        }
    }
}