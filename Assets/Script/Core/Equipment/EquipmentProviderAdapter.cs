using UnityEngine;
using System;
[DisallowMultipleComponent]
public class EquipmentProviderAdapter : MonoBehaviour, IEquipmentProvider
{
    [SerializeField] private EquipmentComponent equipment;

    public event Action<EquipSlot, WeaponSO> OnEquippedChanged
    {
        add    { if (equipment != null) equipment.OnEquippedChanged += value; }
        remove { if (equipment != null) equipment.OnEquippedChanged -= value; }
    }

    public WeaponSO Get(EquipSlot slot) => equipment ? equipment.Get(slot) : null;
    public T GetAs<T>(EquipSlot slot) where T : ItemSO => equipment ? equipment.GetAs<T>(slot) : null;
}
