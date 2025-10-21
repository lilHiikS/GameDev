using UnityEngine;

public class boss_lady_script : MonoBehaviour
{
    [SerializeField]
    private BossState currentState = BossState.Idle;

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
    [SerializeField]
    private Animator animator;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void idle()
    {
        animator.SetBool("isIdle", true);
    }
}
