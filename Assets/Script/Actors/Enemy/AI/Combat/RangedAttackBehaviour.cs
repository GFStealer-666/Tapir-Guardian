using UnityEngine;
using System;

[DisallowMultipleComponent]
public class RangedAttackBehaviour : MonoBehaviour, IEnemyAttack, IAttackRangeProvider
{
    [Header("Origin (Gun muzzle)")]
    [SerializeField] private Transform fireOrigin;
    [SerializeField] private Vector2 localOffset;
    [SerializeField] private Bullet bulletPrefab;
    [SerializeField] private int attack = 8;
    [SerializeField] private float fireRange = 7.5f;
    [SerializeField] private float bulletSpeed = 12f;
    [SerializeField] private float fireCooldown = 0.6f;
    [SerializeField] private LayerMask targetMask;
    [Header("Animation (optional)")]
    [SerializeField] private Animator animator;
    [SerializeField] private string attackTrigger = "Attack";
    [SerializeField] private bool autoTriggerAnimator = true;
    public float AttackRange => fireRange;
    private float nextAt;

    public event Action OnAttackStarted;
    public void ApplyConfig(int attack, float fireRange, float bulletSpeed, float fireCooldown, LayerMask hitMask)
    {
        this.attack = attack;
        this.fireRange = fireRange;
        this.bulletSpeed = bulletSpeed;
        this.fireCooldown = fireCooldown;
        this.targetMask = hitMask;
    }
    public bool TryAttack(Transform self, Transform target)
    {
        if (!target || !bulletPrefab) return false;
        if (Time.time < nextAt) return false;

        Vector2 origin = GetOrigin(self);
        Vector2 to = target.position - (Vector3)origin;
        float dist = to.magnitude;
        if (dist > fireRange) return false;

        // face target horizontally (if you have EnemyFacing2D)
        GetComponent<EnemyFacing2D>()?.FaceByTargetX(target.position.x);

        to.Normalize();
        nextAt = Time.time + fireCooldown;

        // spawn bullet
        var b = Instantiate(bulletPrefab, origin, Quaternion.identity);
        b.Configure(attack, to * bulletSpeed, gameObject, 3.0f, targetMask);

        // animation
        if (autoTriggerAnimator && animator && !string.IsNullOrEmpty(attackTrigger))
            animator.SetTrigger(attackTrigger);

        OnAttackStarted?.Invoke();
        return true;
    }
    public Vector2 GetOrigin(Transform self)
    {
        var t = fireOrigin ? fireOrigin : self;
        return (Vector2)t.TransformPoint((Vector3)localOffset);
    }
}
