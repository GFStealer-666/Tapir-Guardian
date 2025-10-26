// Assets/Scripts/AI/Interfaces/IEnemyAttack.cs
using UnityEngine;

public interface IEnemyAttack
{
    // Returns true if an attack was performed (cooldown is handled inside)
    bool TryAttack(Transform self, Transform target);
}

public interface IAttackRangeProvider
{
    float AttackRange { get; }
}
