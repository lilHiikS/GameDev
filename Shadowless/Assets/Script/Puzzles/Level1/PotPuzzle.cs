using System.Collections;
using TMPro;
using UnityEngine;

public class PotPuzzle : MonoBehaviour, IInteractable
{
    public TextMeshProUGUI textDisplay;
    public DialogueData dialogueData;
    public float typeSpeed = 0.05f;
    public GameObject dialogueBox;
    public Pot pot;

    public ParticleSystem spitting;

    private int currentStep = -1;
    private string[] correctOrder;
    public bool waitingForIngredient = false;
    private bool puzzleStarted = false;

    void Start()
    {
        correctOrder = dialogueData.entrys[2].lines;
    }
    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player") && !puzzleStarted)
        {
            dialogueBox.SetActive(true);
        }
    }

    void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            dialogueBox.SetActive(false);
        }
    }

    public void Interact()
    {
        if (currentStep == -1)
            puzzleStarted = true;
        StartCoroutine(StartRiddle());
    }

    IEnumerator StartRiddle()
    {
        dialogueBox.SetActive(false);
        textDisplay.text = "";
        yield return StartCoroutine(TypeText(dialogueData.entrys[0].lines[0])); // Riddle
        currentStep = 0;
        ShowHint();
    }

    void ShowHint()
    {
        if (currentStep < correctOrder.Length)
        {
            StopAllCoroutines();
            StartCoroutine(TypeText(dialogueData.entrys[1].lines[currentStep]));
            waitingForIngredient = true;
        }
        else
        {
            StartCoroutine(TypeText("You did it! The pot is satisfied."));
            // Puzzle complete logic here
        }
    }

    public void SubmitIngredient(string itemName)
    {
        if (itemName == correctOrder[currentStep])
        {
            currentStep++;
            waitingForIngredient = false;
            ShowHint();
        }
        else
        {
            waitingForIngredient = false;
            StartCoroutine(TypeText("The pot spits it out! Try again from the start."));
            currentStep = 0;
            pot.SpitItems();
            spitting.Play();
            Invoke(nameof(ShowHint), 2f); // Show first hint again after 2 seconds
        }
    }

    IEnumerator TypeText(string line)
    {
        textDisplay.text = "";
        foreach (char c in line)
        {
            textDisplay.text += c;
            yield return new WaitForSeconds(typeSpeed);
        }
    }
}
