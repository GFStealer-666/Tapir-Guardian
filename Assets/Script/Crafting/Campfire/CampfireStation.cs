// Assets/Scripts/Crafting/CampfireStation.cs
using System.Collections.Generic;
using UnityEngine;

[DisallowMultipleComponent]
public class CampfireStation : MonoBehaviour, ICraftingStation
{
    [SerializeField] private string stationId = "campfire";
    [SerializeField] private List<CraftingRecipeSO> recipes = new();

    public string StationId => stationId;
    public IEnumerable<CraftingRecipeSO> Recipes => recipes;
}
