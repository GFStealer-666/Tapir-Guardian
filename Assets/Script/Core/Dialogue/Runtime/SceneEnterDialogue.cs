using UnityEngine;

public class SceneEnterDialogue : MonoBehaviour
{
    [SerializeField] private TextSequenceSO sequence;
    [SerializeField] private PopupDirector popupDirectorlocal;

    [Tooltip("Priority: use 0 for normal, >0 to override notifications.")]
    public int priority = 1;

    private void Start()
    {
        if (!sequence || sequence.lines.Count == 0) return;
        foreach (var line in sequence.lines)
        {
            var req = new PopupRequest(line.text, line.duration, sequence.category, priority);
            popupDirectorlocal?.Enqueue(req);
        }
    }
}
