using UnityEngine;
using System.Collections;

[RequireComponent(typeof(CanvasGroup))]
public class UIFader : MonoBehaviour
{
    [SerializeField] CanvasGroup cg;

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

        yield return new WaitForSecondsRealtime(2f);
        midAction?.Invoke();

        // Fade back in
        yield return StartCoroutine(FadeInCoroutine(inT));
    }
}