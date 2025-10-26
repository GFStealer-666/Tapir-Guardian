using UnityEngine;

public class PlayerDialogueTrigger : MonoBehaviour
{
    [SerializeField] private TextSequenceSO textLine;
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
        foreach (var line in textLine.lines)
        {
            var req = new PopupRequest(line.text, line.duration, PopupCategory.PlayerDialogue, priority);
            popupDirectorlocal?.Enqueue(req);
        }
    }
}
