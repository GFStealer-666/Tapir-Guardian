using UnityEngine;

public class RandomPopupEmitter : MonoBehaviour
{
    [SerializeField] private TextSequenceSO pool; // category = Random
    [SerializeField] private int priority = 0;
    [SerializeField] private bool emitOnInterval = false;
    [SerializeField] private PopupDirector popupDirectorlocal;
    [SerializeField, Min(0.5f)] private float interval = 10f;


    private float timer;

    private void Update()
    {
        if (!emitOnInterval || !pool || pool.lines.Count == 0) return;
        timer += Time.deltaTime;
        if (timer >= interval)
        {
            timer = 0f;
            EmitRandom();
        }
    }

    [ContextMenu("Emit Random Now")]
    public void EmitRandom()
    {
        if (!pool || pool.lines.Count == 0) return;
        var line = pool.lines[Random.Range(0, pool.lines.Count)];
        var req = new PopupRequest(line.text, line.duration, pool.category, priority);
        popupDirectorlocal?.Enqueue(req);
    }
}
