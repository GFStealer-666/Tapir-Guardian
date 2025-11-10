using UnityEngine;

public class AttackAnimationRelay : MonoBehaviour
{
    [SerializeField] private MeleeAttackBehaviour  melee;
    [SerializeField] private RangedAttackBehaviour ranged;

    void Reset()
    {
        melee  = GetComponent<MeleeAttackBehaviour>();
        ranged = GetComponent<RangedAttackBehaviour>();
    }

    // Melee events
    public void Attack_Hit() => melee?.AnimEvent_AttackHit();
    public void Attack_End() => melee?.AnimEvent_AttackEnd();

    // Ranged events (put these on the gun attack clip)
    public void Ranged_Fire()    => ranged?.AnimEvent_Fire();
    public void Ranged_FireEnd() => ranged?.AnimEvent_FireEnd();

    // Optional: death end
    public void Die_End()
    {
        var ctrl = GetComponent<EnemyController>();
        if (ctrl) ctrl.enabled = false;
        var anim = GetComponentInChildren<Animator>();
        if (anim) anim.enabled = false;
    }
}
