using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerAttack : MonoBehaviour
{
    public Animator animator;
    public bool isAttacking = false;
    public bool hasWeapon = false;

    public int attackDamage = 1;

    public Transform attackTransform;
    public float attackRange = 0.5f;
    public LayerMask attackableLayer;
    public float timeBetweenAttacks = 0.5f;
    private float attackTimer = 0f;

    public bool canCombo = false;
    public float comboWindow = 0.5f;
    private float comboTimer = 0f;

    private RaycastHit2D[] hits;

    void Start()
    {
        attackTimer = timeBetweenAttacks;
    }

    public void Attack()
    {
        if (!hasWeapon) return;
        if (isAttacking) return;

        if (attackTimer < timeBetweenAttacks) return;

        isAttacking = true;
        attackTimer = 0f;
        animator.SetTrigger("Attack");
    }

    public void PerformAttack()
    {
        hits = Physics2D.CircleCastAll(attackTransform.position, attackRange, Vector2.zero, 0f, attackableLayer);

        foreach (var hit in hits)
        {
            if (hit.collider != null)
            {
                if (hit.collider.gameObject.TryGetComponent<IDamageable>(out var damageable))
                {
                    damageable.TakeDamage(attackDamage); // Deal specified attack damage
                }
            }
        }
    }

    public void EnableCombo()
    {
        canCombo = true;
    }

    public void EndAttack()
    {
        isAttacking = false;
    }

    void Update()
    {
        attackTimer += Time.deltaTime;

        if (canCombo)
        {
            comboTimer += Time.deltaTime;
            if (comboTimer > comboWindow)
            {
                canCombo = false;
                comboTimer = 0f;
            }
        }
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.DrawWireSphere(attackTransform.position, attackRange);
    }
}