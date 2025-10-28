using UnityEngine;

public class PlayerAudioSoundEffect : MonoBehaviour
{
    public AudioSource audioSource;
    [SerializeField] private AudioClip walkSound;
    [SerializeField] private AudioClip damagedSound;
    [SerializeField] private AudioClip diedSound;
    [SerializeField] private AudioClip speakSound;
    [SerializeField] private AudioClip jumpSound;
    [SerializeField, Min(0.01f)] private float stepPitchJitter = 0.05f; // +/- pitch jitter
    [SerializeField, Range(0.5f, 2f)] private float baseStepPitch = 1.0f;

    private void OnEnable()
    {

    }

    private void OnDisable()
    {
        
    }
    private void Awake()
    {
        if (audioSource == null)
        {
            audioSource = GetComponent<AudioSource>();
        }
    }
    public void PlaySoundEffect(AudioClip clip)
    {
        if (clip != null && audioSource != null)
        {
            audioSource.PlayOneShot(clip);
        }
    }

    public void PlayWalkStep()
    {
        if (!audioSource || !walkSound) return;
        float p = baseStepPitch + Random.Range(-stepPitchJitter, stepPitchJitter);
        audioSource.pitch = p;
        audioSource.PlayOneShot(walkSound);
        audioSource.pitch = 1f; // reset
    }

    public void PlayJump()
    {
        // Prefer 'jumpSound' if set; otherwise reuse 'speakSound' or 'damagedSound' as a placeholder
        var clip = jumpSound ? jumpSound : speakSound;
        PlaySoundEffect(clip);
    }

    private void PlayWalkSound()
    {
        PlaySoundEffect(walkSound);
    }

    private void PlayDamagedSound()
    {
        PlaySoundEffect(damagedSound);
    }

    private void PlayDiedSound()
    {
        PlaySoundEffect(diedSound);
    }

    private void PlaySpeakSound()
    {
        PlaySoundEffect(speakSound);
    }
}
