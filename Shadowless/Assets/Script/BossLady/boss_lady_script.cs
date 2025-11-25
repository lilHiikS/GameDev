using UnityEngine;
using TMPro;
using System.Collections;
using System;
using Script.BossLady.attacks;
using Script.BossLady;


public class boss_lady_script : MonoBehaviour, IInteractable
{
    [SerializeField] internal BossState currentState = BossState.Idle;

    [SerializeField] private TextMeshProUGUI dialogText; // Træk din UI-tekst herind i Inspector
    
    [SerializeField] private TextMeshProUGUI eToskip;

    [SerializeField] private Animator animator;

    [SerializeField] private float lineDelay = 3f; // tid mellem linjer

    [SerializeField] private BossMovement bossMovement;
    
    private PlayerHealth playerHealth;

    private BossHealth bossHealth;

    private BossShooter shooter;

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

    internal enum BossState
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
    
    private bool isShowingDialog = false;
    private bool skipDialog = false;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        bossMovement = GetComponent<BossMovement>();
        bossHealth = GetComponent<BossHealth>();
        shooter = GetComponent<BossShooter>();
        GameObject p = GameObject.FindGameObjectWithTag("Player");
        if (p != null)
            playerHealth = p.GetComponent<PlayerHealth>();
        else
            Debug.LogError("Boss kan ikke finde spilleren!");
        StartCoroutine(StartSequence());
    }

    // Update is called once per frame
    void Update()
    {

    }

    void StartAttackPhase()
    {
        // CancelInvoke(nameof(AttackCycle));
        // InvokeRepeating(nameof(AttackCycle), 1.5f, 1.5f);

        animator.SetBool("isAttacking", true);
    }
    void AttackCycle()
    {
        if (bossHealth.currentHealth <= 0)
        {
            CancelInvoke(nameof(AttackCycle));
            Debug.Log("BOSS ER DØD! Boss HP: " + bossHealth.currentHealth);
            return;
        }

        if (playerHealth == null || playerHealth.isDead)
        {
            CancelInvoke(nameof(AttackCycle));
            Debug.Log("SPILLER ER DØD! Stopper bossens angreb");
            Idle();
            return;
        }
        // Hvis begge lever, så angrib
        if (currentState == BossState.Attack)
        {
            FacePlayer();
            animator.SetBool("isIdle", false);
            animator.SetBool("isAttacking", true);
            shooter.Shoot();
        }
        else
        {
            animator.SetBool("isAttacking", false);
        }

    }

    void Idle()
    {
        currentState = BossState.Idle;
        animator.SetBool("isIdle", true);
        animator.SetBool("isAttacking", false);
        animator.SetBool("isRunning", false);
        animator.SetBool("isWalking", false);

        // Stop alle Invoke kald bare for sikkerhed
        CancelInvoke(nameof(AttackCycle));

        Debug.Log("Bossen står nu stille (Idle).");
    }

    IEnumerator StartSequence()
    {
        PlayerController2D.Instance.interactable = this;
        
        isShowingDialog = true; // dialogen er aktiv
        skipDialog = false;     // ingen skip endnu
        
        // Vis dialogtekst
        dialogText.gameObject.SetActive(true);
        eToskip.gameObject.SetActive(true);
        bossMovement.enabled = false;
        
        yield return StartCoroutine(ShowDialogLines(bigText));
        // Skjul dialog
        yield return new WaitForSeconds(1f);
        
        dialogText.gameObject.SetActive(false);
        eToskip.gameObject.SetActive(false);

        isShowingDialog = false;
        
        PlayerController2D.Instance.interactable = null; // Tag interaktion væk efter dialogen


        bossHealth.canTakeDamage = true;

        bossMovement.enabled = true;
        
        // Start angreb
        currentState = BossState.Attack;
        animator.SetBool("isAttacking", true); // Sørg for at din Animator har denne bool
        
        StartAttackPhase();
    }
    
    IEnumerator ShowDialogLines(string fullText)
    {
        string[] lines = fullText.Split('\n');

        foreach (string line in lines)
        {
            if (skipDialog)
            {
                // Vis sidste linje
                string lastLine = lines[lines.Length - 1].Trim();
                dialogText.text = lastLine;
                
                yield return new WaitForSeconds(1f);
                yield break;
            }
            dialogText.text = line.Trim();
            yield return new WaitForSeconds(lineDelay);
        }
    }

    internal void FacePlayer()
    {
        if (playerHealth != null)
        {
            Vector3 direction = playerHealth.transform.position - transform.position;
            if (direction.x > 0)
            {
                transform.localScale = new Vector3(-7.19446754f,8.0692215f,5.30800009f);
            }
            else
            {
                transform.localScale = new Vector3(7.19446754f,8.0692215f,5.30800009f);
            }
        }
    }

    public void Interact()
    {
        if (isShowingDialog)
        {
            skipDialog = true;
            eToskip.gameObject.SetActive(false);
            Debug.Log("Dialog skippet");
        }
    }
}
