// Utility: try use a consumable item id once; returns true if used & consumed.
using UnityEngine;

public static class ConsumableUseHelper
{
    public static bool TryUseOnce(InventoryComponent inv, ItemDatabaseSO db, string itemId, GameObject user)
    {
        if (!inv || !db || string.IsNullOrEmpty(itemId)) return false;

        // ensure we own one
        if (!inv.Has(itemId, 1)) return false;

        var so = db.Get(itemId);
        if (so is IConsumableItem consumable)
        {
            if (consumable.Use(user))
            {
                inv.Consume(itemId, 1);
                return true;
            }
        }
        return false;
    }
}
