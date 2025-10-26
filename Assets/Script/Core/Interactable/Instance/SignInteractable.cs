using UnityEngine;
using TMPro;

public class SignInteractable : BaseInteractable
{
    [Header("Sign Content")]
    [TextArea(2, 5)] [SerializeField] private string message;
    [SerializeField] private string title = "Sign";

    [Header("UI")]
    [SerializeField] private GameObject signPanel;
    [SerializeField] private TMP_Text titleText;
    [SerializeField] private TMP_Text bodyText;

    private PlayerBrain _cachedPlayer;

    public override bool CanInteract() => true;

    public override string GetPrompt() => "Press E to read sign";

    public override void Interact(PlayerBrain player)
    {
        _cachedPlayer = player;

        // Freeze controls so player can’t move
        player.SetInputBlocked(true);

        // Show UI
        if (signPanel)
        {
            signPanel.SetActive(true);
            if (titleText) titleText.text = title;
            if (bodyText)  bodyText.text  = message;
        }
        RaiseEvents(player);
    }

    // Called by Close button on your sign panel UI
    public void CloseSign()
    {
        if (signPanel) signPanel.SetActive(false);

        if (_cachedPlayer)
            _cachedPlayer.SetInputBlocked(false);
    }
}
