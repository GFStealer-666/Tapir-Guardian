// Assets/Scripts/Crafting/ICraftingStation.cs
using System.Collections.Generic;

public interface ICraftingStation
{
    string StationId { get; }                     // e.g. "campfire"
    IEnumerable<CraftingRecipeSO> Recipes { get; } // what this station can craft
}
