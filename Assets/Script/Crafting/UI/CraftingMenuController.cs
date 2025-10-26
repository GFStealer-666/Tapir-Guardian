using System.Collections.Generic;
using UnityEngine;

[DisallowMultipleComponent]
public class CraftingMenuController : MonoBehaviour
{
    [Header("Menu Root")]
    [Tooltip("Top-level GameObject of the crafting menu UI.")]
    [SerializeField] private GameObject menuRoot;

    [Header("Data")]
    [Tooltip("Recipes shown in the list when opening the menu.")]
    [SerializeField] private List<CraftingRecipeSO> recipes = new();

    [Header("Left List (ScrollView)")]
    [SerializeField] private RectTransform listContent;         // ScrollView Content
    [SerializeField] private CraftingListItemUI listItemPrefab; // prefab with Button+Image+TMP

    [Header("Right Detail Panel")]
    [SerializeField] private CraftingDetailPanel detailPanel;   // title/ingredients/craft button

    [Header("Optional (for craftable badge)")]
    [SerializeField] private CraftingSystem crafting;           // auto-found if null
    [SerializeField] private InventoryComponent inventory;      // auto-found if null

    private readonly List<CraftingListItemUI> _spawned = new();
    private CraftingRecipeSO _selected;

    // ===== Public API (hook your buttons to these) =====
    public void Open()
    {
        if (!menuRoot) return;

        if (!crafting)  crafting  = GetComponentInParent<CraftingSystem>();
        if (!inventory) inventory = GetComponentInParent<InventoryComponent>();

        menuRoot.SetActive(true);
        BuildList();
        SelectDefaultRecipe();
        HookSignals(true);
    }

    public void Close()
    {
        if (!menuRoot) return;
        HookSignals(false);
        menuRoot.SetActive(false);
    }

    public void Toggle()
    {
        if (!menuRoot) return;
        if (menuRoot.activeSelf) Close(); else Open();
    }

    public void SetRecipes(IEnumerable<CraftingRecipeSO> list, bool rebuild = true)
    {
        recipes = new List<CraftingRecipeSO>(list);
        if (menuRoot && menuRoot.activeSelf && rebuild) BuildList();
    }

    // ===== Internals =====
    private void HookSignals(bool on)
    {
        if (inventory == null) return;
        if (on)  inventory.OnItemChanged += OnInventoryChanged;
        else     inventory.OnItemChanged -= OnInventoryChanged;
    }

    private void OnInventoryChanged()
    {
        RefreshRowBadges();
        detailPanel?.SetRecipe(_selected);
    }

    private void BuildList()
    {
        // clear old
        for (int i = _spawned.Count - 1; i >= 0; i--)
            if (_spawned[i]) Destroy(_spawned[i].gameObject);
        _spawned.Clear();

        if (!listContent || !listItemPrefab) return;

        foreach (var r in recipes)
        {
            var row = Instantiate(listItemPrefab, listContent);
            row.Bind(r);
            row.button.onClick.AddListener(() => OnRowClicked(row));
            _spawned.Add(row);
        }

        RefreshRowBadges();
    }

    private void SelectDefaultRecipe()
    {
        CraftingRecipeSO pick = null;

        if (crafting != null)
            foreach (var r in recipes)
                if (crafting.CanStart(r)) { pick = r; break; }

        if (pick == null && recipes.Count > 0) pick = recipes[0];
        SelectRecipe(pick);
    }

    private void OnRowClicked(CraftingListItemUI row)
    {
        if (!row) return;
        SelectRecipe(row.boundRecipe);
    }

    private void SelectRecipe(CraftingRecipeSO recipe)
    {
        _selected = recipe;
        if (detailPanel) detailPanel.SetRecipe(recipe);
        UpdateSelectionHighlight(recipe);
    }

    private void UpdateSelectionHighlight(CraftingRecipeSO recipe)
    {
        foreach (var row in _spawned)
        {
            bool isSel = row && row.boundRecipe == recipe;
            if (row && row.button) row.button.interactable = !isSel; // simple highlight
        }
    }

    private void RefreshRowBadges()
    {
        if (crafting == null) return;

        foreach (var row in _spawned)
        {
            if (!row || row.boundRecipe == null) continue;
            bool can = crafting.CanStart(row.boundRecipe);

            var dot = row.transform.Find("CraftableDot"); // optional child
            if (dot) dot.gameObject.SetActive(can);
        }
    }
}
