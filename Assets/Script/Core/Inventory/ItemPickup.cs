using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class ItemPickup : MonoBehaviour
{
    public ItemSO Item;

    void OnTriggerEnter2D(Collider2D other)
    {
        var inv = other.GetComponentInParent<InventoryComponent>();
        if (!inv || !Item) return;
        if (inv.Add(Item.Id))
            Destroy(gameObject);
    }
}