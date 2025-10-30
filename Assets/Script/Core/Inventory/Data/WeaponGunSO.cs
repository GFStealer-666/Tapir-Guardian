using UnityEngine;

[CreateAssetMenu(fileName = "NewGunWeapon", menuName = "Tapir/Weapon/Gun")]
public class WeaponGunSO : WeaponSO
{
    [Header("Ranged Stats")]
    public int damage = 8;
    public float shootCooldown = 0.35f;
    public float windUp = 0.08f;
    public bool useAnimationEvent = true;

    [Header("Projectile")]
    public GameObject bulletPrefab;
    public float bulletSpeed = 12f;
    public LayerMask hitMask = ~0;

    [Header("Muzzle offset")]
    public Vector2 muzzleLocalOffset = new Vector2(0.5f, 0.2f);

    private void OnValidate()
    {
        kind = WeaponKind.Gun;
    }
}
