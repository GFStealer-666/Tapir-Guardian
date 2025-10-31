using UnityEngine;
using System;

public enum HitTiming { AnimationEvent, Instant }

[DisallowMultipleComponent]
public class MeleeAttackBehaviour : MonoBehaviour,
    IEnemyAttack, IAttackRangeProvider, IAttackOriginProvider
{
    [Header("Origin (hand/weapon)")]
    [SerializeField] private Transform attackOrigin;
    [SerializeField] private Vector2 localOffset;

    [Header("Melee")]
    [SerializeField] private int attack = 12;
    [SerializeField] private float range = 0.56f;
    [SerializeField] private float cooldown = 0.74f;
    [SerializeField] private LayerMask targetMask = -1;

    [Header("Timing")]
    [SerializeField] private HitTiming hitTiming = HitTiming.AnimationEvent;

    [Header("Animator (optional, auto)")]
    [SerializeField] private Animator animator;                   // if null, finds in children
    [SerializeField] private string attackTrigger = "Attack";
    [SerializeField] private bool autoTriggerAnimator = true;

    private float nextAt;
    private bool isSwinging;
    private Transform latchedTarget;

    public float AttackRange => range;
    public event Action OnAttackStarted;

    void Awake()
    {
        if (!animator) animator = GetComponentInChildren<Animator>();
    }

    public Vector2 GetOrigin(Transform self)
    {
        var t = attackOrigin ? attackOrigin : self;
        return (Vector2)t.TransformPoint((Vector3)localOffset);
    }

    public bool TryAttack(Transform self, Transform target)
    {
        if (!target) { return false; }
        if (Time.time < nextAt) { return false; }

        Vector2 origin   = GetOrigin(self);
        Vector2 hitPoint = target.TryGetComponent<Collider2D>(out var col)
            ? col.ClosestPoint(origin)
            : (Vector2)target.position;

        float dist = Vector2.Distance(origin, hitPoint);
        if (dist > range) { return false; }

        // layer gate
        if (((1 << target.gameObject.layer) & targetMask.value) == 0) { return false; }

        // Start swing / or do instant hit
        nextAt = Time.time + cooldown;

        if (hitTiming == HitTiming.Instant)
        {
            ApplyDamageTo(target, origin);
            // still play an anim if available (looks better)
            if (autoTriggerAnimator && animator && !string.IsNullOrEmpty(attackTrigger))
                animator.SetTrigger(attackTrigger);
            return true;
        }

        // AnimationEvent mode
        isSwinging    = true;
        latchedTarget = target;
        OnAttackStarted?.Invoke();
        if (autoTriggerAnimator && animator && !string.IsNullOrEmpty(attackTrigger))
            animator.SetTrigger(attackTrigger);
        return true;
    }

    // === Call these from animation events on the Attack clip ===
    public void AnimEvent_AttackHit()
    {
        if (!isSwinging || !latchedTarget) return;
        ApplyDamageTo(latchedTarget, GetOrigin(transform));
    }

    public void AnimEvent_AttackEnd()
    {
        isSwinging = false;
        latchedTarget = null;
    }

    // === Internals ===
    private void ApplyDamageTo(Transform target, Vector2 origin)
    {
        Vector2 hitPoint = target.TryGetComponent<Collider2D>(out var col)
            ? col.ClosestPoint(origin)
            : (Vector2)target.position;

        if (Vector2.Distance(origin, hitPoint) > range) return;
        if (((1 << target.gameObject.layer) & targetMask.value) == 0) return;

        IDamageable d =
            target.GetComponent<IDamageable>() ??
            target.GetComponentInChildren<IDamageable>(true) ??
            target.GetComponentInParent<IDamageable>();

        if (d != null)
        {
            var data = new DamageData(attack, DamageType.Melee, gameObject, true);
            d.ReceiveDamage(in data);
            // Debug.Log("Melee: damage applied");
        }
    }

#if UNITY_EDITOR
    void OnDrawGizmosSelected()
    {
        var o = GetOrigin(transform);
        Gizmos.color = new Color(1, .3f, .2f, .35f);
        int seg = 28;
        Vector3 prev = o + new Vector2(range, 0f);
        for (int i = 1; i <= seg; i++)
        {
            float a = i * Mathf.PI * 2f / seg;
            Vector3 next = o + new Vector2(Mathf.Cos(a) * range, Mathf.Sin(a) * range);
            Gizmos.DrawLine(prev, next); prev = next;
        }
        Gizmos.color = new Color(1, .3f, .2f, .9f);
        Gizmos.DrawLine(o, o + (Vector2)transform.right * range);
    }
#endif

    // Optional external config
    public void ApplyConfig(int atk, float rng, float cd, LayerMask mask)
    { attack = atk; range = rng; cooldown = cd; targetMask = mask; }
}
