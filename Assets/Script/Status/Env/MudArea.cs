using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class MudArea : MonoBehaviour
{
    public StatusSO slowStatus;           // e.g., id: "slow.mud", magnitude 0.4, duration 2
    public float reapplyInterval = 0.5f;  // keeps it fresh while inside

    private float _timer;

    void OnTriggerStay2D(Collider2D other)
    {
        _timer -= Time.deltaTime;
        if (_timer > 0f) return;
        Debug.Log($"MudArea applying slow to {other.name}");
        var status = other.GetComponentInChildren<StatusComponent>();
        if (status && slowStatus) status.Apply(slowStatus);

        _timer = reapplyInterval;
    }
}
