using System;
using UnityEngine;

[DisallowMultipleComponent]
public class HealthComponent : MonoBehaviour, IHealth,IConfigurableHealth
{
    [SerializeField] private int currentHealth = 0;
    [SerializeField] private int maxHealth = 0;
    public int CurrentHealth => currentHealth;
    public int MaxHealth => maxHealth;
    public event Action<int, int> OnHealthChanged;
    public event Action OnDied;
    public bool IsDead => currentHealth <= 0;
    [Header("Debug Only")]
    [SerializeField] private int damageHealth = 40;
    private void Awake()
    {
        if (CurrentHealth <= 0) currentHealth = maxHealth;
        OnHealthChanged?.Invoke(currentHealth, maxHealth);
    }
    public void SyncMaxStat(int newMax)
    {
        maxHealth = newMax;
        currentHealth = Mathf.Max(currentHealth, maxHealth); // Full health on level up
        OnHealthChanged?.Invoke(currentHealth, maxHealth);
    }
    public void TakeDamage(int amount)
    {
        if (amount <= 0 || currentHealth <= 0) return;
        currentHealth = Mathf.Clamp(currentHealth - amount, 0, maxHealth);
        OnHealthChanged?.Invoke(currentHealth, maxHealth);
        Debug.Log($"{gameObject.name} took {amount} damage. Current HP = {currentHealth}/{maxHealth}");
        if (currentHealth <= 0)
        {
            OnDied?.Invoke();
            Debug.Log($"{gameObject.name} has died.");
        }

    }
    public void Heal(int amount)
    {
        if (amount <= 0 || currentHealth <= 0) return;

        // clamping the value (value , min , max) the value cannot exceed maxhealth and not below 0
        currentHealth = Mathf.Clamp(currentHealth + amount, 0, maxHealth);
        OnHealthChanged?.Invoke(currentHealth, maxHealth);
    }
    public void SetMax(int max, bool fillCurrent)
    {
        maxHealth = Mathf.Max(1, max);
        if (fillCurrent || CurrentHealth > MaxHealth) currentHealth = MaxHealth;
        OnHealthChanged?.Invoke(CurrentHealth, MaxHealth);
    }
#region ContextMenu
    [ContextMenu("Debug: Take 10 Damage")]
    private void DebugDamage10()
    {
        TakeDamage(damageHealth);
        Debug.Log($"[HealthComponent] Took 10 damage. Current HP = {currentHealth}/{maxHealth}", this);
    }

    [ContextMenu("Debug: Heal 10")]
    private void DebugHeal10()
    {
        Heal(10);
        Debug.Log($"[HealthComponent] Healed 10. Current HP = {currentHealth}/{maxHealth}", this);
    }
#endregion
}
