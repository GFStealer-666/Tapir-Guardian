using System;
using UnityEngine;

[DisallowMultipleComponent]
public class EquipmentComponent : MonoBehaviour
{
    [SerializeField] private WeaponSO mainHandWeapon; // gun
    [SerializeField] private WeaponSO sideHandWeapon; // melee / always available

    // Fired whenever a slot changes
    public event Action<EquipSlot, WeaponSO> OnEquippedChanged;

    public WeaponSO Get(EquipSlot slot)
    {
        switch (slot)
        {
            case EquipSlot.MainHand: return mainHandWeapon;
            case EquipSlot.SideHand: return sideHandWeapon;
            default: return null;
        }
    }

    // Player can change only gun for now.
    public void EquipMainGun(WeaponSO gunWeapon)
    {
        if (gunWeapon && gunWeapon.kind == WeaponKind.Gun)
        {
            mainHandWeapon = gunWeapon;
            OnEquippedChanged?.Invoke(EquipSlot.MainHand, mainHandWeapon);
        }
    }

    // Designer-only / persistent: assign knife in inspector and never touch at runtime.
    public void SetSideHandMelee(WeaponSO meleeWeapon)
    {
        if (meleeWeapon && meleeWeapon.kind == WeaponKind.Melee)
        {
            sideHandWeapon = meleeWeapon;
            OnEquippedChanged?.Invoke(EquipSlot.SideHand, sideHandWeapon);
        }
    }

    public T GetAs<T>(EquipSlot slot) where T : ItemSO => Get(slot) as T;
}
