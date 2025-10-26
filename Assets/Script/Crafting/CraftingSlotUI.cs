using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// Simple slot view for the crafting menu:
/// - For inputs: icon + name + "have/need"
/// - For output: icon + name + "xCount"
/// Hook this onto a small prefab with: Image icon, TMP name, TMP count.
/// </summary>
public class CraftingSlotUI : MonoBehaviour
{
    [Header("UI")]
    public Image icon;
    public TMP_Text nameText;
    public TMP_Text countText;

    /// <summary>Bind an input ingredient.</summary>
    public void BindInput(ItemSO item, int have, int need)
    {
        if (icon)     icon.sprite = item ? item.Icon : null;
        if (nameText) nameText.text = item ? item.DisplayName : "-";
        if (countText) countText.text = $"{Mathf.Max(0, have)}/{Mathf.Max(1, need)}";
    }

    /// <summary>Bind the output item.</summary>
    public void BindOutput(ItemSO item, int count)
    {
        if (icon)     icon.sprite = item ? item.Icon : null;
        if (nameText) nameText.text = item ? item.DisplayName : "-";
        if (countText) countText.text = count > 1 ? $"x{count}" : "";
    }

    /// <summary>Clear visuals (optional helper).</summary>
    public void Clear()
    {
        if (icon)     icon.sprite = null;
        if (nameText) nameText.text = "-";
        if (countText) countText.text = "";
    }
}
