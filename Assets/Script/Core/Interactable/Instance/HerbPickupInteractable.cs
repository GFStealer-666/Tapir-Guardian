using UnityEngine;

public class HerbPickupInteractable : BaseInteractable
{
    [Header("Herb Data")]
    [SerializeField] private HerbDataSO herb;
    [SerializeField] private ItemSO herbItem;

    [Header("Popup / UI")]
    [SerializeField] private HerbPopupView popup; // optional; will auto-find if null

    [Header("Behavior")]
    [SerializeField] private bool disableAfterPickup = true; // hide herb after pickup
    [SerializeField] private bool playOnlyOnce      = true;  // prevent repeat pickups
    [SerializeField] private bool lockControlsDuringPopup = false; // usually false for pickups

    private bool _picked;
    private PlayerBrain _cachedPlayer;
    private HerbCodexSession _codex;

    private void Awake()
    {
        _codex = HerbCodexSession.Instance ?? FindAnyObjectByType<HerbCodexSession>();
        if (!popup) popup = FindAnyObjectByType<HerbPopupView>();
    }

    public override bool CanInteract()
    {
        if (!isActiveAndEnabled) return false;
        if (_picked && playOnlyOnce) return false;
        return herb != null;
    }

    public override string GetPrompt()
    {
        var th = herb ? herb.nameTH : "Herb";
        return $"กด E เพื่อเก็บ {th}";
    }

    public override void Interact(PlayerBrain player)
    {
        if (!CanInteract()) return;

        _cachedPlayer = player;

        // Inventory: try the player's inventory first, fall back to a serialized field if you want
        var inv = player ? player.GetComponentInChildren<InventoryComponent>() : null;
        if (!inv)
        {
            // optional: allow serialized fallback (drag in Inspector if desired)
            inv = GetComponentInParent<InventoryComponent>();
        }

        if (inv && herbItem)
        {
            inv.Add(herbItem.Id, 1);
        }
        else
        {
            Debug.LogWarning($"[HerbPickup] Inventory or HerbItem missing on {name}");
        }

        // Codex unlock + popup
        bool alreadyUnlocked = _codex && _codex.IsUnlocked(herb.id);

        if (!alreadyUnlocked)
        {
            if (popup)
            {
                popup.Bind(herb);
                popup.Show(true);
            }

            if (_codex && _codex.Unlock(herb.id))
            {
                Debug.Log($"[HerbPickup] Unlocked '{herb.nameTH}'.");
            }
        }
        else
        {
            // Show info again if you want
            if (popup)
            {
                popup.Bind(herb);
                popup.Show(true);
            }
        }

        if (lockControlsDuringPopup && player)
            player.SetInputBlocked(true); // only if you want to freeze during popup

        // Broadcast for SFX/VFX/quest hooks
        RaiseEvents(player);

        _picked = true;
        if (disableAfterPickup && playOnlyOnce)
        {
            // Hide the herb mesh/renderer so it’s “collected”
            gameObject.SetActive(false);
        }
    }

    // Call this from the popup Close button IF you set lockControlsDuringPopup = true
    public void OnPopupClosed()
    {
        if (lockControlsDuringPopup && _cachedPlayer)
            _cachedPlayer.SetInputBlocked(false);
    }
}
