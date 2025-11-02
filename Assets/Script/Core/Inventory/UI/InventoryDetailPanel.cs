using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class InventoryDetailPanel : MonoBehaviour
{
    [Header("Data")]
    [SerializeField] private InventoryComponent inventory;   // auto-found if null
    [SerializeField] private GameObject mainTarget;
    [SerializeField] private ItemDatabaseSO database;        // optional

    [Header("UI")]
    [SerializeField] private Image itemIcon;
    [SerializeField] private TMP_Text itemNameText;          // "ไอเท็ม"
    [SerializeField] private TMP_Text countText;             // "1 ชิ้น"
    [SerializeField] private Button useButton;               // "ใช้ไอเท็ม"

    private string currentItemId;
    private ItemSO currentSO;

    private void Awake()
    {
        if (!inventory) inventory = GetComponentInParent<InventoryComponent>();
        if (useButton) useButton.onClick.AddListener(OnUseClicked);
        RefreshEmpty();
    }

    public void ShowItem(string itemId)
    {
        currentItemId = itemId;
        currentSO = Resolve(itemId);

        if (!currentSO)
        {
            RefreshEmpty();
            return;
        }

        int count = inventory ? inventory.GetCount(itemId) : 0;

        if (itemIcon)   { itemIcon.sprite = currentSO.Icon; itemIcon.enabled = currentSO.Icon; }
        if (itemNameText) itemNameText.text = currentSO.DisplayName;
        if (countText)  countText.text = $"{Mathf.Max(0, count)} ชิ้น";

        bool canUse = (currentSO is IConsumableItem) && count > 0;
        if (useButton)  useButton.interactable = canUse;
    }

    private void OnUseClicked()
    {
        Debug.Log("Use item clicked");
        if (!inventory || !currentSO || string.IsNullOrEmpty(currentItemId)) return;

        if (currentSO is IConsumableItem c)
        {
            Debug.Log("CurrentSO are IConsumableItem");
            bool used = c.Use(mainTarget);

            if (used)
            {
                inventory.Consume(currentItemId, 1);
                Debug.Log($"{currentItemId} used");
                ShowItem(currentItemId); // refresh count/btn
            }
        }
    }

    private void RefreshEmpty()
    {
        currentItemId = null; currentSO = null;
        if (itemIcon)   { itemIcon.sprite = null; itemIcon.enabled = false; }
        if (itemNameText) itemNameText.text = "ไอเท็ม";
        if (countText)  countText.text = "0 ชิ้น";
        if (useButton)  useButton.interactable = false;
    }

    private ItemSO Resolve(string id)
    {
        if (string.IsNullOrEmpty(id)) return null;
        if (database) return database.Get(id);
        if (inventory) return inventory.Resolve(id);
        return null;
    }
}
