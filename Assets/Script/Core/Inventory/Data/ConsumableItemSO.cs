using UnityEngine;

public abstract class ConsumableItemSO : ItemSO, IConsumableItem
{
    public int healAmount = 25;
    public abstract bool Use(GameObject target);

    protected void OnValidate()
    {
        Kind = ItemKind.Consumable;
        stackable = true;
        if (maxStack < 1) maxStack = 99;
    }
}

public interface IConsumableItem
{
    // Inventory/Hotbar should call this, and if it returns true, remove 1 from inventory.
    bool Use(GameObject target);
}
