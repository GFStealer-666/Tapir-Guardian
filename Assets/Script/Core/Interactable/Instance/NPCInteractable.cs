using UnityEngine;
using TMPro;

public class NPCInteractable : BaseInteractable
{
    [Header("NPC Info")]
    [SerializeField] private string npcName = "???";
    [SerializeField] private ConversationEmitter conversationEmitter;

    private int _lineIndex = 0;
    private PlayerBrain _cachedPlayer;
    
    public override bool CanInteract()
    {
        // Could add logic like "not in combat", "quest stage reached", etc.
        return true;
    }

    public override string GetPrompt()
    {
        return $"Press E to talk to {npcName}";
    }

    public override void Interact(PlayerBrain player)
    {
        Debug.Log($"Interacting with NPC: {npcName}");
        _cachedPlayer = player;
        player.SetInputBlocked(true);
        RaiseEvents(player);
        conversationEmitter.StartConversation();
        float duration = conversationEmitter.GetConversationDuration();
        Invoke(nameof(EndDialogue), duration);
    }

    // Hook this to UI "Close" button
    public void EndDialogue()
    {

        if (_cachedPlayer)
            _cachedPlayer.SetInputBlocked(false);
    }
}
