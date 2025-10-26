using UnityEngine;
using System.Collections;

public class SpriteSequenceAnimator : MonoBehaviour
{
    [SerializeField] private SpriteRenderer[] frames; // assign in inspector
    [SerializeField] private float frameDuration = 0.2f; // seconds per frame
    [SerializeField] private bool loop = true;

    private int currentIndex = 0;
    private Coroutine animRoutine;

    private void OnEnable()
    {
        if (animRoutine != null) StopCoroutine(animRoutine);
        animRoutine = StartCoroutine(PlayAnimation());
    }

    private void OnDisable()
    {
        if (animRoutine != null) StopCoroutine(animRoutine);
        SetAllInactive();
    }

    private IEnumerator PlayAnimation()
    {
        while (true)
        {
            // safety check
            if (frames.Length == 0) yield break;

            // show current
            SetAllInactive();
            frames[currentIndex].enabled = true;

            yield return new WaitForSeconds(frameDuration);

            // move to next
            currentIndex = (currentIndex + 1) % frames.Length;

            // stop if not looping and reached end
            if (!loop && currentIndex == 0)
                break;
        }
    }

    private void SetAllInactive()
    {
        foreach (var s in frames)
        {
            if (s) s.enabled = false;
        }
    }
}
