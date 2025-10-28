using UnityEngine;
using TMPro;
using System.Collections;
using System;
public class boss_lady_script : MonoBehaviour
{
    [SerializeField] private BossState currentState = BossState.Idle;

    [SerializeField] private TextMeshProUGUI dialogText; // Træk din UI-tekst herind i Inspector

    [SerializeField] private Animator animator;

    [SerializeField] private float lineDelay = 3f; // tid mellem linjer


    private String bigText =
    "Ah, finally! The 'big' hero arrives!\n" +
    "Took you long enough\n" +
    "I was starting to think you got lost in my clearly labeled death traps.\n" +
    "You must be so proud of yourself.\n" +
    "All that jumping, dodging, and button mashing…\n" +
    "for this.\n" +
    "Let me guess...\n" +
    "you’re here to ‘defeat evil’ and ‘save the kingdom’?\n" +
    "How original.\n" +
    "Truly, history will remember you as the nine-hundredth fool to try.\n" +
    "But don´t worry...\n" +
    "I´ll make your end quick.\n" + 
    "…Okay, maybe not that quick.\n" + 
    "I do enjoy a good scream. \n" +
    "Now then — shall we dance, sparkle boy?";

    private enum BossState
    {
        Idle,
        Attack,
        Run,
        Walk,
        Die,
        Jump,
        Fall,
        Hit,
        Dead
    }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        StartCoroutine(StartSequence());
    }

    // Update is called once per frame
    void Update()
    {

    }

    void Idle()
    {
        animator.SetBool("isIdle", true);
    }

    IEnumerator StartSequence()
    {
        // Vis dialogtekst
        dialogText.gameObject.SetActive(true);
        yield return StartCoroutine(ShowDialogLines(bigText));
        // Skjul dialog
        dialogText.gameObject.SetActive(false);

        // Start angreb
        currentState = BossState.Attack;
        animator.SetBool("isAttacking", true); // Sørg for at din Animator har denne bool
    }
    
    IEnumerator ShowDialogLines(string fullText)
    {
        string[] lines = fullText.Split('\n');

        foreach (string line in lines)
        {
            dialogText.text = line.Trim();
            yield return new WaitForSeconds(lineDelay);
        }
    }

}
