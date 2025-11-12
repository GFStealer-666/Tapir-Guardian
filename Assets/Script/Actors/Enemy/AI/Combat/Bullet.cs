using UnityEngine;

[DisallowMultipleComponent]
public class Bullet : MonoBehaviour
{
    [SerializeField] private Rigidbody2D rb;
    [SerializeField] private Collider2D  col;
    [SerializeField] private float maxLife = 4f;
    public SpriteRenderer spriteRenderer;

    private int         _damage;
    private DamageType  _damageType = DamageType.Ranged;
    private GameObject  _source;
    [SerializeField] private LayerMask _hitMask = ~0;
    private float       _deathAt;
    private bool        _consumed;

    public void Configure(int damage, Vector2 velocity, GameObject source, float customLife, LayerMask mask, DamageType type)
    {
        _damage     = damage;
        _source     = source;
        _hitMask    = mask;
        _damageType = type;

        if (!rb)  rb  = GetComponent<Rigidbody2D>();
        if (!col) col = GetComponent<Collider2D>();

        if (rb) rb.linearVelocity = velocity; // use velocity (reliable across versions)

        // Make the sprite face travel direction
        if (spriteRenderer)
        {
            bool left = velocity.x < 0f;
            spriteRenderer.flipX = left;
            spriteRenderer.flipY = left; // if you want mirrored vertically too
        }
        // Or rotate instead of flip:
        // transform.right = velocity.normalized;

        _deathAt = Time.time + ((customLife > 0f) ? customLife : maxLife);
    }

    private void Update()
    {
        if (Time.time >= _deathAt) Destroy(gameObject);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (_consumed) return;

        // Layer mask filter
        if (((1 << other.gameObject.layer) & _hitMask) == 0) return;

        // Ignore owner/self (compare roots to be safe)
        if (_source && other.transform.root == _source.transform.root) return;

        // Apply damage
        var damageable = other.GetComponentInChildren<IDamageable>();
        if (damageable != null)
        {
            var data = new DamageData(
                rawDamage: _damage,
                type: _damageType,
                source: _source,
                canBeBlocked: false // set false if you want bullets to be unblockable
            );

            damageable.ReceiveDamage(in data);
        }

        // Consume bullet
        _consumed = true;
        if (col) col.enabled = false;
        Destroy(gameObject);
    }
}
