using System;
using System.Collections.Generic;
using UnityEngine;

[DisallowMultipleComponent]
public class WeaponDriver : MonoBehaviour
{
    // ========= External dependencies (via interfaces for DIP) =========
    [Header("Dependencies (Interfaces)")]
    [Tooltip("Any MonoBehaviour that implements IWeaponSelector (e.g., WeaponSelectorController).")]
    [SerializeField] private WeaponSelectorController selectorBehaviour;   // IWeaponSelector
    [Tooltip("Any MonoBehaviour that implements IEquipmentProvider (e.g., EquipmentProviderAdapter).")]
    [SerializeField] private EquipmentProviderAdapter equipmentProvider;   // IEquipmentProvider

    private IWeaponSelector _selector;
    private IEquipmentProvider _equip;

    // ========= Data & Refs =========
    [Header("Refs")]
    [SerializeField] private PlayerAnimator2D anim2D;
    [SerializeField] private Animator animator;
    [SerializeField] private PlayerAudioSoundEffect sfx;

    [SerializeField] private Transform meleeOrigin;
    [SerializeField] private Transform firePointRight;
    [SerializeField] private Transform firePointLeft;

    [Header("Inventory / Ammo")]
    [SerializeField] private InventoryComponent inventory;
    [SerializeField, Min(1)] private int bulletsPerShot = 1;

    [Header("Animator (optional)")]
    [SerializeField] private bool driveAnimator = true;
    [SerializeField] private string meleeTrigger = "Attack";
    [SerializeField] private string rangedTrigger = "Shoot";

    [Header("Driver Timing")]
    [Tooltip("Max time to wait for an animation event before auto-acting.")]
    [SerializeField] private float eventTimeout = 1.5f;
    [Tooltip("If the player presses attack during Windup/Recover, buffer for this long.")]
    [SerializeField] private float bufferWindow = 0.2f;
    [Tooltip("Short lockout after dry fire to avoid spamming.")]
    [SerializeField] private float dryFireLockout = 0.2f;

    // ========= Public events (keep your existing names) =========
    public event Action OnMeleeStarted;
    public event Action OnMeleeImpact;
    public event Action<WeaponSO> OnRangedStarted;
    public event Action<WeaponSO> OnRangedFired;

    // ========= Internal state machine =========
    private enum DriverState { Idle, Windup, Act, Recover }
    private DriverState _state = DriverState.Idle;

    private WeaponSO _activeWeapon;      // weapon currently executing
    private float _stateEndsAt;          // timer for state end
    private bool _awaitingAnimEvent;     // waiting for Anim_* event?

    // Latches set by animation events
    private bool _meleeImpactEvent;
    private bool _rangedFireEvent;

    // Simple input buffer (one request)
    private float _bufferedUntil;
    private WeaponSO _bufferedWeapon;

    // Per-weapon cooldowns
    private readonly Dictionary<WeaponSO, float> _readyAt = new Dictionary<WeaponSO, float>();
    private float GetReadyAt(WeaponSO w) => _readyAt.TryGetValue(w, out var t) ? t : 0f;
    private void  SetReadyAfter(WeaponSO w, float cd) => _readyAt[w] = Time.time + Mathf.Max(0f, cd);

    // ========= Unity lifecycle =========
    private void Awake()
    {
        if (!inventory) inventory = GetComponentInParent<InventoryComponent>();
        if (!animator) animator = GetComponentInChildren<Animator>();
        
        _selector = selectorBehaviour;
        _equip = equipmentProvider;
        
        if (_selector == null) Debug.LogError("[WeaponDriver] selectorBehaviour must implement IWeaponSelector.");
        if (_equip == null)     Debug.LogError("[WeaponDriver] equipmentProvider must implement IEquipmentProvider.");
    }

    private void Update()
    {
        switch (_state)
        {
            case DriverState.Idle:
                // Auto-start buffered request if still valid
                if (_bufferedWeapon && Time.time <= _bufferedUntil && CanStart(_bufferedWeapon))
                {
                    StartWeapon(_bufferedWeapon);
                    ClearBuffer();
                }
                break;

            case DriverState.Windup:
                if (_awaitingAnimEvent)
                {
                    if (ShouldActFromEvent(_activeWeapon))
                    {
                        EnterAct();
                    }
                    else if (Time.time >= _stateEndsAt) // timeout fallback
                    {
                        EnterAct();
                    }
                }
                else
                {
                    if (Time.time >= _stateEndsAt)
                        EnterAct();
                }
                break;

            case DriverState.Act:
                // Act is instantaneous here → proceed to Recover
                EnterRecover();
                break;

            case DriverState.Recover:
                if (Time.time >= _stateEndsAt)
                    EnterIdle();
                break;
        }
    }

