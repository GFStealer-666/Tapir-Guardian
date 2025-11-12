using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class WeaponHudPresenter : MonoBehaviour
{
    [Header("Weapon Buttons")]
    [SerializeField] private Button slot1Button;  // left = MainHand
    [SerializeField] private Button slot2Button;  // right = SideHand

    [Header("Visuals")]
    [Tooltip("Grey tint when selected.")]
    [SerializeField] private Color selectedColor = new Color(0.7f, 0.7f, 0.7f, 1f);
    [SerializeField] private Color normalColor   = Color.white;

    private Image _slot1Image;
    private Image _slot2Image;

    private void Awake()
    {
        if (slot1Button) _slot1Image = slot1Button.GetComponent<Image>();
        if (slot2Button) _slot2Image = slot2Button.GetComponent<Image>();
    }

    public void UpdateSelection(int selectedIndex)
    {
        if (_slot1Image) _slot1Image.color = (selectedIndex == 0) ? selectedColor : normalColor;
        if (_slot2Image) _slot2Image.color = (selectedIndex == 1) ? selectedColor : normalColor;
    }

    private void OnValidate()
    {
        if (!slot1Button || !slot2Button)
            Debug.LogWarning("[WeaponHudSimple] Assign both weapon slot buttons in the inspector.");
    }
}
