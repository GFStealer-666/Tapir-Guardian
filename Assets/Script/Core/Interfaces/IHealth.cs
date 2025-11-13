using System;

public interface IHealth
{
    int CurrentHealth { get; }
    int MaxHealth { get; }

    void TakeDamage(int dmg);
    void TakeDamage(in DamageData data);
    void ApplyDamage(int dmg, DamageType type);
    void Heal(int heal);

    static event Action<int, int> OnHealthChanged;
    static event Action OnDied;
    static bool IsDead { get; }
}

public interface IConfigurableHealth
{
    void SetMax(int max, bool fillCurrent);
}