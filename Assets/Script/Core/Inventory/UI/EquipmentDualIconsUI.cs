using UnityEngine;
using UnityEngine.UI;

[DisallowMultipleComponent]
public class EquipmentDualIconsUI : MonoBehaviour
{
    [SerializeField] private EquipmentComponent equipment;
    [Header("Images")]
    [SerializeField] private Image mainHandImage;
    [SerializeField] private Image sideHandImage;
    [Header("Selection (optional)")]
    [SerializeField] private Image mainHighlight;
    [SerializeField] private Image sideHighlight;
    [SerializeField] private EquipSlot activeSlot = EquipSlot.MainHand;

    private void Awake()
    {
        if (!equipment) equipment = GetComponentInParent<EquipmentComponent>();
    }

    private void OnEnable()
    {
        RefreshAll();
        if (equipment != null) equipment.OnEquippedChanged += OnEqChanged;
    }

    private void OnDisable()
    {
        if (equipment != null) equipment.OnEquippedChanged -= OnEqChanged;
    }

    private void OnEqChanged(EquipSlot slot, WeaponSO _)
    {
        RefreshAll();
    }

    public void SetActiveSlot(EquipSlot slot)
    {
        activeSlot = slot;
        UpdateHighlights();
    }

    private void RefreshAll()
    {
        SetIcon(mainHandImage, equipment?.Get(EquipSlot.MainHand));
        SetIcon(sideHandImage, equipment?.Get(EquipSlot.SideHand));
        UpdateHighlights();
    }

    private void SetIcon(Image img, WeaponSO wep)
    {
        if (!img) return;
        var sp = wep ? (wep.Icon ? wep.Icon : wep.Icon) : null;
        img.sprite = sp;
        img.enabled = (sp != null);
    }

    private void UpdateHighlights()
    {
        if (mainHighlight) mainHighlight.enabled = (activeSlot == EquipSlot.MainHand);
        if (sideHighlight) sideHighlight.enabled = (activeSlot == EquipSlot.SideHand);
    }
}
