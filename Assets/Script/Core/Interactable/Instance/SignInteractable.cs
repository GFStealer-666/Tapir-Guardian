using UnityEngine;
using TMPro;

public class SignInteractable : BaseInteractable
{
    [Header("Sign Content")]
    [SerializeField] private string title = "Sign";
    [TextArea(2, 6)] [SerializeField] private string message;

    [Header("UI (assign your panel + texts)")]
    [SerializeField] private GameObject signPanel;   // inactive by default

    [Header("Options")]
    [SerializeField] private bool pauseTime = false; // if you want Time.timeScale = 0 while reading

    private PlayerBrain _player;

    public override bool CanInteract() => enabled && gameObject.activeInHierarchy;

    public override string GetPrompt() => "กด E เพื่ออ่านป้าย";

    public override void Interact(PlayerBrain player)
    {
        if (!signPanel) { Debug.LogWarning($"[Sign] Panel not set on {name}"); return; }

        _player = player;
        _player.SetInputBlocked(true);
        if (pauseTime) Time.timeScale = 0f;

        signPanel.SetActive(true);

        // optional: fire UnityEvent/C# event hooks from BaseInteractable
        RaiseEvents(player);
    }

    public void CloseSign()
    {
        if (signPanel) signPanel.SetActive(false);
        if (_player)   _player.SetInputBlocked(false);
        if (pauseTime) Time.timeScale = 1f;
        _player = null;
    }

    public void SetContent(string newTitle, string newMessage)
    {
        title = newTitle;
        message = newMessage;
        // If panel is open, refresh UI

    }
}
