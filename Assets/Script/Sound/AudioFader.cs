using System.Collections;
using UnityEngine;

public static class AudioFader
{
    public static void Fade(MonoBehaviour host, AudioSource src, float targetVolume, float duration)
    {
        if (!src) return;

        // If host is inactive/disabled, do it instantly (no coroutine allowed).
        if (host == null || !host.isActiveAndEnabled)
        {
            ApplyImmediate(src, targetVolume);
            return;
        }

        host.StartCoroutine(FadeTo(src, targetVolume, duration));
    }

    private static void ApplyImmediate(AudioSource src, float target)
    {
        if (target > 0f && !src.isPlaying) src.Play();
        src.volume = target;
        if (Mathf.Approximately(target, 0f)) src.Pause();
    }

    private static IEnumerator FadeTo(AudioSource src, float targetVolume, float duration)
    {
        if (!src) yield break;
        if (!src.isPlaying && targetVolume > 0f) src.Play();

        float start = src.volume;
        float t = 0f;
        while (t < duration)
        {
            t += Time.deltaTime;
            float k = duration > 0f ? t / duration : 1f;
            src.volume = Mathf.Lerp(start, targetVolume, k);
            yield return null;
        }
        src.volume = targetVolume;
        if (Mathf.Approximately(targetVolume, 0f)) src.Pause();
    }
}
