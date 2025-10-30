using UnityEngine;

[DisallowMultipleComponent]
public class Bullet : MonoBehaviour
{
    [SerializeField] private Rigidbody2D rb;
    [SerializeField] private float maxLife = 4f;
    public SpriteRenderer spriteRenderer;
    private int _damage;
    private GameObject _source;
    private LayerMask _hitMask;
    private float _deathAt;

    public void Configure(int damage, Vector2 velocity, GameObject source, float customLife, LayerMask mask)
    {
        _damage = damage;
        _source = source;
        _hitMask = mask;

        if (!rb) rb = GetComponent<Rigidbody2D>();
        if (rb) rb.linearVelocity = velocity;

        // Make the sprite face travel direction
        if (spriteRenderer)
            spriteRenderer.flipX = velocity.x < 0f;
            spriteRenderer.flipY = velocity.x < 0f;

        // Or, if you prefer rotation instead of flip:

        _deathAt = Time.time + ((customLife > 0f) ? customLife : maxLife);
    }

    private void Update()
    {
        if (Time.time >= _deathAt)
            Destroy(gameObject);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {

        if (((1 << other.gameObject.layer) & _hitMask) == 0)
            return;

        if (other.GetComponentInChildren<DamageReceiver>() is DamageReceiver dmg )
        {
            Debug.Log($"Bullet hit {other.gameObject} for {_damage} damage");
            dmg.ReceiveDamage(bullet);
        }

        // bullet is consumed on first hit
        Destroy(gameObject);
    }

    public DamageData bullet
    {
        get
        {
            return new DamageData(
                rawDamage: _damage,
                type: DamageType.Ranged,
                source: _source,
                canBeBlocked: false
            );
        }
    }
}
