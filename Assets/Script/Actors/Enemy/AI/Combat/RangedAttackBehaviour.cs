using UnityEngine;

[DisallowMultipleComponent]
public class RangedAttackBehaviour : MonoBehaviour, IEnemyAttack, IAttackRangeProvider
{
    [Header("Ranged")]
    [SerializeField] private Bullet bulletPrefab;
    [SerializeField] private int attack = 8;
    [SerializeField] private float fireRange = 7.5f;
    [SerializeField] private float bulletSpeed = 12f;
    [SerializeField] private float fireCooldown = 0.6f;
    [SerializeField] private LayerMask targetMask;

    private float nextAt;
    public float AttackRange => fireRange;
    public void ApplyConfig(int attack, float fireRange, float bulletSpeed, float fireCooldown, LayerMask hitMask)
    {
        this.attack = attack;
        this.fireRange = fireRange;
        this.bulletSpeed = bulletSpeed;
        this.fireCooldown = fireCooldown;
        this.targetMask = hitMask;
    }
    public bool TryAttack(Transform self, Transform target)
    {
        if (!target || !bulletPrefab) return false;
        if (Time.time < nextAt) return false;
        if (Vector2.Distance(self.position, target.position) > fireRange) return false;

        Vector2 dir = (target.position - self.position);
        if (dir.sqrMagnitude < 0.0001f) return false;
        dir.Normalize();

        var b = Instantiate(bulletPrefab, self.position, Quaternion.identity);
        b.Configure(attack, dir * bulletSpeed, gameObject, 3.0f, targetMask);   // bullet will create DamageData
        nextAt = Time.time + fireCooldown;
        return true;
    }
}
