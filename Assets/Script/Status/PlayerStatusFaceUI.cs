using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[DisallowMultipleComponent]
public class PlayerStatusFaceUI : MonoBehaviour
{
    [Header("Binding")]
    [SerializeField] private StatusComponent target;
    [SerializeField] private Image faceImage;

    [Header("Sprites")]
    [SerializeField] private Sprite normalSprite;
    [SerializeField] private Sprite poisonSprite;
    [SerializeField] private Sprite slowSprite;

    [Tooltip("Return to normalSprite when there is no relevant status active.")]
    [SerializeField] private bool revertToNormalWhenClear = true;

    // Track when each status was last applied so "latest wins"
    private readonly Dictionary<StatusSO, float> _lastAppliedTime = new();

    void OnEnable()
    {
        if (!target) return;
        target.OnApplied    += HandleApplied;
        target.OnRemoved    += _ => Refresh();
        target.OnAnyChanged += Refresh;
        Refresh();
    }

    void OnDisable()
    {
        if (!target) return;
        target.OnApplied    -= HandleApplied;
        target.OnRemoved    -= _ => Refresh();     // Note: different lambda instance; safe since we're disabling
        target.OnAnyChanged -= Refresh;
        _lastAppliedTime.Clear();
    }

    private void HandleApplied(StatusInstance inst)
    {
        if (inst?.def)
            _lastAppliedTime[inst.def] = Time.unscaledTime;  // record recency
        Refresh();
    }

    private void Refresh()
    {
        if (!faceImage) return;

        // Choose the *latest* relevant status (Poison or Slow)
        StatusSO winner = null;
        float bestTime = float.NegativeInfinity;

        var active = target ? target.Active : null;
        if (active != null)
        {
            for (int i = 0; i < active.Count; i++)
            {
                var inst = active[i];
                if (inst?.def == null) continue;

                // consider only Poison/Slow
                if (inst.def.kind != StatusKind.Poison && inst.def.kind != StatusKind.Slow)
                    continue;

                float t;
                if (!_lastAppliedTime.TryGetValue(inst.def, out t))
                {
                    // If we never saw OnApplied (edge cases), consider it as very old
                    t = -1f;
                }

                if (t > bestTime)
                {
                    bestTime = t;
                    winner = inst.def;
                }
            }
        }

        if (!winner)
        {
            if (revertToNormalWhenClear && normalSprite)
                faceImage.sprite = normalSprite;
            return;
        }

        switch (winner.kind)
        {
            case StatusKind.Poison:
                if (poisonSprite) faceImage.sprite = poisonSprite;
                break;
            case StatusKind.Slow:
                if (slowSprite) faceImage.sprite = slowSprite;
                break;
            default:
                // Unknown kinds fall back to normal
                if (revertToNormalWhenClear && normalSprite)
                    faceImage.sprite = normalSprite;
                break;
        }
    }
}
