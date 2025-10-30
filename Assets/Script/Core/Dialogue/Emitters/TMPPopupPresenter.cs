using TMPro;
using UnityEngine;
using System.Collections;

[DisallowMultipleComponent]
public class TMPPopupPresenter : MonoBehaviour, IPopupPresenter
{
    [Header("Refs")]
    [SerializeField] private TextMeshProUGUI promptLabel;
    [SerializeField] private CanvasGroup canvasGroup;

    [Header("Animation")]
    [SerializeField, Min(0.001f)] private float fadeIn = 0.15f;
    [SerializeField, Min(0.001f)] private float fadeOut = 0.15f;
    [SerializeField, Min(0.0f)] private float charInterval = 0.0f;

    [Header("Sound")]
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip typeSound;
    [SerializeField, Range(0f, 1f)] private float typeVolume = 0.3f;
    [SerializeField, Range(1, 8)] private int soundEveryNChars = 2;
    // play sound every N letters (e.g., 2 = every 2nd letter)

    [Header("Style")]

    [SerializeField] private bool useSound = true;

    private Coroutine _fadeRoutine;
    private Coroutine _typeRoutine;

    private void Awake()
    {
        if(promptLabel == null)
        {
            promptLabel = GetComponentInChildren<TextMeshProUGUI>();
        }

        if (!canvasGroup) canvasGroup = GetComponent<CanvasGroup>();
        if (canvasGroup) canvasGroup.alpha = 0f;
        if (promptLabel) promptLabel.text = "";
        promptLabel.text = promptLabel.GetParsedText();
        if (!audioSource && typeSound)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
            audioSource.playOnAwake = false;
        }
    }

    public void ShowNow(PopupRequest request)
    {
        if (_fadeRoutine != null) StopCoroutine(_fadeRoutine);
        if (_typeRoutine != null) StopCoroutine(_typeRoutine);


        _fadeRoutine = StartCoroutine(FadeTo(1f, fadeIn));
        _typeRoutine = StartCoroutine(TypeText(request.Text));
    }

    public void Hide()
    {
        if (_fadeRoutine != null) StopCoroutine(_fadeRoutine);
        if (_typeRoutine != null) StopCoroutine(_typeRoutine);
        _fadeRoutine = StartCoroutine(FadeTo(0f, fadeOut));
    }

    private IEnumerator FadeTo(float target, float time)
    {
        if (!canvasGroup) yield break;
        float start = canvasGroup.alpha;
        float t = 0f;
        while (t < time)
        {
            t += Time.deltaTime;
            canvasGroup.alpha = Mathf.Lerp(start, target, t / time);
            yield return null;
        }
        canvasGroup.alpha = target;
    }

    private IEnumerator TypeText(string text)
    {
        if (!promptLabel) yield break;
        promptLabel.text = "";
        int charCount = 0;
        
        if (charInterval <= 0f)
        {
            promptLabel.text = text;
            yield break;
        }
        foreach (char c in text)
        {
            promptLabel.text += c;
            charCount++;
            
            if (useSound && typeSound && audioSource && charCount % soundEveryNChars == 0)
            {
                audioSource.pitch = Random.Range(0.95f, 1.05f); // subtle variation
                audioSource.PlayOneShot(typeSound, typeVolume);
            }

            yield return new WaitForSeconds(charInterval);
        }
    }
}
