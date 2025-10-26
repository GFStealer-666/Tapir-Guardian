using UnityEngine;
using UnityEngine.UI;
using System.Collections;

[RequireComponent(typeof(Image))]
public class FadeInOnEnable : MonoBehaviour
{
    [SerializeField] private float fadeDuration = 1f; // seconds
    private Image fadeImage;
    private Coroutine fadeRoutine;

    private void Awake()
    {
        fadeImage = GetComponent<Image>();
        // make sure it starts fully black
        fadeImage.color = new Color(0, 0, 0, 1);
    }

    private void OnEnable()
    {
        if (fadeRoutine != null) StopCoroutine(fadeRoutine);
        fadeRoutine = StartCoroutine(FadeIn());
    }

    private IEnumerator FadeIn()
    {
        float t = 0f;
        Color c = fadeImage.color;

        while (t < fadeDuration)
        {
            t += Time.deltaTime;
            float alpha = 1f - Mathf.Clamp01(t / fadeDuration); // fade from 1 → 0
            fadeImage.color = new Color(0, 0, 0, alpha);
            yield return null;
        }

        fadeImage.color = new Color(0, 0, 0, 0); // fully transparent at end
    }
}
