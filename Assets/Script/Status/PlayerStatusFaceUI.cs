using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[DisallowMultipleComponent]
public class PlayerStatusFaceUI : MonoBehaviour
{
    [Header("Binding")]
    [SerializeField] private StatusComponent target;
    [SerializeField] private HealthComponent health;
    [SerializeField] private Image faceImage;

    [Header("Sprites")]
    [SerializeField] private Sprite normalSprite;
    [SerializeField] private Sprite poisonSprite;
    [SerializeField] private Sprite slowSprite;
    [SerializeField] private Sprite injuredSprite;

    [Header("Rules")]
    [Tooltip("If current HP is below this value, show injuredSprite regardless of status.")]
    [SerializeField] private int injuredHpThreshold = 50;
    [SerializeField] private bool revertToNormalWhenClear = true;

    private readonly Dictionary<StatusSO, float> _lastAppliedTime = new();

    // keep delegate refs so unsubscribe is clean
    private System.Action<StatusInstance> _onApplied;
    private System.Action<StatusInstance> _onRemoved;
    private System.Action _onAnyChanged;
    private System.Action<int,int> _onHealthChanged;

    void OnEnable()
    {
        // Wire Status events
        if (target)
        {
            _onApplied     = HandleApplied;
            _onRemoved     = _ => Refresh();
            _onAnyChanged  = Refresh;

            target.OnApplied    += _onApplied;
            target.OnRemoved    += _onRemoved;
            target.OnAnyChanged += _onAnyChanged;
        }

        // Wire Health events
        if (health)
        {
            _onHealthChanged = (_, __) => Refresh();
            health.OnHealthChanged += _onHealthChanged;
        }

        Refresh();
    }

    void OnDisable()
    {
        if (target)
        {
            if (_onApplied != null)     target.OnApplied    -= _onApplied;
            if (_onRemoved != null)     target.OnRemoved    -= _onRemoved;
            if (_onAnyChanged != null)  target.OnAnyChanged -= _onAnyChanged;
        }
        if (health && _onHealthChanged != null)
            health.OnHealthChanged -= _onHealthChanged;

        _lastAppliedTime.Clear();
        _onApplied = null; _onRemoved = null; _onAnyChanged = null; _onHealthChanged = null;
    }

    private void HandleApplied(StatusInstance inst)
    {
        if (inst?.def) _lastAppliedTime[inst.def] = Time.unscaledTime;
        Refresh();
    }

    private void Refresh()
    {
        if (!faceImage) return;

        // --- Priority 1: Injured override ---
        if (health && injuredSprite && health.CurrentHealth < injuredHpThreshold)
        {
            faceImage.sprite = injuredSprite;
            return;
        }

        // --- Priority 2: Latest status (Poison/Slow) ---
        StatusSO winner = null;
        float bestTime = float.NegativeInfinity;

        var active = target ? target.Active : null;
        if (active != null)
        {
            for (int i = 0; i < active.Count; i++)
            {
                var inst = active[i];
                if (inst?.def == null) continue;

                if (inst.def.kind != StatusKind.Poison && inst.def.kind != StatusKind.Slow)
                    continue;

                float t = _lastAppliedTime.TryGetValue(inst.def, out var when) ? when : -1f;
                if (t > bestTime) { bestTime = t; winner = inst.def; }
            }
        }

        if (!winner)
        {
            if (revertToNormalWhenClear && normalSprite) faceImage.sprite = normalSprite;
            return;
        }

        switch (winner.kind)
        {
            case StatusKind.Poison: if (poisonSprite) faceImage.sprite = poisonSprite; break;
            case StatusKind.Slow:   if (slowSprite)   faceImage.sprite = slowSprite;   break;
            default:
                if (revertToNormalWhenClear && normalSprite) faceImage.sprite = normalSprite;
                break;
        }
    }
}
