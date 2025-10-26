using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Collider2D))]
public class AreaNotification : MonoBehaviour
{
    [SerializeField] private TextLineSO textLine;
    [SerializeField] private int priority = 2; // higher than dialogues to always pop
    [SerializeField] private string playerTag = "Player";
    [SerializeField] private bool oneTimeOnly = true;
    [SerializeField] private PopupDirector popupDirectorlocal;
    private bool hasTriggered = false;  
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag(playerTag)) return;
        if (oneTimeOnly && hasTriggered) return;
        hasTriggered = true;
        var text = $"{textLine.text}";
        var req = new PopupRequest(text, textLine.duration, PopupCategory.Notification, priority);
        popupDirectorlocal?.Enqueue(req);
    }
}
