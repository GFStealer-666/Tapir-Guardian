using UnityEngine;

/// <summary>
/// Drives Animator parameters for a 2D player.
/// Reads velocity from IMover2D (or Rigidbody2D fallback),
/// flips SpriteRenderer, and exposes methods you can call from gameplay (WeaponDriver, Health, etc.)
/// Animator parameters expected:
/// - Float: Speed, MoveX, MoveY
/// - Bool : IsMoving, IsBlocking, IsDead
/// - Trigger: Attack, Ranged, Hurt, Speak
/// </summary>
[DisallowMultipleComponent]
[RequireComponent(typeof(Animator))]
public class PlayerAnimator2D : MonoBehaviour
{
    [Header("Optional Gameplay Refs (drag if you have them)")]
    [SerializeField] private LinearMover linearMover;   
    [SerializeField] private WeaponDriver weaponDriver;    
    [SerializeField] private HealthComponent healthComponent;  // IHealth (optional: for OnDamaged/OnDied)

    [Header("Visual")]
    [SerializeField] private SpriteRenderer spriteRenderer;
    [SerializeField] private bool flipByMoveX = true; // set false if you flip by aim instead
    [SerializeField] private Transform aimOrCursorWorld; // optional: e.g., mouse world pointer

    [Header("Tuning")]
    [SerializeField, Range(0.01f, 1.0f)] private float moveDamp = 0.15f;
    [SerializeField] private float movingThreshold = 0.02f;
    [Header("Facing Control")]
    [SerializeField] private bool faceByVelocity = false;     // TURN THIS OFF to face by input/aim
    [SerializeField, Range(0f, 0.5f)] private float faceDeadzone = 0.12f; // hysteresis around zero
    private bool _suppressFacing;

    // Animator param hashes
    static readonly int SpeedHash     = Animator.StringToHash("Speed");
    static readonly int MoveXHash     = Animator.StringToHash("MoveX");
    static readonly int MoveYHash     = Animator.StringToHash("MoveY");
    static readonly int IsMovingHash  = Animator.StringToHash("IsMoving");
    static readonly int IsBlockingHash= Animator.StringToHash("IsBlocking");
    static readonly int IsDeadHash    = Animator.StringToHash("IsDead");
    static readonly int AttackHash    = Animator.StringToHash("Attack");
    static readonly int RangedHash    = Animator.StringToHash("Ranged");

    private Animator anim;
    private Rigidbody2D rb2d;

    private Vector2 velSmoothed;
    private int faceDir = +1; 
    
    void Awake()
    {
        anim = GetComponent<Animator>();
        rb2d = GetComponent<Rigidbody2D>();

        if (!spriteRenderer) spriteRenderer = GetComponentInChildren<SpriteRenderer>(true);
    }

    void OnEnable()
    {
        if (weaponDriver)
        {
            weaponDriver.OnMeleeStarted  += HandleMelee;
            weaponDriver.OnRangedStarted += HandleRanged;
            weaponDriver.OnMeleeStarted  += () => _suppressFacing = true;
            weaponDriver.OnMeleeImpact   += () => _suppressFacing = false;
            weaponDriver.OnRangedStarted += () => _suppressFacing = true;
            weaponDriver.OnRangedFired   += () => _suppressFacing = false;
        }
        if (healthComponent != null) healthComponent.OnDied += HandleDied;
    }

    void OnDisable()
    {
        if (weaponDriver)
        {
            weaponDriver.OnMeleeStarted  -= HandleMelee;
            weaponDriver.OnRangedStarted -= HandleRanged;
            weaponDriver.OnMeleeStarted  -= () => _suppressFacing = true;  // use named handlers if you enable these
            weaponDriver.OnMeleeImpact   -= () => _suppressFacing = false;
            weaponDriver.OnRangedStarted -= () => _suppressFacing = true;
            weaponDriver.OnRangedFired   -= () => _suppressFacing = false;
        }
        if (healthComponent != null) healthComponent.OnDied -= HandleDied;
    }

    void Update()
    {
        // velocity → anim floats (ok)
        Vector2 v = Vector2.zero;
        if (linearMover != null) v = linearMover.CurrentVelocity;
        else if (rb2d != null)   v = rb2d.linearVelocity;

        velSmoothed = Vector2.Lerp(velSmoothed, v, 1f - Mathf.Exp(-Time.deltaTime / Mathf.Max(0.0001f, moveDamp)));
        float speed = velSmoothed.magnitude;
        bool isMoving = speed > movingThreshold;

        anim.SetFloat(SpeedHash, speed);
        anim.SetFloat(MoveXHash, velSmoothed.x);
        anim.SetFloat(MoveYHash, velSmoothed.y);
        anim.SetBool(IsMovingHash, isMoving);

        if (healthComponent != null) anim.SetBool(IsDeadHash, healthComponent.CurrentHealth <= 0);

        UpdateFacing(velSmoothed);   // now respects faceByVelocity & hysteresis
    }

    private void UpdateFacing(Vector2 v)
    {
        if (!spriteRenderer || _suppressFacing) return;

        if (!faceByVelocity && aimOrCursorWorld != null)
        {
            float dx = aimOrCursorWorld.position.x - transform.position.x;
            if (Mathf.Abs(dx) > faceDeadzone) faceDir = dx >= 0 ? +1 : -1;
        }
        else if (faceByVelocity)
        {
            // Hysteresis: only change when beyond deadzone
            if (v.x > faceDeadzone) faceDir = +1;
            if (v.x < -faceDeadzone) faceDir = -1;
        }

        spriteRenderer.flipX = (faceDir < 0);
    }
    
    public void SetFacingByInput(float x)
    {
        if (Mathf.Abs(x) > faceDeadzone)
            faceDir = (x >= 0f) ? +1 : -1;
        spriteRenderer.flipX = (faceDir < 0);
    }

    // === Public API (call these from gameplay) ===
    public void TriggerMelee()  => anim.SetTrigger(AttackHash);
    public void TriggerRanged() => anim.SetTrigger(RangedHash);
    public void SetBlocking(bool on) => anim.SetBool(IsBlockingHash, on);
    public void SetDead(bool dead)   => anim.SetBool(IsDeadHash, dead);

    // === Event handlers (if wired) ===
    private void HandleMelee()  => TriggerMelee();
    private void HandleRanged() => TriggerRanged();
    private void HandleDied()  => SetDead(true);
    public int FacingSign => faceDir; // +1 right, -1 left
}
