using System;
using UnityEngine;

public class PlayerCombatController : MonoBehaviour
{
    [SerializeField] private Transform combatPivot;
    [SerializeField] private AttackHitBox horizontalHitbox;
    [SerializeField] private float attiveTime = 0.3f;

    private int attackDirection;
    private float nextAttackAllowTime = -100f;
    private bool isAttacking;
    private float attackEndTime;

    public void FrameUpdate(int facingDirection)
    {
        if (isAttacking)
        {
            if (Time.time >= attackEndTime || attackDirection != facingDirection)
                EndAttack();
        }
    }
    public bool CanAttack(bool stateAllowAttack)
    {
        if (!stateAllowAttack)
            return false;

        if (isAttacking)
            return false;

        if (Time.time < nextAttackAllowTime)
            return false;

        return true;
    }
    public void TryAttack(bool stateAllowAttack, int facingDirection)
    {
        if (!CanAttack(stateAllowAttack))
            return;

        StartAttack(facingDirection);
    }
    private void StartAttack(int facingDirection)
    {
        UpdateCombatPivot(facingDirection);
        attackDirection = facingDirection;

        isAttacking = true;

        attackEndTime = Time.time + attiveTime;
        nextAttackAllowTime = Time.time + attiveTime;

        horizontalHitbox.Activate();
    }

    private void EndAttack()
    {
        horizontalHitbox.Deactivate();

        isAttacking = false;
    }
    private void UpdateCombatPivot(int facingDirection)
    {
        Vector3 scale = combatPivot.localScale;
        scale.x = Mathf.Abs(scale.x) * facingDirection;
        combatPivot.localScale = scale;
    }
}
