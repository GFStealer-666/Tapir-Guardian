using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class InteractionScanner : MonoBehaviour
{
    [SerializeField] private string interactableTag = "Interactable";
    [SerializeField] private InteractPromptAnnouncer promptAnnouncer; // optional popup
    private readonly HashSet<IInteractable> _inRange = new();

    private IInteractable _current;

    public IInteractable Current => _current;

    private void Awake()
    {
        var col = GetComponent<Collider2D>();
        col.isTrigger = true;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        // Detect interactable by tag or component
        if (!other.CompareTag(interactableTag)) return;

        var interactable = other.GetComponent<IInteractable>() 
                        ?? other.GetComponentInParent<IInteractable>() 
                        ?? other.GetComponentInChildren<IInteractable>();
        if (interactable == null) return;

        _inRange.Add(interactable);
        UpdateCurrent();
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (!other.CompareTag(interactableTag)) return;

        var interactable = other.GetComponent<IInteractable>() 
                        ?? other.GetComponentInParent<IInteractable>() 
                        ?? other.GetComponentInChildren<IInteractable>();
        if (interactable == null) return;

        _inRange.Remove(interactable);
        UpdateCurrent();
    }

    private void UpdateCurrent()
    {
        // Pick the first valid target (you can extend to “closest” later)
        _current = null;

        foreach (var i in _inRange)
        {
            if (i != null && i.CanInteract())
            {
                _current = i;
                break;
            }
        }

        // Notify popup UI
        promptAnnouncer?.ShowPromptFor(_current);
    }
}
