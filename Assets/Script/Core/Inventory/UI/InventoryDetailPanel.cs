using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class InventoryDetailPanel : MonoBehaviour
{
    [Header("Data")]
    [SerializeField] private InventoryComponent inventory;
    [SerializeField] private ItemDatabaseSO database;

    [Header("UI")]
    [SerializeField] private Image itemIcon;
    [SerializeField] private TMP_Text itemNameText;
    [SerializeField] private TMP_Text detailText;
    [SerializeField] private Button useButton;

    private string currentItemId;
    private ItemSO currentSO;
    private IItemUseTargetProvider targetProvider;

    private void Awake()
    {
        if (!inventory) inventory = GetComponentInParent<InventoryComponent>();
        targetProvider = inventory.GetComponent<IItemUseTargetProvider>();
        Debug.Log(targetProvider);
        if (useButton) useButton.onClick.RemoveAllListeners();
        if (useButton) useButton.onClick.AddListener(OnUseClicked);

        if (inventory) inventory.OnItemChanged += OnInventoryChanged;

        RefreshEmpty();
    }

    private void OnDestroy()
    {
        if (inventory) inventory.OnItemChanged -= OnInventoryChanged;
    }

    private void OnInventoryChanged()
    {
        // keep current selection but refresh count / button
        if (!string.IsNullOrEmpty(currentItemId)) ShowItem(currentItemId);
        else RefreshEmpty();
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

        if (itemIcon)
        {
            itemIcon.sprite = currentSO.Icon;
            itemIcon.enabled = currentSO.Icon != null; // safer than relying on Unity's bool cast
        }
        if (itemNameText) itemNameText.text = currentSO.DisplayName;
        if (detailText) detailText.text = currentSO.Description;

        bool hasTarget = targetProvider != null && targetProvider.GetUseTarget();
        bool canUse = hasTarget && (currentSO is IConsumableItem) && count > 0;
        Debug.Log(canUse);
        if (useButton) useButton.interactable = canUse;
    }

    private void OnUseClicked()
    {
        if (!inventory || !currentSO || string.IsNullOrEmpty(currentItemId)) return;
        var target = (targetProvider != null) ? targetProvider.GetUseTarget() : null;
        if (!target) { if (useButton) useButton.interactable = false; return; }

        if (currentSO is IConsumableItem c)
        {
            bool used = c.Use(target);
            if (used)
            {
                // Consume first, then refresh
                bool ok = inventory.Consume(currentItemId, 1);
                if (!ok) Debug.LogWarning($"Tried to consume {currentItemId} but inventory says no.");
                ShowItem(currentItemId);
            }
        }
    }

    private void RefreshEmpty()
    {
        currentItemId = null; currentSO = null;
        if (itemIcon) { itemIcon.sprite = null; itemIcon.enabled = false; }
        if (itemNameText) itemNameText.text = "ไอเท็ม";
        if (detailText) detailText.text = "คำอธิบายไอเท็ม";
        if (useButton) useButton.interactable = false;
    }

    private ItemSO Resolve(string id)
    {
        if (string.IsNullOrEmpty(id)) return null;
        if (database) return database.Get(id);
        if (inventory) return inventory.Resolve(id);
        return null;
    }
}
