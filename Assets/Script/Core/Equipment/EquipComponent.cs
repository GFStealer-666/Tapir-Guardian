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
    public T GetAs<T>(EquipSlot slot) where T : ItemSO => Get(slot) as T;
}

public enum EquipSlot { MainHand, SideHand }