using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

public class InventorySlotUI : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] private Image icon;
    [SerializeField] private TMP_Text countText;
    [SerializeField] private Button button;     // add a Button on the slot root
    [SerializeField] private GameObject highlight;

    // Data
    public string ItemId { get; private set; }
    public int Count { get; private set; }

    public event Action<InventorySlotUI> OnClicked;

    public void OnClickedInternal()
    {
        OnClicked?.Invoke(this);
    }
    public void Bind(ItemSO item, int count)
    {
        ItemId = item ? item.Id : null;
        Count  = Mathf.Max(0, count);

        if (icon) { icon.sprite = item ? item.Icon : null; icon.enabled = (item != null); }
        if (countText) countText.text = Count > 1 ? Count.ToString() : "1";
        SetHighlight(false);
        gameObject.SetActive(true);
    }

    public void Clear()
    {
        ItemId = null; Count = 0;
        if (icon) { icon.sprite = null; icon.enabled = false; }
        if (countText) countText.text = "";
        SetHighlight(false);
        gameObject.SetActive(true);
    }

    public void SetHighlight(bool on) { if (highlight) highlight.SetActive(on); }
}
