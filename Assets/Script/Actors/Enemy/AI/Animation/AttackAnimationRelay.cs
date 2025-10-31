using UnityEngine;

public class AttackAnimationRelay : MonoBehaviour
{
    [SerializeField] private MeleeAttackBehaviour melee;
    void Reset() { melee = GetComponent<MeleeAttackBehaviour>(); }

    // Called by Animation Events
    public void Attack_Hit() => melee?.AnimEvent_AttackHit();
    public void Attack_End() => melee?.AnimEvent_AttackEnd();

    // Optional: death clip end (destroy self, pool return, etc.)
    public void Die_End()
    {
        // Destroy(gameObject); // or return to pool
        var sr = GetComponentInChildren<SpriteRenderer>();
        if (sr) sr.enabled = false;
        // Keep colliders disabled (done on OnDied in the bridge)
        var ctrl = GetComponent<EnemyController>();
        if (ctrl) ctrl.enabled = false;
        // Disable Animator to freeze
        var anim = GetComponentInChildren<Animator>();
        if (anim) anim.enabled = false;
    }
}
