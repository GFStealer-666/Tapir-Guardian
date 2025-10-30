using UnityEngine;

public class PoisonTrapInteractable : BaseInteractable
{
    [Header("NPC Info")]
    [SerializeField] private string objectName = "หนามพิษ";
    [SerializeField] private Collider2D interactionCollider;
    [SerializeField] private PoisonTrap poisonTrap;
    [SerializeField] private bool oneTimeDialogue = false;
    private int _lineIndex = 0;
    private PlayerBrain _cachedPlayer;
    void Awake()
    {
        interactionCollider = GetComponent<Collider2D>();
    }
    public override bool CanInteract()
    {
        // Could add logic like "not in combat", "quest stage reached", etc.
        return true;
    }

    public override string GetPrompt()
    {
        return $"กด E เพื่อปลด{objectName}";
    }

    public override void Interact(PlayerBrain player)
    {
        Debug.Log($"Interacting with Object: {objectName}");
        _cachedPlayer = player;
        RaiseEvents(player);
        if (oneTimeDialogue)
        {
            this.enabled = false;
            interactionCollider.enabled = false;
        }
    }

}
