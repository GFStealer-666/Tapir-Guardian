using UnityEngine;

public class InteractPromptAnnouncer : MonoBehaviour
{
    [Header("Popup Target")]
    [SerializeField] private PopupDirector popupDirector;

    [Header("Popup Settings")]
    [SerializeField] private PopupCategory category = PopupCategory.Notification; // whatever you use in PopupRequest
    [SerializeField] private int priority = 5;
    [SerializeField] private float defaultDuration = 2f;

    // cache so we don't spam enqueue with the same message every frame
    private IInteractable _lastShownFor;
    private string _lastPrompt;

    // Called by scanner when closest interactable changes
    public void ShowPromptFor(IInteractable interactable)
    {
        if (interactable == null)
        {
            // target gone: clear state and optionally show nothing
            _lastShownFor = null;
            _lastPrompt = null;
            return;
        }

        if (!interactable.CanInteract()) return;

        var prompt = interactable.GetPrompt();
        if (string.IsNullOrWhiteSpace(prompt)) return;

        // only enqueue if it's different from last seen
        if (interactable == _lastShownFor && prompt == _lastPrompt) return;

        _lastShownFor = interactable;
        _lastPrompt = prompt;

        var req = new PopupRequest(prompt, defaultDuration, category, priority);
        popupDirector?.Enqueue(req);
    }

    // Optional helper if you ever want to force-hide externally
    public void ClearPrompt()
    {
        _lastShownFor = null;
        _lastPrompt = null;
    }
}
