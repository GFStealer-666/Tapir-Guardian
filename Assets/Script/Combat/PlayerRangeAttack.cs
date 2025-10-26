using UnityEngine;

[DisallowMultipleComponent]
public class PlayerRangeAttack : MonoBehaviour
{
    [SerializeField] private Transform muzzle;
    [SerializeField] private Vector2 localOffset;
    [SerializeField] private LayerMask hitMask;
    [SerializeField] private Bullet bulletPrefab;

    private float nextAt;

    public bool TryFireForward(float facingX, int damage, float bulletSpeed, float fireCooldown, GameObject source)
    {
        if (!bulletPrefab) return false;
        if (Time.time < nextAt) return false;

        Vector2 origin = GetOrigin(transform);
        Vector2 dir = (facingX >= 0f) ? Vector2.right : Vector2.left;

        var b = Instantiate(bulletPrefab, origin, Quaternion.identity);
        b.Configure(damage, dir * bulletSpeed, source, -1f, hitMask);

        nextAt = Time.time + fireCooldown;
        return true;
    }

    private Vector2 GetOrigin(Transform self)
    {
        var t = muzzle ? muzzle : self;
        return (Vector2)t.TransformPoint((Vector3)localOffset);
    }
}
