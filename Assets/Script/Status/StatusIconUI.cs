using UnityEngine;
using UnityEngine.UI;
using TMPro;

[DisallowMultipleComponent]
public class StatusIconUI : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private Image iconImage;
    [SerializeField] private TMP_Text statusNameText; // optional
    private StatusInstance boundInstance;

    /// <summary>Initialize or update this icon to represent a specific StatusInstance.</summary>
    public void Bind(StatusInstance instance)
    {
        boundInstance = instance;

        if (instance?.def == null)
        {
            Clear();
            return;
        }

        if (iconImage) iconImage.sprite = instance.def.icon;
        UpdateTexts();
    }

    /// <summary>Update timer and stack text — call each frame or when values change.</summary>
    public void UpdateVisual()
    {
        if (boundInstance == null || boundInstance.def == null)
        {
            Clear();
            return;
        }
        UpdateTexts();
    }

    private void UpdateTexts()
    {
        if (statusNameText) statusNameText.text = boundInstance.def.displayName;
    }

    private void Clear()
    {
        if (iconImage) iconImage.sprite = null;
        if (statusNameText) statusNameText.text = "";
    }
}
