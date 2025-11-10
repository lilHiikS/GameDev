using UnityEngine;

public class TorchManager : MonoBehaviour
{
    public Torch[] torches; // drag them in Inspector
    public int[] correctOrder = { 0, 1, 2, 3 }; // Blue, Purple, Yellow, Green
    private int currentStep = 0;

    public GameObject crystal;
    public GameObject door;

    public void TorchActivated(int torchID)
    {
        if (torchID == correctOrder[currentStep])
        {
            currentStep++;

            if (currentStep >= correctOrder.Length)
            {
                PuzzleSolved();
            }
        }
        else
        {
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
        Debug.Log("Wrong order! Resetting...");
    }

    void PuzzleSolved()
    {
        Debug.Log("Correct sequence! Door unlocked.");
        crystal.GetComponent<SpriteRenderer>().color = Color.cyan; // Glow change
        door.SetActive(false); // Remove or open the door
    }
}
