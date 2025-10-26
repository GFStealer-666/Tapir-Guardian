using System.Collections.Generic;
using UnityEngine;

public class StatusBarUI : MonoBehaviour
{
    [Header("Binding")]
    public StatusComponent target;
    public Transform iconRoot;
    public StatusIconUI iconPrefab;

    private readonly Dictionary<StatusSO, StatusIconUI> _icons = new();

    void OnEnable()
    {
        if (!target) return;
        target.OnApplied    += HandleChanged;
        target.OnRemoved    += HandleChanged;
        target.OnAnyChanged += RefreshAll;
        RefreshAll();
    }

    void OnDisable()
    {
        if (!target) return;
        target.OnApplied    -= HandleChanged;
        target.OnRemoved    -= HandleChanged;
        target.OnAnyChanged -= RefreshAll;
    }

    private void HandleChanged(StatusInstance _) => RefreshAll();

    private void RefreshAll()
    {
        if (!target || !iconRoot || !iconPrefab) return;

        // Build active set
        var active = target.Active;
        HashSet<StatusSO> activeDefs = new();
        foreach (var inst in active)
            if (inst?.def) activeDefs.Add(inst.def);

        // Remove missing
        var toRemove = new List<StatusSO>();
        foreach (var kv in _icons)
            if (!activeDefs.Contains(kv.Key)) toRemove.Add(kv.Key);
        foreach (var def in toRemove)
        {
            if (_icons.TryGetValue(def, out var ui) && ui)
                Destroy(ui.gameObject);
            _icons.Remove(def);
        }

        // Add / update
        foreach (var inst in active)
        {
            if (inst?.def == null || !inst.def.showOnStatusBar) continue;

            if (!_icons.TryGetValue(inst.def, out var ui) || !ui)
            {
                ui = Instantiate(iconPrefab, iconRoot);
                _icons[inst.def] = ui;
            }
            ui.Bind(inst);
        }
    }

    void Update()
    {
        foreach (var ui in _icons.Values)
            if (ui) ui.UpdateVisual();
    }
}
