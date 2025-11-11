using UnityEngine;

public class AnimEventHandler : MonoBehaviour
{
    private PlayerAttack playerAttack;

    void Start()
    {
        playerAttack = GetComponentInParent<PlayerAttack>();
    }

    public void OnAttack()
    {
        playerAttack.PerformAttack();
    }

    public void OnEndAttack()
    {
        playerAttack.EndAttack();
    }

    public void OnEnableCombo()
    {
        playerAttack.canCombo = true;
    }
}
