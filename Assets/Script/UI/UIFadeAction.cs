using UnityEngine;

public class UIFadeAction : MonoBehaviour
{
    [SerializeField] private UIFader fader;
    [SerializeField] private GameObject objectToActivate;

    public void FadeAndActivate()
    {
        if (!fader) return;

        fader.FadeOutDoFadeIn(0.5f, 2f, () =>
        {
            if (objectToActivate)
                objectToActivate.SetActive(true);
        });
    }
}
