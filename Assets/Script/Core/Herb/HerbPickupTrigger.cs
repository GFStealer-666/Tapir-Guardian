using UnityEngine;

/// <summary>
/// Attach this to a herb pickup object on the ground.
/// It detects player collision, checks unlock state from HerbCodexController,
/// and shows the popup (HerbPopupView) accordingly.
/// </summary>
[RequireComponent(typeof(Collider2D))]
public class HerbPickupTrigger : MonoBehaviour
{
    [Header("Herb Data")]
    public HerbDataSO herb;
    public ItemSO herbItem;

    [Header("References")]
    public HerbPopupView popup;              // link to your popup prefab/panel in the scene
    public string playerTag = "Player";
    [SerializeField] private InventoryComponent inventoryComponent;

    [Header("Behavior")]
    public bool disableAfterPickup = true;   // hide pickup after being triggered
    public bool playOnlyOnce = true;         // skip if already unlocked (optional debug)

    private HerbCodexSession _codex;

    void Awake()
    {
        _codex = FindAnyObjectByType<HerbCodexSession>();
        if (!popup)
            popup = FindAnyObjectByType<HerbPopupView>();

        if (!_codex)
            Debug.LogWarning("[HerbPickupTrigger] No HerbDataController found!");
        if (!popup)
            Debug.LogWarning("[HerbPickupTrigger] No HerbPopupView found!");
    }

    private void Pickup(Collider2D other)
    {
        if (!herb || !other.CompareTag("Player"))
            return;

        Debug.Log($"[HerbPickupTrigger] Player entered trigger for herb '{herb.nameTH}'");

        // Check if herb already unlocked
        bool isUnlocked = _codex && _codex.IsUnlocked(herb.id);

        if (!isUnlocked)
        {
            // First time discovery → show popup and unlock on confirm
            ShowPopupAndUnlock();
        }
        else
        {
            // Already discovered → just show popup info again
            ShowPopupOnly();
        }

        if (disableAfterPickup && playOnlyOnce)
            gameObject.SetActive(false);

        inventoryComponent.Add(herbItem.Id , 1);
    }

    private void ShowPopupAndUnlock()
    {
        Debug.Log($"[HerbPickupTrigger] Unlocking herb '{herb.nameTH}'");
        if (!popup) return;
        popup.Bind(herb);
        popup.Show(true);

        // After showing popup, mark as unlocked immediately (or after confirm)
        var session = HerbCodexSession.Instance;
        if (session && session.Unlock(herb.id))
        {
            Debug.Log($"[HerbPickupTrigger] Herb '{herb.nameTH}' unlocked and saved.");
        }
    }

    private void ShowPopupOnly()
    {
        if (!popup) return;
        popup.Bind(herb);
        popup.Show(true);
        Debug.Log($"[HerbPickupTrigger] Showing info for already-unlocked herb '{herb.nameTH}'");
    }
}
