// Assets/Scripts/Crafting/UI/CraftingUIController.cs
using System.Collections.Generic;
using UnityEngine;

[DisallowMultipleComponent]
public class CraftingStationController : MonoBehaviour
{
    [Header("Pieces")]
    [SerializeField] private CraftingMenuController menu;   // your list builder (left)
    [SerializeField] private CraftingDetailPanel detail;    // your detail (right)
    [SerializeField] private CraftingSystem crafting;       // job runner

    private string _activeStationId;

    private void Awake()
    {
        if (!crafting) crafting = GetComponentInParent<CraftingSystem>();
    }

    public void SetActiveStation(ICraftingStation station)
    {
        if (station == null) return;
        _activeStationId = station.StationId;

        // feed recipes to the menu
        menu.SetRecipes(station.Recipes);
        menu.Open();

        // tell the detail panel which station to bind to
        detail.SetStation(_activeStationId);
    }

    public void ClearActiveStation()
    {
        _activeStationId = null;
        menu.Close();
        detail.SetStation(null);
    }

    public void CancelAllJobs()
    {
        if (!string.IsNullOrEmpty(_activeStationId))
            crafting.CancelAllForStation(_activeStationId, refund: true);
    }
}
