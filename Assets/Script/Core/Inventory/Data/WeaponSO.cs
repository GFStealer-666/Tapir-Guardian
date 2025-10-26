using UnityEngine;

public abstract class WeaponSO : ItemSO
{
    public WeaponKind kind;
    public AudioClip weaponSound;
}

public enum WeaponKind
{
    Melee,
    Gun
}
