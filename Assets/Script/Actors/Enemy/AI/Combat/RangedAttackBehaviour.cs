using UnityEngine;
using System;

[DisallowMultipleComponent]
public class RangedAttackBehaviour : MonoBehaviour,
    IEnemyAttack, IAttackRangeProvider, IAttackOriginProvider
{
    [Header("Origin (Gun muzzle)")]
    [SerializeField] private Transform fireOrigin;
    [SerializeField] private Vector2 localOffset;

    [Header("Ranged")]
    [SerializeField] private Bullet bulletPrefab;
    [SerializeField] private int attack = 8;
    [SerializeField] private float fireRange = 7.5f;
    [SerializeField] private float bulletSpeed = 36f;
    [SerializeField] private float fireCooldown = 0.6f;
    [SerializeField] private LayerMask targetMask;

    [Header("Timing")]
    [SerializeField] private HitTiming fireTiming = HitTiming.AnimationEvent; // use AnimationEvent

    [Header("Animation (optional)")]
    [SerializeField] private Animator animator;
    [SerializeField] private string attackTrigger = "Attack";
    [SerializeField] private bool autoTriggerAnimator = true;

    [Header("SFX")]
    [SerializeField] private AudioSource sfxSource;   // optional, will use GetComponent<AudioSource>() if null
    [SerializeField] private AudioClip[] fireSfx = new AudioClip[2];
    [Range(0f,1f)] [SerializeField] private float fireSfxVolume = 1f;

    public float AttackRange => fireRange;
    public event Action OnAttackStarted;
    private int _lastSfx = -1;

    private float nextAt;
    private bool isFiring;           // latched during the clip
    private Transform latchedTarget; // target snapshot when attack starts

    void Awake()
    {
        if (!animator)   animator   = GetComponentInChildren<Animator>();
        if (!sfxSource)  sfxSource  = GetComponent<AudioSource>();
    }

    public Vector2 GetOrigin(Transform self)
    {
        var t = fireOrigin ? fireOrigin : self;
        return (Vector2)t.TransformPoint((Vector3)localOffset);
    }

    public void ApplyConfig(int attack, float fireRange, float bulletSpeed, float fireCooldown, LayerMask hitMask)
    {
        this.attack       = attack;
        this.fireRange    = fireRange;
        this.bulletSpeed  = bulletSpeed;
        this.fireCooldown = fireCooldown;
        this.targetMask   = hitMask;
    }

    public bool TryAttack(Transform self, Transform target)
    {
        if (!target || !bulletPrefab) return false;
        if (Time.time < nextAt) return false;

        // range check
        Vector2 origin = GetOrigin(self);
        float dist = ((Vector2)target.position - origin).magnitude;
        if (dist > fireRange) return false;

        // face target (optional)
        GetComponent<EnemyFacing2D>()?.FaceByTargetX(target.position.x);

        // one cooldown per whole clip (burst = one attack)
        nextAt = Time.time + fireCooldown;

        if (fireTiming == HitTiming.Instant)
        {
            // single immediate shot (rarely used if you rely on animation)
            Vector2 dir = ((Vector2)target.position - origin).normalized;
            FireNow(origin, dir);
            PlayFireSfx();
            if (autoTriggerAnimator && animator && !string.IsNullOrEmpty(attackTrigger))
                animator.SetTrigger(attackTrigger);
            OnAttackStarted?.Invoke();
        }
        else
        {
            // animation-driven burst: events will call AnimEvent_Fire multiple times
            isFiring = true;
            latchedTarget = target;

            if (autoTriggerAnimator && animator && !string.IsNullOrEmpty(attackTrigger))
                animator.SetTrigger(attackTrigger);

            OnAttackStarted?.Invoke();
        }

        return true;
    }

    // === Animation Events ===
    // Place this on every muzzle frame you want a bullet (each event = one bullet + sound)
    public void AnimEvent_Fire()
    {
        if (!isFiring) return;

        Vector2 origin = GetOrigin(transform);

        Vector2 dir;
        if (latchedTarget)
            dir = ((Vector2)latchedTarget.position - origin);
        else
            dir = (Vector2)transform.right; // fallback

        if (dir.sqrMagnitude < 1e-6f) dir = Vector2.right;
        FireNow(origin, dir.normalized);
        PlayFireSfx();
    }

    // Call once at the end of the clip
    public void AnimEvent_FireEnd()
    {
        isFiring = false;
        latchedTarget = null;
    }

    // === Internals ===
    private void FireNow(Vector2 origin, Vector2 dir)
    {
        var b = Instantiate(bulletPrefab, origin, Quaternion.identity);
        b.Configure(attack, dir * bulletSpeed, gameObject, 3.0f, targetMask);
    }

    private void PlayFireSfx()
    {
        if (fireSfx == null || fireSfx.Length == 0) return;
        int idx;
        if (fireSfx.Length == 1)
        {
            idx = 0;
        }
        else
        {
            do { idx = UnityEngine.Random.Range(0, fireSfx.Length); }
            while (idx == _lastSfx);
            _lastSfx = idx;
        }

        var clip = fireSfx[idx];
        if (!clip) return;

        var src = sfxSource ? sfxSource : GetComponent<AudioSource>();
        if (src)
        {
            src.PlayOneShot(clip, fireSfxVolume);
        }
        else
        {
            AudioSource.PlayClipAtPoint(clip, transform.position, fireSfxVolume);
        }
        Debug.Log("Playings");
    }
}
