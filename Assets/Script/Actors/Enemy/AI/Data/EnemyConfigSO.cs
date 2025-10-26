using UnityEngine;

public enum EnemyKind { Melee, Gun }

[CreateAssetMenu(menuName="Game/AI/Enemy Config")]
public class EnemyConfigSO : ScriptableObject
{
    [Header("Kind")]
    public EnemyKind kind = EnemyKind.Melee;

    [Header("Base Stats (no level scaling)")]
    public int MaxHealth = 60;
    public int Attack    = 12;
    public int Defense   = 4;
    public float MoveSpeed = 4.5f;

    [Header("Perception")]
    public float SightRadius = 8f;
    public float FieldOfView = 110f;   // degrees

    [Header("Patrol")]
    public float WaypointTolerance = 0.2f;
    public float WaitAtPoint = 0.5f;

    [Header("Chase")]
    public float ChaseStopDistance = 0.8f; // how close to stop before attacking
    public float LoseTargetAfter = 2.0f;   // lose if unseen for this many seconds

    [Header("Melee")]
    public float MeleeRange = 1.0f;
    public float MeleeCooldown = 0.8f;

    [Header("Ranged")]
    public float FireCooldown = 0.6f;
    public float BulletSpeed = 12f;
    public float BulletLife = 2.5f;
    public float FireRange = 7.5f;

    [Header("Layers")]
    public LayerMask ObstacleMask; // for line-of-sight ray
    public LayerMask TargetMask;   // e.g., Player layer
}