    // ========= Public API (call this from your brain when ShootPressed) =========
    public void OnAttackInput()
    {
        var w = _selector != null ? _selector.SelectedWeapon : null;
        if (w == null) return;
        Request(w);
    }

    // ========= Animation event hooks =========
    public void Anim_MeleeImpact() => _meleeImpactEvent = true;
    public void Anim_RangedFire()  => _rangedFireEvent  = true;

    // ========= Request / Buffering =========
    private bool Request(WeaponSO w)
    {
        if (!w) return false;

        if (_state == DriverState.Idle && CanStart(w))
        {
            StartWeapon(w);
            return true;
        }

        // Otherwise, buffer one request
        _bufferedWeapon = w;
        _bufferedUntil  = Time.time + bufferWindow;
        return false;
    }

    private void ClearBuffer()
    {
        _bufferedWeapon = null;
        _bufferedUntil  = 0f;
    }

    private bool CanStart(WeaponSO w)
    {
        if (!w) return false;
        if (Time.time < GetReadyAt(w)) return false;

        // For guns, ensure ammo before committing to windup
        if (w.kind == WeaponKind.Gun)
        {
            var gun = (WeaponGunSO)w;
            if (!HasAmmo(gun, bulletsPerShot))
                return false;
        }
        return true;
    }

    private void StartWeapon(WeaponSO w)
    {
        _activeWeapon = w;
        _meleeImpactEvent = _rangedFireEvent = false;

        switch (w.kind)
        {
            case WeaponKind.Melee:
                OnMeleeStarted?.Invoke();
                if (driveAnimator && animator && !string.IsNullOrEmpty(meleeTrigger))
                    animator.SetTrigger(meleeTrigger);
                BeginWindup(((WeaponMeleeSO)w).useAnimationEvent, ((WeaponMeleeSO)w).windUp);
                break;

            case WeaponKind.Gun:
                OnRangedStarted?.Invoke(_activeWeapon);
                // if (driveAnimator && animator && !string.IsNullOrEmpty(rangedTrigger))
                //     animator.SetTrigger(rangedTrigger);
                BeginWindup(((WeaponGunSO)w).useAnimationEvent, ((WeaponGunSO)w).windUp);
                break;
        }
    }

    private void BeginWindup(bool useAnimEvent, float windUpSeconds)
    {
        _state = DriverState.Windup;
        _awaitingAnimEvent = useAnimEvent;
        _stateEndsAt = Time.time + (useAnimEvent ? Mathf.Max(0.05f, eventTimeout)
                                                 : Mathf.Max(0f, windUpSeconds));
    }

    private bool ShouldActFromEvent(WeaponSO w)
    {
        if (w.kind == WeaponKind.Melee) return _meleeImpactEvent;
        if (w.kind == WeaponKind.Gun)   return _rangedFireEvent;
        return false;
    }

    private void EnterAct()
    {
        _state = DriverState.Act;

        switch (_activeWeapon.kind)
        {
            case WeaponKind.Melee:
                if (sfx) sfx.PlayWeaponSound(_activeWeapon.weaponSound);
                DoMelee((WeaponMeleeSO)_activeWeapon);
                OnMeleeImpact?.Invoke();
                
                break;

            case WeaponKind.Gun:
            {
                var gun = (WeaponGunSO)_activeWeapon;

                // Final ammo check & consume (handles external inventory edits)
                if (!ConsumeAmmo(gun, bulletsPerShot))
                {
                    SetReadyAfter(_activeWeapon, Mathf.Max(0.1f, dryFireLockout));
                    _stateEndsAt = Time.time + dryFireLockout; // short recover to release state
                    return;
                }

                DoShoot(gun);
                OnRangedFired?.Invoke(_activeWeapon);
                break;
            }
        }
    }

    private void EnterRecover()
    {
        _state = DriverState.Recover;
        switch (_activeWeapon.kind)
        {
            case WeaponKind.Melee:
                SetReadyAfter(_activeWeapon, ((WeaponMeleeSO)_activeWeapon).attackCooldown);
                _stateEndsAt = Time.time + Mathf.Max(0f, ((WeaponMeleeSO)_activeWeapon).attackCooldown);
                break;

            case WeaponKind.Gun:
                SetReadyAfter(_activeWeapon, ((WeaponGunSO)_activeWeapon).shootCooldown);
                _stateEndsAt = Time.time + Mathf.Max(0f, ((WeaponGunSO)_activeWeapon).shootCooldown);
                break;
        }
    }

