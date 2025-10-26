using UnityEngine;
using UnityEngine.Events;

public abstract class BaseInteractable : MonoBehaviour, IInteractable
{
    [Header("Interact Settings")]
    [SerializeField] private float maxRange = 0.4f;

    [Header("Broadcast")]
    public UnityEvent onInteracted;  
    // Hook in inspector -> play sound, trigger animation, etc.

    // C# event for code systems (quests, etc.)
    public event System.Action<PlayerBrain, BaseInteractable> InteractedCSharp;

    // ---- IInteractable core ----
    public abstract bool CanInteract();
    public abstract void Interact(PlayerBrain player); // we'll call RaiseEvents() inside child
    public abstract string GetPrompt();

    public Vector3 GetWorldPosition()
    {
        return transform.position;
    }

    public float MaxRange => maxRange;

    // ---- Registry handling ----
    protected virtual void OnEnable()
    {
        InteractableRegistry.Register(this);
        Debug.Log($"[Interactable] Registered {name}");
    }

    protected virtual void OnDisable()
    {
        InteractableRegistry.Unregister(this);
    }

    // Call this from your child class *after* doing its own logic
    protected void RaiseEvents(PlayerBrain player)
    {
        // UnityEvent (Inspector stuff)
        onInteracted?.Invoke();

        // C# event (code listeners)
        InteractedCSharp?.Invoke(player, this);
    }
}
