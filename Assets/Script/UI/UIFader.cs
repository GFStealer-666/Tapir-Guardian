using UnityEngine;
using System.Collections;

[RequireComponent(typeof(CanvasGroup))]
public class UIFader : MonoBehaviour
{
    CanvasGroup cg;

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
}
