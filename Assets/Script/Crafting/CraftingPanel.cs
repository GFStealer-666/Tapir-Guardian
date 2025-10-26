using UnityEngine;
using UnityEngine.UI;
using TMPro;

// Deprecated this file.

/// <summary>
/// Timer-aware crafting panel that matches your sketch:
/// - Left: N input slots (use 2 for your example)
/// - Right: 1 output slot
/// - Button "Craft" (Thai: "สร้าง")
/// - Optional progress bar + timer text
/// Works with CraftingSystem + CraftingRecipeSO + InventoryComponent.
/// </summary>
public class CraftingPanel : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private InventoryComponent inventory;      // auto-found if null
    [SerializeField] private CraftingSystem crafting;           // auto-found if null
    [SerializeField] private CraftingRecipeSO recipe;           // assign at design-time or via SetRecipe()

    [Header("UI - Inputs (left)")]
    [Tooltip("Provide at least as many slots as the recipe has inputs.")]
    [SerializeField] private CraftingSlotUI[] inputSlots;       // size >= recipe.inputs.Length

    [Header("UI - Output (right)")]
    [SerializeField] private CraftingSlotUI outputSlot;

    [Header("UI - Controls")]
    [SerializeField] private Button craftButton;                // label it "สร้าง"
    [SerializeField] private Slider progressBar;                // optional
    [SerializeField] private TMP_Text timerText;                // optional

    private CraftingJob boundJob;

    private void Awake()
    {
        if (!inventory) inventory = GetComponentInParent<InventoryComponent>();
        if (!crafting) crafting = GetComponentInParent<CraftingSystem>();
    }

    private void OnEnable()
    {
        if (inventory) inventory.OnItemChanged += RefreshStatic;
        if (crafting) crafting.OnJobsChanged += OnJobsChanged;
        if (craftButton) craftButton.onClick.AddListener(OnCraftClicked);

        RebindJob();
        RefreshStatic();
    }

    private void OnDisable()
    {
        if (inventory) inventory.OnItemChanged -= RefreshStatic;
        if (crafting) crafting.OnJobsChanged -= OnJobsChanged;
        if (craftButton) craftButton.onClick.RemoveListener(OnCraftClicked);
        boundJob = null;
    }

    private void Update()
    {
        // live progress when a job is running
        if (boundJob != null && boundJob.state == CraftState.InProgress)
            UpdateProgress(boundJob);
    }

    /// <summary>Switch which recipe this panel is showing.</summary>
    public void SetRecipe(CraftingRecipeSO r)
    {
        recipe = r;
        RebindJob();
        RefreshStatic();
    }

    // ---------- Internals ----------

    private void OnCraftClicked()
    {
        if (!crafting || !recipe) return;

        // ignore if already crafting this recipe
        if (boundJob != null && boundJob.state == CraftState.InProgress) return;

        var started = crafting.StartJob(recipe);
        boundJob = started;
        RefreshStatic(); // disables button, sets initial bar/timer
        if (started != null) UpdateProgress(started);
    }

    private void OnJobsChanged()
    {
        // job may complete while panel closed or in background → rebind and refresh
        RebindJob();
        RefreshStatic();
    }

    private void RebindJob()
    {
        boundJob = (crafting && recipe) ? crafting.GetActiveJobFor(recipe) : null;
    }

    private void RefreshStatic()
    {
        if (!recipe || !inventory) return;

        // Inputs
        for (int i = 0; i < inputSlots.Length; i++)
        {
            if (i < (recipe.inputs?.Length ?? 0))
            {
                var ing = recipe.inputs[i];
                int have = string.IsNullOrEmpty(ing.itemId) ? 0 : inventory.GetCount(ing.itemId);

                inputSlots[i].gameObject.SetActive(true);
                inputSlots[i].BindInput(ing.item, have, ing.count);
            }
            else
            {
                inputSlots[i].gameObject.SetActive(false);
            }
        }

        // Output
        if (outputSlot)
            outputSlot.BindOutput(recipe.outputItem, recipe.outputCount);

        // Button state
        bool craftingNow = boundJob != null && boundJob.state == CraftState.InProgress;
        if (craftButton)
            craftButton.interactable = !craftingNow && crafting && crafting.CanStart(recipe);

        // Static bar/time when idle
        if (progressBar) progressBar.value = craftingNow ? boundJob.Progress01 : 0f;
        if (timerText)
            timerText.text = craftingNow
                ? FormatTime(boundJob.Remaining)
                : (recipe.craftSeconds > 0f ? FormatTime(recipe.craftSeconds) : "");
    }

    private void UpdateProgress(CraftingJob job)
    {
        if (progressBar) progressBar.value = job.Progress01;
        if (timerText) timerText.text = FormatTime(job.Remaining);
    }

    private static string FormatTime(float seconds)
    {
        seconds = Mathf.Max(0f, seconds);
        int m = (int)(seconds / 60f);
        int s = Mathf.RoundToInt(seconds % 60f);
        return m > 0 ? $"{m}:{s:00}" : $"{s}";
    }
}
