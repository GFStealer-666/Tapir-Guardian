// Assets/Scripts/World/AnimalInteractable.cs
using UnityEngine;
using UnityEngine.Events;
using System;

[DisallowMultipleComponent]
public class AnimalInteractable : BaseInteractable
{
    [Header("Core")]
    [SerializeField] private AnimalDataUnlocker animalDataUnlocker;
    [SerializeField] private ConversationEmitter conversationEmitter;

    [Header("Collider / Use Once")]
    [SerializeField] private Collider2D interactionCollider;
    [SerializeField] private bool oneTimeUse = false;


    [Header("Broadcast (UnityEvents)")]
    public UnityEvent OnInteracted;
    public UnityEvent OnSequenceFinished;

    /// <summary>
    /// Global broadcast for systems that prefer C# events.
    /// Payload = the unlocker that was used (so listeners can read which animal).
    /// </summary>
    public static event Action<AnimalDataUnlocker> OnAnimalFreedGlobal;

    private PlayerBrain _cachedPlayer;

    void Awake()
    {
        if (!conversationEmitter) conversationEmitter = GetComponentInChildren<ConversationEmitter>(true);
        if (!interactionCollider) interactionCollider = GetComponent<Collider2D>();
        // visuals are optional; leave them null if not used
    }

    public override bool CanInteract() => true;

    public override string GetPrompt()
    {
        string animalName = animalDataUnlocker && animalDataUnlocker.animalToUnlock
            ? animalDataUnlocker.animalToUnlock.name
            : "สัตว์";
        return $"กด E เพื่อปล่อย {animalName}";
    }

    public override void Interact(PlayerBrain player)
    {
        _cachedPlayer = player;

        // BaseInteractable hook (if you use it)
        RaiseEvents(player);

        // ---- Broadcast (before doing anything else if you want listeners to react immediately)
        OnInteracted?.Invoke();
        OnAnimalFreedGlobal?.Invoke(animalDataUnlocker);

        // ---- Unlock data
        if (animalDataUnlocker) animalDataUnlocker.UnlockNow();

        // ---- Start conversation (optional)
        float duration = 0f;
        if (conversationEmitter)
        {
            conversationEmitter.StartConversation();
            duration = Mathf.Max(0f, conversationEmitter.GetConversationDuration());
        }

        // ---- Finish after the dialogue (or immediately if none)
        if (duration > 0f) Invoke(nameof(FinishSequence), duration);
        else FinishSequence();

        // ---- One-time use?
        if (oneTimeUse)
        {
            enabled = false;
            if (interactionCollider) interactionCollider.enabled = false;
        }
    }

    private void FinishSequence()
    {
        OnSequenceFinished?.Invoke();
        // place any additional end-of-sequence logic here
    }
}
