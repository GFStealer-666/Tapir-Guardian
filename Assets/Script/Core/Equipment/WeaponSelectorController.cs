using System;
using UnityEngine;

[DisallowMultipleComponent]
public class WeaponSelectorController : MonoBehaviour, IWeaponSelector
{
    [Header("Equipment")]
    [SerializeField] private EquipmentProviderAdapter equipmentProvider;
    private IEquipmentProvider _equip;

    [Header("Input")]
    [Tooltip("Enable built-in reading of scroll wheel and keys 1/2.")]
    [SerializeField] private bool readPlayerInput = true;

    public int SelectedIndex { get; private set; } = 0; // default select MainHand (index 0)
    public WeaponSO SelectedWeapon => GetByIndex(SelectedIndex);

    public event Action<int> OnSelectionChanged;

    private void Awake()
    {
        _equip = equipmentProvider;

        if (_equip == null)
            Debug.LogError("[WeaponSelectorController] equipmentProvider must implement IEquipmentProvider.");

        if (_equip != null)
        {
            // If designer didn’t set a main gun yet, fallback to sidehand
            if (_equip.Get(EquipSlot.MainHand) == null && _equip.Get(EquipSlot.SideHand) != null)
                SelectedIndex = 1;

            _equip.OnEquippedChanged += HandleEquippedChanged;
        }
    }

    private void OnDestroy()
    {
        if (_equip != null)
            _equip.OnEquippedChanged -= HandleEquippedChanged;
    }

    private void Update()
    {
        if (!readPlayerInput) return;

        float scroll = Input.GetAxisRaw("Mouse ScrollWheel");
        if (scroll > 0.01f) SelectNext(+1);
        else if (scroll < -0.01f) SelectNext(-1);
        
    }
    // Actial swapping the weapon
    public void SelectIndex(int index)
    {
        index = Mathf.Clamp(index, 0, 1);
        if (SelectedIndex == index) return;

        SelectedIndex = index;
        Debug.Log($"[WeaponSelector] Swapping to weapon {GetByIndex(SelectedIndex).name}");
        OnSelectionChanged?.Invoke(SelectedIndex);
    }

    public void SelectNext(int direction)
    {
        int next = (SelectedIndex + (direction >= 0 ? 1 : -1)) & 1; // wrap 0..1
        SelectIndex(next);
    }

    private void HandleEquippedChanged(EquipSlot slot, WeaponSO weapon)
    {
        // If the currently selected index became null, auto-switch to the other slot if available.
        if (GetByIndex(SelectedIndex) != null) return;

        int other = (SelectedIndex == 0) ? 1 : 0;
        if (GetByIndex(other) != null)
            SelectIndex(other);
        else
            OnSelectionChanged?.Invoke(SelectedIndex); 
    }

    private WeaponSO GetByIndex(int idx)
    {
        if (_equip == null) return null;
        return (idx == 0) ? _equip.Get(EquipSlot.MainHand)
                          : _equip.Get(EquipSlot.SideHand);
    }
}
