using UnityEngine;

/// <summary>
/// Generic world item pickup — can represent ammo, coin, health, etc.
/// Drops into the world, waits for player trigger, adds to inventory or applies effect.
/// </summary>
[RequireComponent(typeof(Collider2D))]
public class ItemPickup : MonoBehaviour
{
    [Header("Item Data")]
    [SerializeField] private ItemSO itemData;         // optional - if tied to inventory system
    [SerializeField] private int quantity = 1;        // amount given when picked
    [SerializeField] private string playerTag = "Player";

    [Header("Behavior")]
    [SerializeField] private bool autoDestroy = true; // destroy after pickup
    [SerializeField] private float destroyDelay = 0.05f;
    [SerializeField] private bool playPickupEffect = true;

    [Header("Visuals & Audio (optional)")]
    [SerializeField] private AudioClip pickupSound;
    [SerializeField] private GameObject pickupVFX;

    private bool _pickedUp;

    private void Awake()
    {
        // ensure trigger collider
        var col = GetComponent<Collider2D>();
        col.isTrigger = true;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (_pickedUp) return;
        if (!other.CompareTag(playerTag)) return;

        var inventory = other.GetComponentInChildren<InventoryComponent>();
        if (inventory && itemData)
        {
            inventory.Add(itemData.Id, quantity);
            Debug.Log($"[Pickup] {itemData.name} +{quantity}");
        }
        else
        {
            // Fallbacks for non-inventory items
            var stats = other.GetComponentInChildren<HealthComponent>();
            if (stats && itemData && itemData.Kind == ItemKind.Consumable)
            {
                stats.Heal(quantity);
            }
        }

        HandlePickup();
    }

    private void HandlePickup()
    {
        _pickedUp = true;

        if (pickupSound)
            AudioSource.PlayClipAtPoint(pickupSound, transform.position);

        if (pickupVFX)
            Instantiate(pickupVFX, transform.position, Quaternion.identity);

        // optional: broadcast events (quest, analytics, etc.)
        SendMessage("OnItemPicked", itemData, SendMessageOptions.DontRequireReceiver);

        if (autoDestroy)
            Destroy(gameObject, destroyDelay);
    }

    // Allow spawner/enemy to set item type & amount dynamically
    public void Configure(ItemSO item, int count)
    {
        itemData = item;
        quantity = count;
    }
}
