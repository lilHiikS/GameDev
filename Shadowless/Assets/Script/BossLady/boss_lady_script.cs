using UnityEngine;
using TMPro;
using System.Collections;
public class boss_lady_script : MonoBehaviour
{
    [SerializeField]
    private BossState currentState = BossState.Idle;
    
    [SerializeField]
    private TextMeshProUGUI dialogText; // Træk din UI-tekst herind i Inspector
    
    [SerializeField]
    private Animator animator;
    
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
        dialogText.text = "You dare challenge me?";
        
        // Vent 3 sekunder
        yield return new WaitForSeconds(3f);

        // Skjul dialog
        dialogText.gameObject.SetActive(false);

        // Start angreb
        currentState = BossState.Attack;
        animator.SetBool("isAttacking", true); // Sørg for at din Animator har denne bool
    }

}
