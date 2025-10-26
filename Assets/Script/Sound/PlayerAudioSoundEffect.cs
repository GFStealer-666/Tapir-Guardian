using UnityEngine;

public class PlayerAudioSoundEffect : MonoBehaviour
{
    public AudioSource audioSource;
    [SerializeField] private AudioClip walkSound;
    [SerializeField] private AudioClip damagedSound;
    [SerializeField] private AudioClip diedSound;
    [SerializeField] private AudioClip speakSound;

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
