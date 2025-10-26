using UnityEngine;
using UnityEngine.UI;
using System.Collections;
public class FadingUI : MonoBehaviour
{
    [SerializeField] private float fadeDuration = 1f; // seconds
    private Image fadeImage;
    private Coroutine fadeRoutine;

    private void Awake()
    {
        fadeImage = GetComponent<Image>();
        fadeImage.color = new Color(0, 0, 0, 0); // start transparent black
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
            float alpha = Mathf.Clamp01(t / fadeDuration);
            fadeImage.color = new Color(0, 0, 0, alpha);
            yield return null;
        }

        fadeImage.color = new Color(0, 0, 0, 1f);
    }
}
