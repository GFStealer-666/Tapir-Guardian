using System.Collections.Generic;
using UnityEngine;

[DisallowMultipleComponent]
public class PopupDirector : MonoBehaviour
{

    [SerializeField] private MonoBehaviour presenterBehaviour; // assign TMPPopupPresenter
    private IPopupPresenter presenter;

    [SerializeField] private List<PopupRequest> _queue = new(); // simple priority list
     [SerializeField]private bool _running;
    [SerializeField] private PopupRequest? _current;

    private void Awake()
    {

        presenter = presenterBehaviour as IPopupPresenter;
        if (presenter == null)
        {
            // try to auto-find a presenter in children to be forgiving
            presenter = GetComponentInChildren<IPopupPresenter>(true);
        }

        if (presenter == null)
        {
            Debug.LogError("[PopupDirector] No presenter assigned/found. Assign a TMPPopupPresenter on the canvas panel.");
        }
    }

    /// <summary>Add a popup. Higher priority interrupts current.</summary>
    public void Enqueue(PopupRequest req)
    {
        Debug.Log($"[PopupDirector] Enqueue: \"{req.Text}\" (Priority {req.Priority})");

        // Insert by priority (descending)
        int idx = _queue.FindIndex(r => r.Priority < req.Priority);
        if (idx < 0) _queue.Add(req); else _queue.Insert(idx, req);

        // Preempt if incoming has higher priority than current
        if (_current.HasValue && req.Priority > _current.Value.Priority)
        {
            Debug.Log("[PopupDirector] Preempting current popup.");
            StopAllCoroutines();
            _current = null;
            TryDequeueNext();              // immediately show the new one
            return;
        }
        if (!_running) TryDequeueNext();
    }

    public void ClearAll()
    {
        _queue.Clear();
        _current = null;
        _running = false;
        presenter?.Hide();
    }

    private void TryDequeueNext()
    {
        if (_queue.Count == 0)
        {
            _current = null;
            _running = false;
            presenter?.Hide();
            return;
        }

        _running = true;
        var next = _queue[0];
        _queue.RemoveAt(0);
        _current = next;

        presenter?.ShowNow(next);
        StartCoroutine(RunTimer(next.Duration));
    }

    private System.Collections.IEnumerator RunTimer(float seconds)
    {
        float t = 0f;
        while (t < seconds)
        {
            t += Time.deltaTime;
            yield return null;
        }
        TryDequeueNext();
    }
}
