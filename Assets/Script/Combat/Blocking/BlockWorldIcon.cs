using UnityEngine;

/// <summary>
/// World-space SpriteRenderer feedback:
/// - Shows shield when block starts (pressing/holding)
/// - When a hit is actually blocked, swaps to normal or parry icon
/// - Hides when block ends
/// </summary>
[DisallowMultipleComponent]
public class BlockWorldIcon : MonoBehaviour
{
    [Header("Hub & Follow")]
    [SerializeField] private BlockSignalHub hub;
    [SerializeField] private Transform followTarget;
    [SerializeField] private Vector3 worldOffset = new Vector3(0f, 1.2f, 0f);

    [Header("Renderer & Sprites")]
    [SerializeField] private SpriteRenderer iconRenderer; // world-space
    [SerializeField] private Sprite shieldSprite;         // shown on block start (press)
    [SerializeField] private Sprite normalBlockSprite;    // shown when a normal blocked hit occurs
    [SerializeField] private Sprite parrySprite;          // shown when a perfect blocked hit occurs

    [Header("Sorting")]
    [SerializeField] private string sortingLayerName = "Default";
    [SerializeField] private int sortingOrder = 50;

    [Header("FX")]
    [SerializeField] private bool pulseOnParry = true;
    [SerializeField, Min(0f)] private float pulseHz = 6f;
    [SerializeField, Range(0f, 0.5f)] private float pulseScale = 0.15f;

    private Vector3 baseScale;

    private void Reset()
    {
        if (!hub) hub = GetComponentInParent<BlockSignalHub>() ?? GetComponent<BlockSignalHub>();
        if (!followTarget) followTarget = transform;
    }

    private void Awake()
    {
        if (!hub) hub = GetComponentInParent<BlockSignalHub>() ?? GetComponent<BlockSignalHub>();
        if (!followTarget) followTarget = transform;

        if (!iconRenderer)
        {
            var go = new GameObject("BlockIconSprite");
            go.transform.SetParent(followTarget, false);
            go.transform.localPosition = worldOffset;
            iconRenderer = go.AddComponent<SpriteRenderer>();
        }

        iconRenderer.sortingLayerName = sortingLayerName;
        iconRenderer.sortingOrder = sortingOrder;
        baseScale = iconRenderer.transform.localScale;
        SetVisible(false);
    }

    private void OnEnable()
    {
        if (!hub) return;
        hub.OnBlockStarted += HandleBlockStarted;
        hub.OnBlockEnded   += HandleBlockEnded;
        hub.OnBlockHit     += HandleBlockHit;
    }

    private void OnDisable()
    {
        if (!hub) return;
        hub.OnBlockStarted -= HandleBlockStarted;
        hub.OnBlockEnded   -= HandleBlockEnded;
        hub.OnBlockHit     -= HandleBlockHit;
    }

    private void LateUpdate()
    {
        if (!iconRenderer || !followTarget) return;
        iconRenderer.transform.position = followTarget.position + worldOffset;

        // Parry pulse
        if (iconRenderer.enabled && iconRenderer.sprite == parrySprite && pulseOnParry)
        {
            float s = 1f + Mathf.Sin(Time.unscaledTime * Mathf.PI * 2f * pulseHz) * pulseScale;
            iconRenderer.transform.localScale = baseScale * s;
        }
    }

    private void HandleBlockStarted()
    {
        if (!iconRenderer) return;
        iconRenderer.sprite = shieldSprite;
        ResetScale();
        SetVisible(true);
    }

    private void HandleBlockHit(bool isPerfect)
    {
        if (!iconRenderer) return;
        iconRenderer.sprite = isPerfect ? parrySprite : normalBlockSprite;
        ResetScale();
        SetVisible(true);
    }

    private void HandleBlockEnded()
    {
        SetVisible(false);
        ResetScale();
    }

    private void ResetScale()
    {
        if (iconRenderer) iconRenderer.transform.localScale = baseScale;
    }

    private void SetVisible(bool v)
    {
        if (!iconRenderer) return;
        iconRenderer.enabled = v;
        var c = iconRenderer.color;
        c.a = v ? 1f : 0f;
        iconRenderer.color = c;
    }
}
