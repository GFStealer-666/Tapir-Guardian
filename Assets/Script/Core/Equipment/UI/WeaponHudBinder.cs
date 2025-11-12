using UnityEngine;

public class WeaponHudBinder : MonoBehaviour
{
    [SerializeField] private EquipmentProviderAdapter equipmentProvider; // IEquipmentProvider
    [SerializeField] private WeaponSelectorController weaponSelector;    // IWeaponSelector
    [SerializeField] private WeaponHudPresenter hud;               // IWeaponHud

    private IEquipmentProvider _equip;
    private IWeaponSelector _selector;

    private void Awake()
    {
        _equip    = equipmentProvider;
        _selector = weaponSelector;

        if (_equip == null || _selector == null)
        {
            Debug.LogError("[WeaponHudBinder] Missing references or wrong interfaces.");
            enabled = false;
            return;
        }
    }

    private void OnEnable()
    {
        _selector.OnSelectionChanged += Refresh;
        _equip.OnEquippedChanged += OnEquipChanged;
        Refresh(_selector.SelectedIndex);
    }

    private void OnDisable()
    {
        _selector.OnSelectionChanged -= Refresh;
        _equip.OnEquippedChanged -= OnEquipChanged;
    }

    private void OnEquipChanged(EquipSlot slot, WeaponSO so)
    {
        Refresh(_selector.SelectedIndex);
    }

    private void Refresh(int selectedIndex)
    {
        hud.UpdateSelection(selectedIndex);
    }
}
