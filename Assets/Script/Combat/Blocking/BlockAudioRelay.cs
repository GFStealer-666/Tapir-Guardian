using UnityEngine;

/// <summary>
/// Plays SFX in sync with BlockSignalHub events.
/// - GuardUp (when block begins)
/// - NormalBlock on normal blocked hit
/// - Parry on perfect blocked hit
/// Uses your PlayerAudioSoundEffect to actually play.
/// </summary>
[DisallowMultipleComponent]
public class BlockAudioRelay : MonoBehaviour
{
    [SerializeField] private BlockSignalHub hub;
    [SerializeField] private PlayerAudioSoundEffect audioFX;

    [Header("Clips")]
    [SerializeField] private AudioClip guardUpSfx;     // on block start (press)
    [SerializeField] private AudioClip normalBlockSfx; // when a normal blocked hit occurs
    [SerializeField] private AudioClip parrySfx;       // when a perfect blocked hit occurs

    [Header("Spam Guard")]
    [SerializeField, Min(0f)] private float minInterval = 0.03f;
    private float lastPlay;

    private void Reset()
    {
        if (!hub) hub = GetComponentInParent<BlockSignalHub>() ?? GetComponent<BlockSignalHub>();
        if (!audioFX) audioFX = GetComponentInParent<PlayerAudioSoundEffect>() ?? GetComponent<PlayerAudioSoundEffect>();
    }

    private void Awake()
    {
        if (!hub) hub = GetComponentInParent<BlockSignalHub>() ?? GetComponent<BlockSignalHub>();
        if (!audioFX) audioFX = GetComponentInParent<PlayerAudioSoundEffect>() ?? GetComponent<PlayerAudioSoundEffect>();
    }

    private void OnEnable()
    {
        if (!hub) return;
        hub.OnBlockStarted += HandleBlockStarted;
        hub.OnBlockHit     += HandleBlockHit;
        hub.OnBlockEnded   += HandleBlockEnded;
    }

    private void OnDisable()
    {
        if (!hub) return;
        hub.OnBlockStarted -= HandleBlockStarted;
        hub.OnBlockHit     -= HandleBlockHit;
        hub.OnBlockEnded   -= HandleBlockEnded;
    }

    private void HandleBlockStarted()
    {
        TryPlay(guardUpSfx);
    }

    private void HandleBlockHit(bool isPerfect)
    {
        TryPlay(isPerfect ? parrySfx : normalBlockSfx);
    }

    private void HandleBlockEnded()
    {
        // no sound by default; add one if you want
    }

    private void TryPlay(AudioClip clip)
    {
        if (!clip || !audioFX) return;
        if (Time.time - lastPlay < minInterval) return;
        audioFX.PlaySoundEffect(clip);
        lastPlay = Time.time;
    }
}
