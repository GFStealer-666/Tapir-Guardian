using UnityEngine;

/// Drives the enemy Animator from mover velocity + AI/health events.
/// Expected Animator params:
///   Bool    IsWalking
///   Trigger Attack
///   Trigger Damaged
///   Trigger Die
[DisallowMultipleComponent]
[RequireComponent(typeof(Animator))]
public class EnemyAnimator2D : MonoBehaviour
{
    [Header("Refs")]
    [SerializeField] private Animator animator;
    [SerializeField] private LinearMover mover;                // optional; reads CurrentVelocity
    [SerializeField] private Rigidbody2D rb2d;                 // fallback if mover is null
    [SerializeField] private StateMachine sm;                  // optional, for Walk/Idle hint
    [SerializeField] private EnemyRoot root;                   // holds IHealth, IMover2D, etc.
    [SerializeField] private MeleeAttackBehaviour melee;       // to catch OnAttackStarted
    [SerializeField] private SpriteRenderer spriteRenderer;    // for flipX

    [Header("Parameters (names)")]
    [SerializeField] private string walkBool = "IsWalking";
    [SerializeField] private string attackTrig = "Attack";
    [SerializeField] private string damagedTrig = "Damaged";
    [SerializeField] private string dieTrig = "Die";

    [Header("Motion → Walk")]
    [SerializeField, Range(0.0f, 0.2f)] private float movingThreshold = 0.02f;
    [SerializeField] private bool flipByVelocityX = true;

    // hashes
    int hWalk, hAtk, hDmg, hDie;

    void Reset()
    {
        animator = GetComponentInChildren<Animator>();
        rb2d     = GetComponent<Rigidbody2D>();
        sm       = GetComponent<StateMachine>();
        root     = GetComponent<EnemyRoot>();
        melee    = GetComponent<MeleeAttackBehaviour>();
        if (!spriteRenderer) spriteRenderer = GetComponentInChildren<SpriteRenderer>(true);
    }

    void Awake()
    {
        if (!animator) animator = GetComponentInChildren<Animator>();
        if (!rb2d)     rb2d     = GetComponent<Rigidbody2D>();
        if (!root)     root     = GetComponent<EnemyRoot>();
        if (!spriteRenderer) spriteRenderer = GetComponentInChildren<SpriteRenderer>(true);

        hWalk = Animator.StringToHash(walkBool);
        hAtk  = Animator.StringToHash(attackTrig);
        hDmg  = Animator.StringToHash(damagedTrig);
        hDie  = Animator.StringToHash(dieTrig);

        if (sm) sm.OnStateChanged += HandleStateChanged;


        // attack wind-up → trigger
        if (melee) melee.OnAttackStarted += () => animator.SetTrigger(hAtk);

        // health → damaged/die
        if (root?.health is HealthComponent hc)
        {
            hc.OnHealthChanged += (_, __) => animator.SetTrigger(hDmg);
            hc.OnDied += HandleDied;
        }
    }

    void OnDestroy()
    {
        if (sm) sm.OnStateChanged -= HandleStateChanged;
        if (root?.health is HealthComponent hc)
        {
            hc.OnDied -= HandleDied;
        }
    }

    void Update()
    {
        // Poll velocity (works even if you didn't add the event)
        Vector2 v = mover ? mover.CurrentVelocity : (rb2d ? rb2d.linearVelocity : Vector2.zero);
        bool moving = v.sqrMagnitude > (movingThreshold * movingThreshold);
        animator.SetBool(hWalk, moving);

        if (flipByVelocityX && spriteRenderer)
        {
            if (v.x >  movingThreshold) spriteRenderer.flipX = false;
            if (v.x < -movingThreshold) spriteRenderer.flipX = true;
        }
    }

    void HandleStateChanged(string s)
    {
        // Patrol/Chase considered "walking", others not. Polling still refines this each frame.
        bool walking = s == "Patrol" || s == "Chase";
        animator.SetBool(hWalk, walking);
    }

    void HandleDied()
    {
        animator.ResetTrigger(hDmg);
        animator.SetTrigger(hDie);

        // stop AI & movement
        var ctrl = GetComponent<EnemyController>();
        if (ctrl) ctrl.enabled = false;
        if (root?.Mover != null) root.Mover.Move(Vector2.zero);

        // make colliders pass-through (or disable them)
        foreach (var c in GetComponentsInChildren<Collider2D>(true))
        {
            // choose ONE:
            c.isTrigger = true;
            // c.enabled = false;
        }
    }
}
