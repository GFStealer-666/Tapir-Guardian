using UnityEngine;

[DisallowMultipleComponent]
public class HotbarComponent : MonoBehaviour
{
    // [SerializeField] private ItemDatabaseSO database;
    // [Tooltip("Index 0..9 = keys 1..0")]
    // [SerializeField] private string[] slotItemIds = new string[10];

    // /// <summary>
    // /// Assign an item id to a hotbar slot (1..10, where 10 == key '0').
    // /// </summary>
    // public void SetSlot(int slotIndex1to10, string itemId)
    // {
    //     int i = Mathf.Clamp(slotIndex1to10, 1, 10) - 1;
    //     slotItemIds[i] = itemId;
    // }

    // /// <summary>
    // /// Equip the item in the given hotbar slot (1..10) into MainHand.
    // /// Matches your PlayerBrain's call hotbar.EquipSlot(...).
    // /// </summary>
    // public bool ConfigureSlot(int slotIndex1to10, EquipmentComponent equipment)
    // {
    //     if (!equipment || database == null) return false;

    //     int i = Mathf.Clamp(slotIndex1to10, 1, 10) - 1;
    //     var id = slotItemIds[i];
    //     if (string.IsNullOrEmpty(id)) return false;

    //     var item = database.Get(id);
    //     if (!item) return false;

    //     // For now always equip to MainHand; expand if you add OffHand logic later.
    //     return equipment.EquipMainGun();
    // }

    // public string GetItemId(int slotIndex1to10)
    // {
    //     int i = Mathf.Clamp(slotIndex1to10, 1, 10) - 1;
    //     return slotItemIds[i];
    // }
}


public enum HotbarMode { None, EquipMainHand, UseItem }