    private void EnterIdle()
    {
        _state = DriverState.Idle;
        _activeWeapon = null;
        _awaitingAnimEvent = false;

        // Kick buffered request immediately if still valid
        if (_bufferedWeapon && Time.time <= _bufferedUntil && CanStart(_bufferedWeapon))
        {
            StartWeapon(_bufferedWeapon);
            ClearBuffer();
        }
        else
        {
            ClearBuffer();
        }
    }

    // ========= Ammo / SFX helpers =========
    private bool HasAmmo(WeaponGunSO w, int need = 1)
    {
        if (!w || !w.ammoType) return true; // no ammo required
        if (!inventory) return false;
        return inventory.Has(w.ammoType.Id, need);
    }

    private bool ConsumeAmmo(WeaponGunSO w, int amount = 1)
    {
        if (!w || !w.ammoType) return true; // infinite ammo case
        if (!inventory) return false;
        return inventory.Consume(w.ammoType.Id, amount);
    }


    // ========= Actions =========
    private void DoMelee(WeaponMeleeSO w)
    {
        Transform t = meleeOrigin ? meleeOrigin : transform;
        int sign = anim2D ? anim2D.FacingSign : 1;

        Vector2 local = w.swingLocalOffset;
        local.x *= sign;
        Vector2 origin = (Vector2)t.TransformPoint(local);

        var hits = Physics2D.OverlapCircleAll(origin, w.swingRadius, w.hitMask);
        if (hits == null || hits.Length == 0)
            return;

        foreach (var h in hits)
        {
            var receiver = h.GetComponentInChildren<DamageReceiver>();
            if (!receiver) continue;

           

            var data = new DamageData(w.damage, DamageType.Melee, gameObject, true);
            receiver.ReceiveDamage(in data);
        }
    }

    private void DoShoot(WeaponGunSO w)
    {
        if (!w.bulletPrefab) return;

        int sign = anim2D ? anim2D.FacingSign : 1;
        Vector2 dir = (sign >= 0) ? Vector2.right : Vector2.left;
        Transform muzzle = (sign >= 0) ? firePointRight : firePointLeft;
        if (!muzzle) muzzle = transform;

        int count = Mathf.Max(1, bulletsPerShot);
        for (int i = 0; i < count; i++)
        {
            var go = Instantiate(w.bulletPrefab, muzzle.position, Quaternion.identity);
            var b  = go.GetComponent<Bullet>();

            if (b != null)
            {
                b.Configure(
                    damage:     w.damage,
                    velocity:   dir * w.bulletSpeed,
                    source:     gameObject,
                    customLife: -1f,
                    mask:       w.hitMask,
                    type:       w.damageType // forward weapon’s damage type
                );
            }
            else
            {
                var rb = go.GetComponent<Rigidbody2D>();
                if (rb) rb.linearVelocity = dir * w.bulletSpeed;
            }
        }

        if (sfx) sfx.PlayWeaponSound(w.weaponSound);
    }

#if UNITY_EDITOR
    private void OnDrawGizmosSelected()
    {
        // Draw melee gizmo for currently selected main-hand if it's melee (Editor convenience)
        var m = _equip != null ? _equip.Get(EquipSlot.SideHand) as WeaponMeleeSO : null;
        var mMain = _equip != null ? _equip.Get(EquipSlot.MainHand) as WeaponMeleeSO : null;

        // Prefer the actually selected one if we have a selector at runtime
        WeaponMeleeSO target = null;
        if (_selector != null)
        {
            var sel = _selector.SelectedWeapon as WeaponMeleeSO;
            if (sel) target = sel;
        }
        if (target == null) target = mMain ? mMain : m;

        if (target)
        {
            Transform t = meleeOrigin ? meleeOrigin : transform;
            int sign = anim2D ? anim2D.FacingSign : 1;
            Vector2 local = target.swingLocalOffset; local.x *= sign;
            Vector2 origin = (Vector2)t.TransformPoint(local);

            Gizmos.color = new Color(1f, 0.3f, 0.2f, 0.35f);
            const int seg = 28;
            Vector3 prev = origin + Vector2.right * target.swingRadius;
            for (int i = 1; i <= seg; i++)
            {
                float a = i * Mathf.PI * 2f / seg;
                Vector3 next = origin + new Vector2(Mathf.Cos(a), Mathf.Sin(a)) * target.swingRadius;
                Gizmos.DrawLine(prev, next);
                prev = next;
            }
        }
    }
#endif
}
