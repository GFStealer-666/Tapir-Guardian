using System.Collections.Generic;
using UnityEngine;

[DisallowMultipleComponent]
public class InventoryMenuController : MonoBehaviour
{
    [Header("Menu Root")]
    [SerializeField] private GameObject menuRoot;

    [Header("Data")]
    [SerializeField] private InventoryComponent inventory;
    [SerializeField] private ItemDatabaseSO database;     // optional
    [SerializeField] private bool aggregateStacks = true; // one tile per itemId

    [Header("UI")]
    [SerializeField] private RectTransform gridRoot;      // left ScrollView Content / grid
    [SerializeField] private InventorySlotUI slotPrefab;  // the square tile prefab
    [SerializeField] private InventoryDetailPanel detail; // the right panel

    private readonly List<InventorySlotUI> spawned = new();
    private InventorySlotUI selected;

    // ----- Hook these from your Open/Close buttons -----
    public void Open()
    {
        if (!menuRoot) return;
        if (!inventory) inventory = GetComponentInParent<InventoryComponent>();
        menuRoot.SetActive(true);
        BuildGrid();
        SelectFirst();
    }

    public void Close()
    {
        ClearGrid();
        if (menuRoot) menuRoot.SetActive(false);
        selected = null;
    }

    // ----- internals -----
    private void BuildGrid()
    {
        ClearGrid();
        if (!gridRoot || !slotPrefab || inventory == null) return;

        // gather items
        var items = new List<(ItemSO so, int count)>();
        if (aggregateStacks)
        {
            var totals = new Dictionary<string, int>(32);
            foreach (var st in inventory.Stacks)
            {
                if (string.IsNullOrEmpty(st.ItemId) || st.Count <= 0) continue;
                totals[st.ItemId] = (totals.TryGetValue(st.ItemId, out var c) ? c : 0) + st.Count;
            }
            foreach (var kv in totals)
            {
                var so = Resolve(kv.Key);
                if (so) items.Add((so, kv.Value));
            }
        }
        else
        {
            foreach (var st in inventory.Stacks)
            {
                if (string.IsNullOrEmpty(st.ItemId) || st.Count <= 0) continue;
                var so = Resolve(st.ItemId);
                if (so) items.Add((so, st.Count));
            }
        }

        // optional sort by name
        items.Sort((a, b) => string.Compare(a.so.DisplayName, b.so.DisplayName, System.StringComparison.Ordinal));

        // spawn rows
        foreach (var (so, cnt) in items)
        {
            var slot = Instantiate(slotPrefab, gridRoot);
            slot.Bind(so, cnt);
            slot.OnClicked += OnSlotClicked;
            spawned.Add(slot);
        }
    }

    private void ClearGrid()
    {
        for (int i = spawned.Count - 1; i >= 0; i--)
        {
            if (spawned[i])
            {
                spawned[i].OnClicked -= OnSlotClicked;
                Destroy(spawned[i].gameObject);
            }
        }
        spawned.Clear();
    }

    private void OnSlotClicked(InventorySlotUI slot)
    {
        Debug.Log("Slot clicked: " + slot.ItemId);
        if (selected) selected.SetHighlight(false);
        selected = slot;
        if (selected) selected.SetHighlight(true);

        if (detail) detail.ShowItem(slot.ItemId);
    }

    private void SelectFirst()
    {
        if (spawned.Count == 0) { if (detail) detail.ShowItem(null); return; }
        OnSlotClicked(spawned[0]);
    }

    private ItemSO Resolve(string id)
    {
        if (!string.IsNullOrEmpty(id))
        {
            if (database) return database.Get(id);
            if (inventory) return inventory.Resolve(id);
        }
        return null;
    }
}
