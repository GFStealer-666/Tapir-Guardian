using TMPro;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public class AnimalListItemButton : MonoBehaviour
{
    [HideInInspector] public string animalId;
    [HideInInspector] public AnimalDataController controller;

    [Header("UI")]
    [SerializeField] private TMP_Text labelTH;
    [SerializeField] private Image lockIcon;
    [SerializeField] private Image animalImage;  // 🆕 image of the animal when unlocked
    [SerializeField] private CanvasGroup canvasGroup; // optional fade

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
        controller?.SelectAnimal(animalId);
    }

    /// <summary>Sets label text and animal sprite (called by controller).</summary>
    public void Setup(string textTH, Sprite animalSprite)
    {
        _realNameTH = textTH ?? "";
        if (animalImage) animalImage.sprite = animalSprite;
        RefreshVisuals();
    }

    /// <summary>Toggle locked/unlocked visuals and update text/icons.</summary>
    public void SetLocked(bool locked)
    {
        _isLocked = locked;
        RefreshVisuals();
    }

    private void RefreshVisuals()
    {
        if (!_btn) return;

        _btn.interactable = !_isLocked;

        // show/hide icons
        if (lockIcon) lockIcon.gameObject.SetActive(_isLocked);
        if (animalImage) animalImage.gameObject.SetActive(!_isLocked);

        // fade out if locked
        if (canvasGroup)
            canvasGroup.alpha = _isLocked ? 0.5f : 1f;

        // text
        if (labelTH)
            labelTH.text = _isLocked ? "ยังไม่ปลดล็อค" : _realNameTH;
    }

    public bool IsLocked => _isLocked;
}
