using UnityEngine;
using TMPro;

public class TorchManager : MonoBehaviour
{
    public Torch[] torches; // drag them in Inspector
    public int[] correctOrder = { 0, 1, 2, 3 }; // Matches the visual order (Blue, Pink, Yellow, Green)
    private int currentStep = 0;

    public GameObject crystal;
    public Mover doorMover; // The script component on the DoorSprite
    public TMP_Text textWrong;

   public void TorchActivated(Torch droppedTorch)
{
    Debug.Log($"[TorchManager] Torch {droppedTorch.torchID} dropped. Current step: {currentStep}."); 

    if (droppedTorch.torchID == correctOrder[currentStep])
    {
        // SUCCESS!
        droppedTorch.LightTorch(); // Now we light the torch if it was the correct one
        currentStep++;

        if (currentStep >= correctOrder.Length)
        {
            PuzzleSolved();
        }
    }
    else
    {
        // FAILURE!
        droppedTorch.gameObject.SetActive(true); 
        ResetPuzzle();
    }
}

    void ResetPuzzle()
    {
        currentStep = 0;
        foreach (var t in torches)
        {
            t.ResetTorch();
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