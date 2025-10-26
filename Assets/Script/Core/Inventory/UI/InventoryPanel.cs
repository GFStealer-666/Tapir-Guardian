using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Grid inventory view like the mockup. Reads from InventoryComponent,
/// resolves icons via ItemDatabaseSO, aggregates stacks per itemId,
/// and fills a fixed number of visual slots.
/// </summary>
[DisallowMultipleComponent]
public class InventoryPanel : MonoBehaviour
{
    [Header("Data")]
    [SerializeField] private InventoryComponent inventory;   // auto-found if null
    [SerializeField] private ItemDatabaseSO database;        // optional (will use inventory.Resolve if null)

    [Header("UI")]
    [SerializeField] private RectTransform gridRoot;         // parent with GridLayoutGroup
    [SerializeField] private InventorySlotUI slotPrefab;
    [SerializeField, Min(1)] private int visibleSlots = 16;  // set 4 for your mockup, or more

    [Header("Options")]
    [Tooltip("Combine multiple stacks of the same item into one tile with total count.")]
    [SerializeField] private bool aggregateStacks = true;

    private readonly List<InventorySlotUI> _slots = new();

    private void Awake()
    {
        if (!inventory) inventory = GetComponentInParent<InventoryComponent>();
        if (!database && inventory) { /* optional: rely on inventory.Resolve */ }
    }

    private void OnEnable()
    {
        EnsureSlots();
        Refresh();
        if (inventory) inventory.OnItemChanged += Refresh;
    }

    private void OnDisable()
    {
        if (inventory) inventory.OnItemChanged -= Refresh;
    }

    // --- Build grid once ---
    private void EnsureSlots()
    {
        if (!gridRoot || !slotPrefab) return;

        // Destroy extras if we reduced visibleSlots
        for (int i = _slots.Count - 1; i >= visibleSlots; i--)
        {
            if (_slots[i]) Destroy(_slots[i].gameObject);
            _slots.RemoveAt(i);
        }

        // Spawn missing
        while (_slots.Count < visibleSlots)
        {
            var s = Instantiate(slotPrefab, gridRoot);
            _slots.Add(s);
        }
    }

    // --- Fill tiles from inventory ---
    public void Refresh()
    {
        if (_slots.Count == 0) EnsureSlots();
        if (_slots.Count == 0) return;

        // Build a map itemId -> count
        var items = new List<(ItemSO so, int count)>();

        if (inventory != null)
        {
            if (aggregateStacks)
            {
                var totals = new Dictionary<string, int>(32);
                foreach (var st in inventory.Stacks)
                {
                    if (string.IsNullOrEmpty(st.ItemId) || st.Count <= 0) continue;
                    if (!totals.TryGetValue(st.ItemId, out var c)) c = 0;
                    totals[st.ItemId] = c + st.Count;
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
        }

        // Optional: sort (by name)
        items.Sort((a, b) => string.Compare(a.so.DisplayName, b.so.DisplayName, System.StringComparison.Ordinal));

        // Fill tiles
        int iItem = 0;
        for (int i = 0; i < _slots.Count; i++)
        {
            if (iItem < items.Count)
            {
                var (so, cnt) = items[iItem++];
                _slots[i].Bind(so, cnt);
            }
            else
            {
                _slots[i].Clear();
            }
        }
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

    // Public API if you want to change capacity at runtime
    public void SetVisibleSlots(int count)
    {
        visibleSlots = Mathf.Max(1, count);
        EnsureSlots();
        Refresh();
    }
}
