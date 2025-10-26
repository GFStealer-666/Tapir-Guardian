using UnityEngine;

[CreateAssetMenu(menuName = "Items/Crafting Material", fileName = "NewCraftingMaterial")]
public class CraftingMaterialSO : ItemSO
{
    [Header("Material Info")]
    [TextArea(1, 4)]
    public string description;

    [Tooltip("Optional: for grouping (e.g., Wood, Metal, Herb, MonsterPart).")]
    public string category;

    [Tooltip("Optional: for filtering/searching in UI (comma-separated or free text).")]
    public string tags;

    private void OnValidate()
    {
        // Ensure correct defaults for crafting mats
        Kind = ItemKind.CraftingMaterial;
        stackable = true;
        if (maxStack <= 0) maxStack = 999; // generous default
    }
}
