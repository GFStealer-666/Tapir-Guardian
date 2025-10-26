using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// Right-side detail panel:
/// - Title: output name
/// - Icon (optional): output icon
/// - Ingredient list (dynamic): rows of "name have/need"
/// - Craft button (uses CraftingSystem) + optional timer/progress
/// </summary>
public class CraftingDetailPanel : MonoBehaviour
{
    [Header("Systems")]
    [SerializeField] private InventoryComponent inventory;   // auto-found if null
    [SerializeField] private CraftingSystem crafting;        // auto-found if null

    [Header("UI")]
    [SerializeField] private TMP_Text titleText;
    [SerializeField] private Image outputIcon;

    [Header("Ingredients")]
    [SerializeField] private RectTransform ingredientsRoot;  // vertical layout group container
    [SerializeField] private IngredientRowUI ingredientPrefab;

    [Header("Controls")]
    [SerializeField] private Button craftButton;             // label: "คราฟไอเท็ม"
    [SerializeField] private Slider progressBar;             // optional
    [SerializeField] private TMP_Text timerText;             // optional
    private string stationId; 
    private CraftingRecipeSO recipe;
    private CraftingJob boundJob;
    private readonly List<IngredientRowUI> _rows = new();

    private void Awake()
    {
        if (!inventory) inventory = GetComponentInParent<InventoryComponent>();
        if (!crafting)  crafting  = GetComponentInParent<CraftingSystem>();
    }

    private void OnEnable()
    {
        if (inventory) inventory.OnItemChanged += RefreshStatic;
        if (crafting)  crafting.OnJobsChanged += OnJobsChanged;
        if (craftButton) craftButton.onClick.AddListener(OnCraftClicked);
        RefreshStatic();
    }

    private void OnDisable()
    {
        if (inventory) inventory.OnItemChanged -= RefreshStatic;
        if (crafting)  crafting.OnJobsChanged -= OnJobsChanged;
        if (craftButton) craftButton.onClick.RemoveListener(OnCraftClicked);
        boundJob = null;
    }

    private void Update()
    {
        if (boundJob != null && boundJob.state == CraftState.InProgress)
            UpdateProgress(boundJob);
    }
    public void SetStation(string id) // called by CraftingUIController
    {
        stationId = id;
        // If a recipe is already selected, rebind the job context
        if (recipe && crafting) boundJob = crafting.GetActiveJobFor(recipe, stationId);
        RefreshStatic();
    }
    // -------- Public API --------
    public void SetRecipe(CraftingRecipeSO r)
    {
        recipe = r;
        boundJob = crafting ? crafting.GetActiveJobFor(recipe, stationId) : null;
        RebuildIngredients();
        RefreshStatic();
    }

    // -------- Internals --------
    private void OnCraftClicked()
    {
        if (!crafting || !recipe) return;
        if (boundJob != null && boundJob.state == CraftState.InProgress) return;

        var started = crafting.StartJob(recipe, stationId);
        boundJob = started;
        RefreshStatic();
        if (started != null) UpdateProgress(started);
    }

    private void OnJobsChanged()
    {
        if (recipe == null || crafting == null) return;
        boundJob = crafting.GetActiveJobFor(recipe);
        RefreshStatic();
    }

    private void RebuildIngredients()
    {
        // clear rows
        for (int i = _rows.Count - 1; i >= 0; i--)
            if (_rows[i]) Destroy(_rows[i].gameObject);
        _rows.Clear();

        if (!ingredientsRoot || !ingredientPrefab || recipe == null || recipe.inputs == null)
            return;

        foreach (var ing in recipe.inputs)
        {
            var row = Instantiate(ingredientPrefab, ingredientsRoot);
            _rows.Add(row);
        }
    }

    private void RefreshStatic()
    {
        // Title & output
        if (titleText) titleText.text = recipe && recipe.outputItem ? recipe.outputItem.DisplayName : "-";
        if (outputIcon) outputIcon.sprite = recipe && recipe.outputItem ? recipe.outputItem.Icon : null;

        // Ingredients have/need
        if (recipe != null && recipe.inputs != null)
        {
            for (int i = 0; i < _rows.Count && i < recipe.inputs.Length; i++)
            {
                var ing = recipe.inputs[i];
                int have = (inventory != null && !string.IsNullOrEmpty(ing.itemId)) ? inventory.GetCount(ing.itemId) : 0;
                var item = ing.item ? ing.item : (inventory ? inventory.Resolve(ing.itemId) : null);
                var name = item ? item.DisplayName : "-";
                var icon = item ? item.Icon : null;

                _rows[i].gameObject.SetActive(true);
                _rows[i].Bind(icon, name, have, ing.count);
            }
        }

        // Button state
        bool craftingNow = boundJob != null && boundJob.state == CraftState.InProgress;
        if (craftButton)
            craftButton.interactable = !craftingNow && crafting && recipe && crafting.CanStart(recipe);

        // Static progress/time
        if (progressBar) progressBar.value = craftingNow ? boundJob.Progress01 : 0f;
        if (timerText)
            timerText.text = craftingNow
                ? FormatTime(boundJob.Remaining)
                : (recipe != null && recipe.craftSeconds > 0f ? FormatTime(recipe.craftSeconds) : "");
    }

    private void UpdateProgress(CraftingJob job)
    {
        if (progressBar) progressBar.value = job.Progress01;
        if (timerText)   timerText.text   = FormatTime(job.Remaining);
    }

    private static string FormatTime(float seconds)
    {
        seconds = Mathf.Max(0f, seconds);
        int m = (int)(seconds / 60f);
        int s = Mathf.RoundToInt(seconds % 60f);
        return m > 0 ? $"{m}:{s:00}" : $"{s}";
    }
}
