using System.Collections;
using UnityEngine;
using UnityEngine.UI;

// Optional blur (URP):
// If you use URP with a Global Volume, uncomment these two lines:
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

[RequireComponent(typeof(Collider2D))]
public class TriggerTeleportWithFade : MonoBehaviour
{
    [Header("Who & Where")]
    [SerializeField] private string playerTag = "Player";
    [SerializeField] private Transform targetPoint;

    [Header("Fade UI (required)")]
    [SerializeField] private Image fadeImage;         // Fullscreen Image on a Screen Space canvas (black)
    [SerializeField] private float fadeOutDuration = 0.35f;
    [SerializeField] private float holdBlackSeconds = 0.75f;
    [SerializeField] private float fadeInDuration = 0.45f;

    [Header("Optional URP Blur")]
    [SerializeField] private Volume globalVolume;     // Global Volume with Depth of Field (optional)
    [SerializeField] private bool enableBlur = true; // toggle blur behavior

    // Cached:
    private Collider2D _col;
    private DepthOfField _dof; // URP DoF component if available
    private bool _busy;

    private void Reset()
    {
        _col = GetComponent<Collider2D>();
        if (_col) _col.isTrigger = true;
    }

    private void Awake()
    {
        _col = GetComponent<Collider2D>();
        if (_col) _col.isTrigger = true;

        // Ensure fadeImage starts transparent & enabled
        if (fadeImage)
        {
            var c = fadeImage.color;
            fadeImage.color = new Color(c.r, c.g, c.b, 0f);
            fadeImage.raycastTarget = true; // block clicks during fade
        }

        // Try get URP DoF if a volume was assigned
        if (globalVolume && globalVolume.profile)
        {
            globalVolume.profile.TryGet(out _dof);
            if (_dof != null) _dof.active = false; // start off
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (_busy) return;
        if (!other.CompareTag(playerTag)) return;
        if (!targetPoint) return;
        StartCoroutine(FadeTeleportSequence(other.transform));
    }

    private IEnumerator FadeTeleportSequence(Transform player)
    {
        _busy = true;
        
        // Fade to black
        yield return StartCoroutine(FadeAlpha(0f, 1f, fadeOutDuration));
        player.position = targetPoint.position;
        // Optional: enable blur while on black (so the coming fade-in reveals blur briefly)
        if (enableBlur && _dof != null)
        {
            _dof.active = true;
            // If needed, you can tweak these defaults in the Volume Profile via Inspector
            // Example (safe if these exist in your URP version):
            // _dof.mode.value = DepthOfFieldMode.Bokeh;
            // _dof.focusDistance.value = 0.05f;
            // _dof.focalLength.value = 150f;
            // _dof.aperture.value = 1.4f;
        }

        // Hold
        yield return new WaitForSeconds(holdBlackSeconds);

        // Teleport
        

        // Start fade-in; disable blur slightly after we start revealing the scene
        float t = 0f;
        while (t < fadeInDuration)
        {
            t += Time.deltaTime;
            float p = Mathf.Clamp01(t / fadeInDuration);
            SetFadeAlpha(1f - p);

            // Turn off blur near the end of the fade so the scene snaps back to sharp
            if (enableBlur && _dof != null && p >= 0.6f && _dof.active)
                _dof.active = false;

            yield return null;
        }
        SetFadeAlpha(0f);

        _busy = false;
    }

    private IEnumerator FadeAlpha(float from, float to, float duration)
    {
        if (!fadeImage || duration <= 0f)
        {
            SetFadeAlpha(to);
            yield break;
        }

        float t = 0f;
        while (t < duration)
        {
            t += Time.deltaTime;
            float p = Mathf.Clamp01(t / duration);
            float a = Mathf.Lerp(from, to, p);
            SetFadeAlpha(a);
            yield return null;
        }
        SetFadeAlpha(to);
    }

    private void SetFadeAlpha(float a)
    {
        if (!fadeImage) return;
        var c = fadeImage.color;
        fadeImage.color = new Color(c.r, c.g, c.b, a);
    }
}
