using UnityEngine;

public class JailInteractable : BaseInteractable
{
    [Header("Jail Info")]
    [SerializeField] private ConversationEmitter conversationEmitter;
    [SerializeField] private AnimalDataUnlocker animalDataUnlocker;
    [SerializeField] private Collider2D interactionCollider;
    [SerializeField] private GameObject jailOpen, jailClosed;
    [SerializeField] private bool oneTimePrompt = false;
    private PlayerBrain _cachedPlayer;
    void Awake()
    {
        conversationEmitter = GetComponentInChildren<ConversationEmitter>();
        interactionCollider = GetComponent<Collider2D>();
    }
    public override bool CanInteract()
    {
        // Could add logic like "not in combat", "quest stage reached", etc.
        return true;
    }

    public override string GetPrompt()
    {
        return $"กด E เพื่อปล่อย {animalDataUnlocker.animalToUnlock.name}";
    }

    public override void Interact(PlayerBrain player)
    {
        Debug.Log($"Interacting with Jail: {animalDataUnlocker.animalToUnlock.name}");

        _cachedPlayer = player;
        player.SetInputBlocked(true);
        RaiseEvents(player);
        conversationEmitter.StartConversation();
        float duration = conversationEmitter.GetConversationDuration();
        animalDataUnlocker.UnlockNow();

        jailOpen.SetActive(true);
        jailClosed.SetActive(false);

        Invoke(nameof(EndDialogue), duration);

        if (oneTimePrompt)
        {
            this.enabled = false;
            interactionCollider.enabled = false;
        }
    }

    public void EndDialogue()
    {

        if (_cachedPlayer)
            _cachedPlayer.SetInputBlocked(false);
    }
}
