using UnityEngine;

[DisallowMultipleComponent]
public class MeleeAttackBehaviour : MonoBehaviour,
    IEnemyAttack, IAttackRangeProvider, IAttackOriginProvider
{
    [Header("Origin (hand/weapon)")]
    [SerializeField] private Transform attackOrigin;  // assign a child like "Hand"
    [SerializeField] private Vector2 localOffset;     // fine-tune from that transform

    [Header("Melee")]
    [SerializeField] private int attack = 12;
    [SerializeField] private float range = 0.75f;
    [SerializeField] private float cooldown = 0.8f;
    [SerializeField] private LayerMask targetMask;

    private float nextAt;

    public float AttackRange => range;

    // ===== IAttackOriginProvider =====
    public Vector2 GetOrigin(Transform self)
    {
        // Use the attackOrigin if set, else the enemy transform.
        var t = attackOrigin ? attackOrigin : self;

        // Convert a local-space offset to world-space using Unity's math.
        // If localOffset is (0,0), this returns t.position.
        return (Vector2)t.TransformPoint((Vector3)localOffset);
    }

    // ===== IEnemyAttack =====
    public bool TryAttack(Transform self, Transform target)
    {
        if (!target || Time.time < nextAt) return false;

        Vector2 origin = GetOrigin(self);

        Vector2 hitPoint = target.TryGetComponent<Collider2D>(out var col)
            ? col.ClosestPoint(origin)
            : (Vector2)target.position;

        if (Vector2.Distance(origin, hitPoint) > range) return false;

        // Optional layer check
        int layer = target.gameObject.layer;
        if ((targetMask.value & (1 << layer)) == 0) return false;

        // Find IDamageable anywhere on the target hierarchy
        IDamageable d =
            target.GetComponent<IDamageable>() ??
            target.GetComponentInChildren<IDamageable>(true) ??
            target.GetComponentInParent<IDamageable>();

        if (d != null)
        {
            var data = new DamageData(attack, DamageType.Melee, gameObject, true);
            d.ReceiveDamage(in data);
            Debug.Log("Attacking");
            nextAt = Time.time + cooldown;
            return true;
        }
        return false;
    }

#if UNITY_EDITOR
    void OnDrawGizmosSelected()
    {
        var self = transform;
        Vector2 origin = GetOrigin(self);
        Gizmos.color = new Color(1f, 0.3f, 0.2f, 0.35f);
        DrawCircle(origin, range, 32);
        Gizmos.color = new Color(1f, 0.3f, 0.2f, 0.9f);
        Gizmos.DrawLine(origin, origin + (Vector2)self.right * range);
    }

    private static void DrawCircle(Vector3 c, float r, int seg)
    {
        Vector3 prev = c + new Vector3(r, 0f, 0f);
        for (int i = 1; i <= seg; i++)
        {
            float a = i * Mathf.PI * 2f / seg;
            Vector3 next = c + new Vector3(Mathf.Cos(a) * r, Mathf.Sin(a) * r, 0f);
            Gizmos.DrawLine(prev, next);
            prev = next;
        }
    }
#endif

    // Optional: used by EnemyInstaller
    public void ApplyConfig(int atk, float rng, float cd, LayerMask mask)
    { attack = atk; range = rng; cooldown = cd; targetMask = mask; }
}
