using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

/// <summary>
/// Runtime status driver: ticks poison damage into HealthComponent
/// and pushes slow multiplier into SpeedModifierStack.
/// </summary>
[DisallowMultipleComponent]
public class StatusComponent : MonoBehaviour
{
    [Header("Optional bindings (auto-found if null)")]
    [SerializeField] private HealthComponent health;              // uses TakeDamage(int)
    [SerializeField] private SpeedModifierStack speedStack;       // uses SetModifier/RemoveModifier

    [Header("Global rules")]
    [Tooltip("Minimal allowed movement multiplier when slowed (e.g., 0.15 = 85% max slow).")]
    [Range(0.01f, 1f)] public float minMoveMultiplier = 0.15f;

    [Tooltip("Round poison damage up to int per tick (ceil). If false, rounds normally.")]
    public bool ceilPoisonDamage = true;

    // --- State ---
    [SerializeField] private List<StatusInstance> _active = new();
    public IReadOnlyList<StatusInstance> Active => _active;

    // Events for UI/VFX
    public event Action<StatusInstance> OnApplied;
    public event Action<StatusInstance> OnRemoved;
    public event Action OnAnyChanged;

    // cache: last-applied slow multiplier we sent to SpeedModifierStack
    private float _lastSlowMult = 1f;

    private const string kSlowLabel = "StatusSlow";

    void Awake()
    {
        if (!health)     health     = GetComponentInParent<HealthComponent>() ?? GetComponent<HealthComponent>();
        if (!speedStack) speedStack = GetComponentInParent<SpeedModifierStack>() ?? GetComponent<SpeedModifierStack>();
    }

    void OnDisable()
    {
        // make sure we clear our speed modifier when this turns off
        if (speedStack) speedStack.RemoveModifier(this);
        _lastSlowMult = 1f;
    }

    void Update()
    {
        float dt = Time.deltaTime;
        bool changed = false;

        // Tick & expire
        for (int i = _active.Count - 1; i >= 0; --i)
        {
            var inst = _active[i];
            bool expired = inst.UpdateTimers(dt, HandleTick);
            if (expired)
            {
                var removed = inst;
                _active.RemoveAt(i);
                OnRemoved?.Invoke(removed);
                changed = true;
            }
        }

        if (changed) RecalcAggregatesAndPush();
    }

    // -------- Public API --------
    public void Apply(StatusSO def)
    {
        if (!def) return;

        var existing = _active.FirstOrDefault(s => s.def == def);
        switch (def.stackPolicy)
        {
            case StackPolicy.RefreshDuration:
                if (existing != null) existing.RefreshDuration();
                else _active.Add(new StatusInstance(def));
                break;

            case StackPolicy.StackIntensity:
                if (existing != null) existing.AddStack();
                else _active.Add(new StatusInstance(def));
                break;

            case StackPolicy.IgnoreIfPresent:
                if (existing == null) _active.Add(new StatusInstance(def));
                break;
        }

        OnApplied?.Invoke(existing ?? _active.Last());
        RecalcAggregatesAndPush();
    }

    public int RemoveById(string statusId)
    {
        if (string.IsNullOrWhiteSpace(statusId)) return 0;
        int removed = _active.RemoveAll(s => s.def && s.def.id == statusId);
        if (removed > 0) { OnAnyChanged?.Invoke(); RecalcAggregatesAndPush(); }
        return removed;
    }

    public int RemoveByTag(string tag)
    {
        if (string.IsNullOrWhiteSpace(tag)) return 0;
        int removed = _active.RemoveAll(s => s.def && s.def.tags != null && s.def.tags.Contains(tag));
        if (removed > 0) { OnAnyChanged?.Invoke(); RecalcAggregatesAndPush(); }
        return removed;
    }

    public int RemoveDispellable()
    {
        int removed = _active.RemoveAll(s => s.def && s.def.dispellable);
        if (removed > 0) { OnAnyChanged?.Invoke(); RecalcAggregatesAndPush(); }
        return removed;
    }

    // -------- Internals --------
    private void HandleTick(StatusInstance inst)
    {
        if (inst.def == null || health == null || health.IsDead) return;

        switch (inst.def.kind)
        {
            case StatusKind.Poison:
            {
                // magnitude = damage per tick; scale by stacks; convert to int
                float raw = inst.def.magnitude * Mathf.Max(1, inst.stacks);
                int dmg = ceilPoisonDamage ? Mathf.CeilToInt(raw) : Mathf.RoundToInt(raw);
                if (dmg > 0) health.TakeDamage(dmg);
                break;
            }

            // Add other periodic kinds if needed (e.g., Burn, Regen, etc.)
        }
    }

    private void RecalcAggregatesAndPush()
    {
        // ---- SLOW aggregation ----
        // Pick the strongest (lowest) multiplier among all Slow statuses.
        float slowMult = 1f;
        foreach (var s in _active)
        {
            if (s.def == null) continue;
            if (s.def.kind != StatusKind.Slow) continue;

            // magnitude interpreted as "slow percent" in [0..1]
            float slowPercent = Mathf.Clamp01(s.def.magnitude) * Mathf.Max(1, s.stacks);
            float mult = Mathf.Clamp01(1f - slowPercent);
            slowMult = Mathf.Min(slowMult, mult);
        }
        slowMult = Mathf.Clamp(slowMult, minMoveMultiplier, 1f);

        // Push to SpeedModifierStack
        if (speedStack)
        {
            if (slowMult < 0.999f) // has some slow
            {
                if (!Mathf.Approximately(slowMult, _lastSlowMult))
                {
                    speedStack.SetModifier(this, slowMult, kSlowLabel);
                    _lastSlowMult = slowMult;
                }
            }
            else
            {
                // no slow active
                if (!Mathf.Approximately(_lastSlowMult, 1f))
                {
                    speedStack.RemoveModifier(this);
                    _lastSlowMult = 1f;
                }
            }
        }

        OnAnyChanged?.Invoke();
    }
}
