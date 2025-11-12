using System;

public interface IWeaponSelector
{
    /// 0 = MainHand slot, 1 = SideHand slot (index mirrors EquipSlot)
    int SelectedIndex { get; }
    WeaponSO SelectedWeapon { get; }

    /// Fired when user changes selection (index changed)
    event Action<int> OnSelectionChanged;

    /// Called by UI or input layer
    void SelectIndex(int index);
    void SelectNext(int direction);  // +1 / -1 (wraps 0..1)
}
