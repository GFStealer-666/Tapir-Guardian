using UnityEngine;
using System.Collections.Generic;
[RequireComponent(typeof(Collider2D))]
public class MudArea : MonoBehaviour
{
    [Header("Slow effect")]
    public StatusSO slowStatus;  // e.g. Mud slow
    public bool removeOnExit = true;

    private readonly HashSet<StatusComponent> _inside = new();

    void Reset()
    {
        var col = GetComponent<Collider2D>();
        col.isTrigger = true;
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        var status = other.GetComponentInChildren<StatusComponent>();
        if (status && slowStatus && !_inside.Contains(status))
        {
            status.Apply(slowStatus); // apply once
            _inside.Add(status);
            Debug.Log($"[MudArea] Applied slow to {other.name}");
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (!removeOnExit) return;

        var status = other.GetComponentInChildren<StatusComponent>();
        if (status && slowStatus && _inside.Contains(status))
        {
            status.RemoveById(slowStatus.id);
            _inside.Remove(status);
            Debug.Log($"[MudArea] Removed slow from {other.name}");
        }
    }

    void OnDisable()
    {
        // Clean up if mud is disabled while player is still inside
        foreach (var s in _inside)
        {
            if (s && slowStatus)
                s.RemoveById(slowStatus.id);
        }
        _inside.Clear();
    }
}
