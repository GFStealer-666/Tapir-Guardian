using UnityEngine;

public class UIFadeAction : MonoBehaviour
{
    [SerializeField] private UIFader fader;
    [SerializeField] private GameObject[] objectToActivate;
    [SerializeField] private GameObject objectToDeativate;
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip actionSound , winningSound;

    void OnEnable()
    {
        fader.OnFadeOutComplete += PlayWinningSound;
    }

    void OnDisable()
    {
        fader.OnFadeOutComplete -= PlayWinningSound;
    }
    public void FadeAndActivate()
    {
        if (!fader) return;

        fader.FadeOutDoFadeIn(0.5f, 2f, () =>
        {
            if (objectToActivate != null)
            {
                foreach (var obj in objectToActivate)
                {
                    obj.SetActive(true);
                }
            }
            if (objectToDeativate)
            {
                objectToDeativate.SetActive(false);
            }
            if (audioSource && actionSound)
            {
                audioSource.PlayOneShot(actionSound);
            }
        });
    }

    public void PlayWinningSound()
    {
        if (audioSource && winningSound)
        {
            audioSource.PlayOneShot(winningSound);
        }
        fader.OnFadeOutComplete -= PlayWinningSound;
    }   
}
