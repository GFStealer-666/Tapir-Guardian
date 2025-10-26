using UnityEngine;

[CreateAssetMenu(fileName = "NewGunWeapon", menuName = "Tapir/Weapon/Gun")]
public class WeaponGunSO : WeaponSO
{
    [Header("Gun Stats")]
    public int damage = 4;
    public float fireCooldown = 0.2f;
    public float bulletSpeed = 12f;

    [Header("Ammo")]
    public string ammoItemId;   // e.g. "9mm"
    public int ammoPerShot = 1;

    [Header("Bullet Prefab")]
    public Bullet bulletPrefab;

    [Header("Muzzle offset")]
    public Vector2 muzzleLocalOffset = new Vector2(0.5f, 0.2f);

    private void OnValidate()
    {
        kind = WeaponKind.Gun;
    }
}
