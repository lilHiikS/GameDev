using System.Collections;
using TMPro;
using UnityEngine;

public class PotPuzzle : MonoBehaviour, IInteractable
{
    public TextMeshProUGUI textDisplay;
    public DialogueData dialogueData;
    public float typeSpeed = 0.04f;
    public GameObject dialogueBox;
    public Pot pot;

    public ParticleSystem spitting;

    private int currentStep = -1;
    private string[] correctOrder;
    public bool waitingForIngredient = false;
    private bool puzzleStarted = false;

    public float spitPostDelay = 2f;  // delay after message before hint
    public float spitPreDelay = 0.2f; // delay before showing spit message

    public GameObject exitPortal;

    // NEW: single typing coroutine handle
    private Coroutine typingCoroutine;

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
        // ensure previous typing is stopped
        StopTyping();
        StartTyping(dialogueData.entrys[0].lines[0]); // Riddle
        currentStep = 0;
        // wait until riddle finishes typing (optional) or just show hint immediately:
        // yield return typingCoroutine; // if you need to wait
        ShowHint();
        yield return null;
    }

    void ShowHint()
    {
        if (currentStep < correctOrder.Length)
        {
            // stop only typing coroutine, not other coroutines like spitting
            StopTyping();
            StartTyping(dialogueData.entrys[1].lines[currentStep]);
            waitingForIngredient = true;
        }
        else
        {
            StopTyping();
            StartTyping("You did it! The pot is satisfied.");
            exitPortal.SetActive(true);
        }
    }

    public void SubmitIngredient(string itemName)
    {
        if (!waitingForIngredient) return; // guard against spam
        waitingForIngredient = false; // consume input immediately to avoid double submits

        if (itemName == correctOrder[currentStep])
        {
            currentStep++;
            ShowHint();
        }
        else
        {
            StartCoroutine(HandleSpitAndMessage());
        }
    }

    // centralized typing start/stop helpers
    private void StartTyping(string line)
    {
        StopTyping();
        typingCoroutine = StartCoroutine(TypeText(line));
    }

    private void StopTyping()
    {
        if (typingCoroutine != null)
        {
            StopCoroutine(typingCoroutine);
            typingCoroutine = null;
        }
    }

    IEnumerator TypeText(string line)
    {
        yield return new WaitForSeconds(spitPreDelay);
        textDisplay.text = "";
        foreach (char c in line)
        {
            textDisplay.text += c;
            yield return new WaitForSeconds(typeSpeed);
        }

        yield return new WaitForSeconds(spitPostDelay);
        typingCoroutine = null;
    }

    private IEnumerator HandleSpitAndMessage()
    {
        StopTyping();
        pot.SpitItems();
        spitting.Play();

        yield return new WaitForSeconds(spitPreDelay);

        StartTyping("The pot spits it out! Try again from the start.");
        currentStep = 0;

        yield return new WaitForSeconds(spitPostDelay);

        ShowHint();
    }
}