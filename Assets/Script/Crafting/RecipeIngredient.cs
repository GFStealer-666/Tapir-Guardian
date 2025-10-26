using System;
using UnityEngine;

[Serializable]
public struct RecipeIngredient
{
    public ItemSO item;   // assign in Inspector
    public int count;     // >= 1

    [HideInInspector] public string itemId; // auto-filled from item

    public void OnValidate()
    {
        itemId = item ? item.Id : null;
        if (count < 1) count = 1;
    }
}
