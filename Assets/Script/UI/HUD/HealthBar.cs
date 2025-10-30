using System.Collections;
using NUnit.Framework.Constraints;
using UnityEngine;
using UnityEngine.UI;
public class HealthBar : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] private Image healthFill;
    [Header("Object Reference")]
    [SerializeField] private Transform searchRoot;
    [Header("Colors")]
    [SerializeField] private Color fullColor = new Color(0.15f, 0.8f, 0.3f); // green
    [SerializeField] private Color emptyColor = new Color(0.9f, 0.2f, 0.2f); // red
    [Header("Animation")]
    [SerializeField] private float animateDuration = 0.25f;   // seconds
    [SerializeField] private bool useUnscaledTime = false;
    [SerializeField] private AnimationCurve ease = AnimationCurve.EaseInOut(0, 0, 1, 1);
    [Header("Script reference")]
    [SerializeField] private HealthComponent healthComponent; // player health component
    private Coroutine animCo;
    private float displayedValue;
    private int currentMax;
    void Awake()
    {
        if (!healthComponent) healthComponent = GetComponentInParent<HealthComponent>();
        var root = searchRoot == null ? transform : searchRoot;

    }
    void OnEnable()
    {
        if (!healthComponent) return;
        healthComponent.OnHealthChanged += OnHealthChanged;
        OnHealthChanged(healthComponent.CurrentHealth, healthComponent.MaxHealth);
    }

    void OnDisable()
    {
        if (!healthComponent) return;
        healthComponent.OnHealthChanged -= OnHealthChanged;
    }
    private void OnHealthChanged(int current, int max)
    {
        currentMax = max;
        Debug.Log($"current health point {current}");
        if (animCo != null) StopCoroutine(animCo);
        animCo = StartCoroutine(AnimateTo(current, max));

    }
    private IEnumerator AnimateTo(int targetValue, int targetMax)
    {
        float start = displayedValue;
        float dur = Mathf.Max(0.0001f, animateDuration);
        float t = 0f;

        while (t < 1f)
        {
            t += (useUnscaledTime ? Time.unscaledDeltaTime : Time.deltaTime) / dur;
            float k = ease != null ? ease.Evaluate(Mathf.Clamp01(t)) : Mathf.Clamp01(t);

            displayedValue = Mathf.Lerp(start, targetValue, k);
            ApplyVisual(displayedValue, targetMax);
            yield return null;
        }

        displayedValue = targetValue;
        ApplyVisual(displayedValue, targetMax);
        animCo = null;
    }
    private void ApplyVisual(float value, int max)
    {
        float fraction = max > 0 ? Mathf.Clamp01(value / max) : 0f;
        Color c = Color.Lerp(emptyColor, fullColor, fraction);

        if (healthFill)
        {
            healthFill.fillAmount = fraction;
            healthFill.color = c;
        }

    }
}
