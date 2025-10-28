using UnityEngine;

[DisallowMultipleComponent]
public class WeaponDriver : MonoBehaviour
{
    [Header("Refs")]
    [SerializeField] private EquipmentComponent equipment;
    [SerializeField] private InventoryComponent inventory;
    [SerializeField] private Transform attackOrigin; // melee origin (torso/hand)
    [SerializeField] private Transform muzzleOrigin; // gun muzzle
    [SerializeField] private LayerMask hitMask;      // what we can damage
    [SerializeField] private PlayerAudioSoundEffect audioEffect;

    [Header("Facing")]
    [SerializeField] private bool useScaleForFacing = false;
    private float lastFacingX = 1f;

    // Separate cooldowns
    private float _nextGunTime = 0f;
    private float _nextMeleeTime = 0f;

    private void Awake()
    {
        if (!equipment) equipment = GetComponent<EquipmentComponent>() ?? GetComponentInParent<EquipmentComponent>();
        if (!inventory) inventory = GetComponent<InventoryComponent>() ?? GetComponentInParent<InventoryComponent>();

        if (!attackOrigin) attackOrigin = transform;
        if (!muzzleOrigin) muzzleOrigin = transform;
    }

    public void UpdateFacingFromInput(Vector2 move)
    {
        if (!useScaleForFacing && Mathf.Abs(move.x) > 0.01f)
            lastFacingX = Mathf.Sign(move.x);

        if (useScaleForFacing)
        {
            float sx = transform.localScale.x;
            if (Mathf.Abs(sx) > 0.0001f)
                lastFacingX = Mathf.Sign(sx);
        }
    }

    public void TryAttack()
    {
        Debug.Log("WeaponDriver TryAttack");
        bool didShoot = TryGunAttack();
        if (didShoot) return;

        TryMeleeAttack();
    }

    // -------------------------
    // GUN LOGIC (MainHand)
    // -------------------------
    private bool TryGunAttack()
    {
        var main = equipment ? equipment.Get(EquipSlot.MainHand) as WeaponSO : null;
        var gunData = main as WeaponGunSO;
        if (!gunData) return false;

        // cooldown check
        if (Time.time < _nextGunTime) return false;

        // ammo check
        if (!CanSpendAmmo(gunData))
        {
            return false; // no ammo -> will fallback melee in TryAttack()
        }

        // spawn bullet
        FireBullet(gunData);

        // spend ammo
        SpendAmmo(gunData);

        // set cooldown
        _nextGunTime = Time.time + gunData.fireCooldown;
        audioEffect?.PlaySoundEffect(gunData.weaponSound);
        // TODO play muzzle flash anim/sfx
        return true;
    }

    private bool CanSpendAmmo(WeaponGunSO gun)
    {
        // gun with no ammo type is treated as "infinite ammo"
        if (string.IsNullOrEmpty(gun.ammoItemId)) return true;
        if (!inventory) return false;
        return inventory.Has(gun.ammoItemId, gun.ammoPerShot);
    }

    private void SpendAmmo(WeaponGunSO gun)
    {
        if (string.IsNullOrEmpty(gun.ammoItemId)) return;
        if (!inventory) return;
        inventory.Consume(gun.ammoItemId, gun.ammoPerShot);
    }

    private void FireBullet(WeaponGunSO gun)
    {
        if (!gun.bulletPrefab) return;

        // where is muzzle in world?
        Vector2 basePos = muzzleOrigin
            ? (Vector2)muzzleOrigin.TransformPoint(Vector3.zero)
            : (Vector2)transform.position;

        Vector2 off = gun.muzzleLocalOffset;
        off.x *= Mathf.Sign(lastFacingX);
        Vector2 spawnPos = basePos + off;

        Vector2 dir = (lastFacingX >= 0f) ? Vector2.right : Vector2.left;

        Bullet b = Object.Instantiate(gun.bulletPrefab, spawnPos, Quaternion.identity);
        b.Configure(
            gun.damage,
            dir * gun.bulletSpeed,
            gameObject,
            -1f,
            hitMask
        );
    }


    // -------------------------
    // MELEE LOGIC (SideHand fallback)
    // -------------------------
    private void TryMeleeAttack()
    {
        var side = equipment ? equipment.Get(EquipSlot.SideHand) as WeaponSO : null;
        var meleeData = side as WeaponMeleeSO;

        if (meleeData)
        {
            TryDoMelee(meleeData.damage,
                       meleeData.swingRadius,
                       meleeData.swingLocalOffset,
                       meleeData.attackCooldown,
                       canBeBlocked: true);
        }
        else
        {
            // fallback fists if somehow no melee in SideHand
            TryDoMelee(
                damage: 5,
                radius: 1.0f,
                localOffset: new Vector2(0.6f, 0f),
                cooldown: 0.4f,
                canBeBlocked: true
            );
        }
        audioEffect?.PlaySoundEffect(meleeData.weaponSound);
    }

    private void TryDoMelee(int damage, float radius, Vector2 localOffset, float cooldown, bool canBeBlocked)
    {
        // cooldown check
        if (Time.time < _nextMeleeTime) return;

        // position of swing
        Vector2 basePos = attackOrigin
            ? (Vector2)attackOrigin.TransformPoint(Vector3.zero)
            : (Vector2)transform.position;

        Vector2 off = localOffset;
        off.x *= Mathf.Sign(lastFacingX);

        Vector2 center = basePos + off;

        // hit detection
        var cols = Physics2D.OverlapCircleAll(center, radius, hitMask);
        foreach (var c in cols)
        {
            if (c && c.TryGetComponent<IDamageable>(out var d))
            {
                var packet = new DamageData(
                    rawDamage: damage,
                    type: DamageType.Melee,
                    source: gameObject,
                    canBeBlocked: canBeBlocked
                );
                d.ReceiveDamage(in packet);
            }
        }

        // cooldown apply
        _nextMeleeTime = Time.time + cooldown;

        // TODO: play slash anim, shake cam, etc.
    }


    private void OnDrawGizmosSelected()
    {
        // just visualize sidehand swing because that's always available
        if (!equipment) return;

        var side = equipment.Get(EquipSlot.SideHand) as WeaponMeleeSO;
        if (!side) return;

        Vector2 basePos = attackOrigin
            ? (Vector2)attackOrigin.TransformPoint(Vector3.zero)
            : (Vector2)transform.position;

        Vector2 off = side.swingLocalOffset;
        off.x *= Mathf.Sign(lastFacingX);

        Vector2 center = basePos + off;

        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(center, side.swingRadius);
    }

}
