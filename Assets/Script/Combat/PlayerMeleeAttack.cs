using UnityEngine;

[DisallowMultipleComponent]
public class PlayerMeleeAttack : MonoBehaviour
{
    [SerializeField] private Transform origin;     // hand/torso
    [SerializeField] private Vector2 localOffset;
    [SerializeField] private float radius = 1.2f;
    [SerializeField] private LayerMask hitMask;

    private float nextAt;

    public bool TryMelee(float facingX, int damage, float cooldown, GameObject source)
    {
        if (Time.time < nextAt) return false;

        Vector2 o = GetOrigin(transform);
        Vector2 center = o + new Vector2(Mathf.Sign(facingX) * radius * 0.5f, 0f);
        var cols = Physics2D.OverlapCircleAll(center, radius, hitMask);

        foreach (var c in cols)
        {
            if (c && c.TryGetComponent<IDamageable>(out var d))
            {
                var data = new DamageData(
                    rawDamage: damage,
                    type: DamageType.Melee,
                    source: source ? source : gameObject,
                    canBeBlocked: true
                );
                d.ReceiveDamage(in data);
            }
        }

        nextAt = Time.time + cooldown;
        return cols.Length > 0;
    }

    private Vector2 GetOrigin(Transform self)
    {
        var t = origin ? origin : self;
        return (Vector2)t.TransformPoint((Vector3)localOffset);
    }

#if UNITY_EDITOR
    private void OnDrawGizmosSelected()
    {
        Vector2 o = GetOrigin(transform);
        Vector2 center = o + new Vector2(radius * 0.5f, 0f);
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(center, radius);
    }
#endif
}
