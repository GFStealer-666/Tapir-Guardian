using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class HerbPopupPanel : MonoBehaviour
{
    [Header("UI")]
    public GameObject root;
    public TMP_Text titleText;
    public TMP_Text herbNameText;
    public Image herbImage;
    public TMP_Text infoText;
    public Button confirmButton;

    private HerbDataSO _current;
    public event Action<HerbDataSO> Confirmed;

    void Awake()
    {
        if (confirmButton)
            confirmButton.onClick.AddListener(OnConfirm);
        Hide();
    }

    public void Show(HerbDataSO herb)
    {
        _current = herb;
        if (!herb) return;

        if (titleText) titleText.text = "ค้นพบสมุนไพรใหม่!";
        if (herbNameText) herbNameText.text = herb.nameTH;
        if (herbImage) herbImage.sprite = herb.image;

        infoText.text =
            $"ชื่อวิทยาศาสตร์ : {herb.scientificName}\n\n" +
            $"สรรพคุณ :\n{herb.propertiesText}\n\n" +
            $"การใช้งาน :\n{herb.usageText}";

        root.SetActive(true);
    }

    private void OnConfirm()
    {
        root.SetActive(false);
        Confirmed?.Invoke(_current);
    }

    public void Hide() => root.SetActive(false);
}
