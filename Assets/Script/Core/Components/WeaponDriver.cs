using System;
using System.Collections;
using UnityEngine;

[DisallowMultipleComponent]
public class WeaponDriver : MonoBehaviour
{
    [Header("Data")]
    public WeaponSO mainHand;              // assign a Melee or Ranged SO
    public WeaponSO sideHand;              // optional second weapon

    [Header("Refs")]
    [SerializeField] private PlayerAnimator2D anim2D;  
    [SerializeField] private PlayerAudioSoundEffect sfx;
    [SerializeField] private Transform meleeOrigin;   
    [SerializeField] private Transform firePointRight,firePointLeft;
    [Header("Inventory / Ammo")]
    [SerializeField] private InventoryComponent inventory;
    [SerializeField] private AudioClip dryFireSound;           
    [SerializeField, Min(1)] private int bulletsPerShot = 1;  

    public event Action OnMeleeStarted;
    public event Action OnMeleeImpact;
    public event Action OnRangedStarted;
    public event Action OnRangedFired;

    bool _busy;
    float _nextTime;

    // animation-event latches
    bool _meleeImpactEvent;
    bool _rangedFireEvent;

    // replace _nextTime with per-weapon map
    private readonly System.Collections.Generic.Dictionary<WeaponSO, float> _readyAt =
        new System.Collections.Generic.Dictionary<WeaponSO, float>();

    private float GetReadyAt(WeaponSO w) => _readyAt.TryGetValue(w, out var t) ? t : 0f;
    private void  SetReadyAfter(WeaponSO w, float cd) => _readyAt[w] = Time.time + Mathf.Max(0f, cd);
    private void Awake()
    {
        if (!inventory) inventory = GetComponentInParent<InventoryComponent>();
    }
     private bool HasAmmo(WeaponGunSO w, int need = 1)
    {
        if (!w || !w.ammoType) return true;
        if (!inventory) return false;

        // Assuming AmmoSO inherits ItemSO and uses the same `id` string as items
        return inventory.Has(w.ammoType.Id, need);
    }

    private bool ConsumeAmmo(WeaponGunSO w, int amount = 1)
    {
        if (!w || !w.ammoType) return true; // infinite ammo case
        if (!inventory) return false;
        return inventory.Consume(w.ammoType.Id, amount);
    }

    private void PlayDry()
    {
        if (sfx && dryFireSound) sfx.PlaySoundEffect(dryFireSound);
    }
    public bool TryUse(WeaponSO w)
    {
        if (!w) return false;
        if (_busy) return false;
        if (Time.time < GetReadyAt(w)) return false;

        switch (w.kind)
        {
            case WeaponKind.Melee:
                StartCoroutine(MeleeRoutine((WeaponMeleeSO)w));
                return true;

            case WeaponKind.Gun:
            {
                var gun = (WeaponGunSO)w;

                if (!HasAmmo(gun, bulletsPerShot))
                {
                    PlayDry();
                    return false;
                }

                StartCoroutine(RangedRoutine(gun));
                return true;
            }
        }
        return false;
    }

    public void TryUseMain()
    {
        // try main; if it couldn't start (cooldown/busy/no ammo), fall back to side
        if (!TryUse(mainHand))
            TryUse(sideHand);
    }

    public void TryUseSide() => TryUse(sideHand);
    public void Anim_MeleeImpact() => _meleeImpactEvent = true;
    public void Anim_RangedFire()  => _rangedFireEvent  = true;

    // === Coroutines ===
    private IEnumerator MeleeRoutine(WeaponMeleeSO w)
    {
        _busy = true;
        OnMeleeStarted?.Invoke();

        if (w.useAnimationEvent) { _meleeImpactEvent = false; float t=0, timeout=1.5f;
            while(!_meleeImpactEvent && (t+=Time.deltaTime)<timeout) yield return null;
        } else {
            yield return new WaitForSeconds(w.windUp);
        }

        DoMelee(w);
        OnMeleeImpact?.Invoke();

        SetReadyAfter(w, w.attackCooldown);
        _busy = false;
    }

    private IEnumerator RangedRoutine(WeaponGunSO w)
    {
        _busy = true;
        OnRangedStarted?.Invoke();

        if (w.useAnimationEvent) { _rangedFireEvent=false; float t=0, timeout=1.5f;
            while(!_rangedFireEvent && (t+=Time.deltaTime)<timeout) yield return null;
        } else {
            yield return new WaitForSeconds(w.windUp);
        }
        if (!ConsumeAmmo(w, bulletsPerShot))
        {
            PlayDry();
            SetReadyAfter(w, Mathf.Max(0.1f, w.shootCooldown * 0.25f));
            _busy = false;
            yield break;
        }
        DoShoot(w);
        OnRangedFired?.Invoke();

        SetReadyAfter(w, w.shootCooldown);
        _busy = false;
    }

    // === Actions ===
    void DoMelee(WeaponMeleeSO w)
    {
        Debug.Log("WeaponDriver DoMelee");
        Transform t = meleeOrigin ? meleeOrigin : transform;
        // flip-aware origin (so offsets work for left/right)
        int sign = anim2D ? anim2D.FacingSign : 1;
        Vector2 local = w.swingLocalOffset;
        local.x *= sign;
        Vector2 origin = (Vector2)t.TransformPoint(local);

        var hits = Physics2D.OverlapCircleAll(origin, w.swingRadius, w.hitMask);
        if (hits == null)
        {
            Debug.Log("Melee hit nothing");
            return;
        }

        foreach (var h in hits)
        {
            Debug.Log($"Melee hit {h.gameObject.name}");
            var receiver = h.GetComponentInChildren<DamageReceiver>();
            sfx.PlayWeaponSound(w.weaponSound); 
            var data = new DamageData(w.damage, DamageType.Melee, gameObject, true);
            receiver.ReceiveDamage(data);
        }
    }

    void DoShoot(WeaponGunSO w)
    {
        Debug.Log("WeaponDriver DoShoot");
        if (!w.bulletPrefab) return;

        int sign = anim2D ? anim2D.FacingSign : 1;
        Vector2 dir = (sign >= 0) ? Vector2.right : Vector2.left;
        var muzzle = (sign >= 0) ? firePointRight : firePointLeft;
        var go = Instantiate(w.bulletPrefab, muzzle.position, Quaternion.identity);
        var b = go.GetComponent<Bullet>();
        if (b)
        {
            b.Configure(w.damage, dir * w.bulletSpeed, gameObject, -1f, w.hitMask);
            sfx.PlayWeaponSound(w.weaponSound);
        }
        else
        {
            var rb = go.GetComponent<Rigidbody2D>();
            if (rb) rb.linearVelocity = dir * w.bulletSpeed;
        }
    }

#if UNITY_EDITOR
    private void OnDrawGizmosSelected()
    {
        if (mainHand is WeaponMeleeSO m)
        {
            Transform t = meleeOrigin ? meleeOrigin : transform;
            int sign = anim2D ? anim2D.FacingSign : 1;
            Vector2 local = m.swingLocalOffset; local.x *= sign;
            Vector2 origin = (Vector2)t.TransformPoint(local);

            Gizmos.color = new Color(1f, 0.3f, 0.2f, 0.35f);
            const int seg = 28;
            Vector3 prev = origin + Vector2.right * m.swingRadius;
            for (int i = 1; i <= seg; i++)
            {
                float a = i * Mathf.PI * 2f / seg;
                Vector3 next = origin + new Vector2(Mathf.Cos(a), Mathf.Sin(a)) * m.swingRadius;
                Gizmos.DrawLine(prev, next);
                prev = next;
            }
        }
    }
#endif
}
