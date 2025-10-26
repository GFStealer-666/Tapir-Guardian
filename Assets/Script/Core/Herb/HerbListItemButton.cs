using TMPro;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public class HerbListItemButton : MonoBehaviour
{
    [HideInInspector] public string herbId;
    [HideInInspector] public HerbDataController controller;

    [Header("UI")]
    [SerializeField] private TMP_Text labelTH;
    [SerializeField] private Image lockIcon;
    [SerializeField] private Image herbImage;
    [SerializeField] private CanvasGroup canvasGroup;

    private Button _btn;
    private string _realNameTH = "";
    private bool _isLocked = true;

    void Awake()
    {
        _btn = GetComponent<Button>();
        _btn.onClick.AddListener(OnClick);
    }

    private void OnClick()
    {
        if (_isLocked) return;
        controller?.SelectHerb(herbId);
    }

    public void Setup(string textTH, Sprite herbSprite)
    {
        _realNameTH = textTH ?? "";
        if (herbImage) herbImage.sprite = herbSprite;
        RefreshVisuals();
    }

    public void SetLocked(bool locked)
    {
        _isLocked = locked;
        RefreshVisuals();
    }

    private void RefreshVisuals()
    {
        if (!_btn) return;

        _btn.interactable = !_isLocked;
        if (lockIcon) lockIcon.gameObject.SetActive(_isLocked);
        if (herbImage) herbImage.gameObject.SetActive(!_isLocked);
        if (canvasGroup) canvasGroup.alpha = _isLocked ? 0.5f : 1f;
        if (labelTH) labelTH.text = _isLocked ? "ยังไม่ปลดล็อค" : _realNameTH;
    }

    public bool IsLocked => _isLocked;
}
