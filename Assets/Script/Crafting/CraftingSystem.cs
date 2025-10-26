using System.Collections.Generic;
using UnityEngine;

[DisallowMultipleComponent]
public class CraftingSystem : MonoBehaviour
{
    [SerializeField] private InventoryComponent inventory;

    // Jobs persist while scene is alive (even if panel is closed)
    [SerializeField] private List<CraftingJob> jobs = new();
    private int nextId = 1;

    public System.Action OnJobsChanged;

    private void Awake()
    {
        if (!inventory) inventory = GetComponent<InventoryComponent>() ?? GetComponentInParent<InventoryComponent>();
    }

    private void Update()
    {
        bool changed = false;

        // Auto-complete jobs that finished
        for (int i = 0; i < jobs.Count; i++)
        {
            var j = jobs[i];
            if (j.state == CraftState.InProgress && j.IsDone)
            {
                // deliver output
                if (j.recipe && j.recipe.outputItem)
                    inventory.Add(j.recipe.outputItem.Id, j.recipe.outputCount);

                j.state = CraftState.Completed;
                jobs[i] = j;
                changed = true;
            }
        }

        if (changed) OnJobsChanged?.Invoke();
    }

    // ===== Queries =====
    public bool CanStart(CraftingRecipeSO recipe)
    {
        if (!recipe || !inventory || !recipe.outputItem) return false;
        foreach (var ing in recipe.inputs)
        {
            if (string.IsNullOrEmpty(ing.itemId) || ing.count <= 0) return false;
            if (!inventory.Has(ing.itemId, ing.count)) return false;
        }
        return true;
    }

    public CraftingJob GetActiveJobFor(CraftingRecipeSO recipe)
    {
        if (!recipe) return null;
        for (int i = 0; i < jobs.Count; i++)
        {
            var j = jobs[i];
            if (j.recipe == recipe && j.state == CraftState.InProgress)
                return j;
        }
        return null;
    }

    // ===== Commands =====
    public CraftingJob StartJob(CraftingRecipeSO recipe)
    {
        if (!CanStart(recipe)) return null;

        // consume inputs upfront
        foreach (var ing in recipe.inputs)
            if (!inventory.Consume(ing.itemId, ing.count))
                return null; // desync guard

        var job = new CraftingJob
        {
            id = nextId++,
            recipe = recipe,
            startTime = Time.time,
            endTime = Time.time + Mathf.Max(0f, recipe.craftSeconds),
            state = (recipe.craftSeconds <= 0f) ? CraftState.Completed : CraftState.InProgress
        };

        jobs.Add(job);

        // instant craft: deliver immediately
        if (job.state == CraftState.Completed && recipe.outputItem)
            inventory.Add(recipe.outputItem.Id, recipe.outputCount);

        OnJobsChanged?.Invoke();
        return job;
    }

    public bool CancelJob(int jobId, bool refund = true)
    {
        int idx = jobs.FindIndex(j => j.id == jobId && j.state == CraftState.InProgress);
        if (idx < 0) return false;

        var job = jobs[idx];

        // optional refund
        if (refund && job.recipe != null)
        {
            foreach (var ing in job.recipe.inputs)
                inventory.Add(ing.itemId, ing.count);
        }

        jobs.RemoveAt(idx);
        OnJobsChanged?.Invoke();
        return true;
    }

    public void RemoveCompleted()
    {
        jobs.RemoveAll(j => j.state == CraftState.Completed);
        OnJobsChanged?.Invoke();
    }
    public CraftingJob GetActiveJobFor(CraftingRecipeSO recipe, string stationId = null)
    {
        if (!recipe) return null;
        for (int i = 0; i < jobs.Count; i++)
        {
            var j = jobs[i];
            if (j.recipe == recipe && j.state == CraftState.InProgress &&
                (stationId == null || j.stationId == stationId))
                return j;
        }
        return null;
    }

    public CraftingJob StartJob(CraftingRecipeSO recipe, string stationId = null)
    {
        if (!CanStart(recipe)) return null;

        foreach (var ing in recipe.inputs)
            if (!inventory.Consume(ing.itemId, ing.count)) return null;

        var duration = Mathf.Max(0f, recipe.craftSeconds);
        var job = new CraftingJob
        {
            id = nextId++,
            recipe = recipe,
            stationId = stationId,                // <— NEW
            startTime = Time.time,
            endTime = Time.time + duration,
            state = (duration <= 0f) ? CraftState.Completed : CraftState.InProgress
        };

        jobs.Add(job);

        if (job.state == CraftState.Completed && recipe.outputItem)
            inventory.Add(recipe.outputItem.Id, recipe.outputCount);

        OnJobsChanged?.Invoke();
        return job;
    }

    public bool CancelAllForStation(string stationId, bool refund = true)
    {
        if (string.IsNullOrEmpty(stationId)) return false;
        bool any = false;

        for (int i = jobs.Count - 1; i >= 0; i--)
        {
            var j = jobs[i];
            if (j.state != CraftState.InProgress) continue;
            if (j.stationId != stationId) continue;

            if (refund && j.recipe != null)
                foreach (var ing in j.recipe.inputs) inventory.Add(ing.itemId, ing.count);

            jobs.RemoveAt(i);
            any = true;
        }

        if (any) OnJobsChanged?.Invoke();
        return any;
    }
}
