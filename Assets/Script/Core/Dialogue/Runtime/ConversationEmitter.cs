using UnityEngine;

public class ConversationEmitter : MonoBehaviour
{
    [SerializeField] private int priority = 1;
    [SerializeField] private TextSequenceSO defaultSequence;
    [SerializeField] private PopupDirector popupDirectorlocal;
    public void StartConversation()
    {
        Debug.Log("Starting conversation emitter");
        if (!defaultSequence) return;
        foreach (var line in defaultSequence.lines)
        {
            var req = new PopupRequest(line.text, line.duration, defaultSequence.category, priority);
            popupDirectorlocal.Enqueue(req);
        }
    }
    public float GetConversationDuration()
    {
        if (!defaultSequence) return 0f;
        float totalDuration = 0f;
        foreach (var line in defaultSequence.lines)
        {
            totalDuration += line.duration;
        }
        return totalDuration;
    }
}
