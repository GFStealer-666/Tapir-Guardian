using UnityEngine;

[CreateAssetMenu(fileName = "NewMeleeWeapon", menuName = "Tapir/Weapon/Melee")]
public class WeaponMeleeSO : WeaponSO
{
    [Header("Melee Stats")]
    public int damage = 10;
    public float attackCooldown = 0.4f;

    [Header("Hitbox Shape")]
    public float swingRadius = 1.2f;
    public Vector2 swingLocalOffset = new Vector2(0.6f, 0f);
    public float windUp = 0.12f;
    public bool useAnimationEvent = true;
    public LayerMask hitMask = ~0;
    private void OnValidate()
    {
        kind = WeaponKind.Melee;
    }
}