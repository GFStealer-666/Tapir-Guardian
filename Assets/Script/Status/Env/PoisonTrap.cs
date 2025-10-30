using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class PoisonTrap : MonoBehaviour
{
    public StatusSO poisonStatus;
    public float destroyDelay = 0.05f; // tiny delay for SFX/VFX if any

    void Reset()
    {
        var col = GetComponent<Collider2D>();
        col.isTrigger = true;
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        var status = other.GetComponentInChildren<StatusComponent>();
        if (status && poisonStatus)
        {
            status.Apply(poisonStatus);
        }
    }
}
