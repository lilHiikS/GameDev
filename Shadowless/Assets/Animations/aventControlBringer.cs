using UnityEngine;

public class aventControlBringer : MonoBehaviour
{
    public Bringer bringer;

    public void EndAttack()
    {
        bringer.OnMeleeAttackEnd();
    }

    public void CheckDmg()
    {
        bringer.CheckDmg();
    }

    public void Destroy()
    {
        Destroy(bringer.gameObject);
    }
}
