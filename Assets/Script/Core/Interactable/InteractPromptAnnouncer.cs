using UnityEngine;

public class InteractPromptAnnouncer : MonoBehaviour
{
    [Header("Popup Target")]
    [SerializeField] private PopupDirector popupDirector;

    [Header("Popup Settings")]
    [SerializeField] private PopupCategory category = PopupCategory.Notification;
    [SerializeField] private int priority = 5;

    private IInteractable _lastShownFor;
    private string _lastPrompt;
    private bool _promptActive;

    public void ShowPromptFor(IInteractable interactable)
    {
        if (interactable == null || !interactable.CanInteract())
        {
            if (_promptActive)
            {
                popupDirector?.ClearAll(); // or make a specific HidePrompt() if you prefer
                _promptActive = false;
            }

            _lastShownFor = null;
            _lastPrompt = null;
            return;
        }

        var prompt = interactable.GetPrompt();
        if (string.IsNullOrWhiteSpace(prompt)) return;

        if (interactable == _lastShownFor && prompt == _lastPrompt) return;

        _lastShownFor = interactable;
        _lastPrompt = prompt;
        _promptActive = true;

        var req = new PopupRequest(prompt, 9999f, category, priority, true); // 👈 persistent
        popupDirector?.Enqueue(req);
    }

    public void ClearPrompt()
    {
        _lastShownFor = null;
        _lastPrompt = null;
        _promptActive = false;
        popupDirector?.ClearAll();
    }
}
