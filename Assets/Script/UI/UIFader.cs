using UnityEngine;
using System.Collections;
using UnityEngine.Events;
using System;

[RequireComponent(typeof(CanvasGroup))]
public class UIFader : MonoBehaviour
{
    [SerializeField] CanvasGroup cg;
    [SerializeField] private float fadedelay = 3f;
    public event Action OnFadeOutComplete;
    void Awake()
    {
        cg = GetComponent<CanvasGroup>();
        if (!cg) cg = gameObject.AddComponent<CanvasGroup>();
    }

    public IEnumerator FadeOutCoroutine(float t)
    {
        float a0 = cg.alpha;
        float t0 = Time.unscaledTime;
        while (Time.unscaledTime - t0 < t)
        {
            float k = (Time.unscaledTime - t0) / t;
            cg.alpha = Mathf.Lerp(a0, 1f, k);
            yield return null;
        }
        cg.alpha = 1f;
    }

    public IEnumerator FadeInCoroutine(float t)
    {
        float a0 = cg.alpha;
        float t0 = Time.unscaledTime;
        while (Time.unscaledTime - t0 < t)
        {
            float k = (Time.unscaledTime - t0) / t;
            cg.alpha = Mathf.Lerp(a0, 0f, k);
            yield return null;
        }
        cg.alpha = 0f;
    }

    public void FadeOutDoFadeIn(float outT, float inT, System.Action midAction)
    {
        StartCoroutine(FadeOutDoFadeInRoutine(outT, inT, midAction));
    }

    private IEnumerator FadeOutDoFadeInRoutine(float outT, float inT, System.Action midAction)
    {
        // Fade out
        yield return StartCoroutine(FadeOutCoroutine(outT));
        midAction?.Invoke();
        yield return new WaitForSecondsRealtime(fadedelay);
    
        // Fade back in
        yield return StartCoroutine(FadeInCoroutine(inT));
        OnFadeOutComplete?.Invoke();
    }
}