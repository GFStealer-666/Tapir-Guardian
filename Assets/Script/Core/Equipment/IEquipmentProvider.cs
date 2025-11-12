using System;

public interface IEquipmentProvider
{
    event Action<EquipSlot, WeaponSO> OnEquippedChanged;
    WeaponSO Get(EquipSlot slot);
    T GetAs<T>(EquipSlot slot) where T : ItemSO;
}
