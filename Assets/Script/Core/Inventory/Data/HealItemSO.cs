using UnityEngine;

[CreateAssetMenu(fileName = "NewBandageGradual", menuName = "Game/Items/Bandage (Gradual Heal)")]
public class HealItemSO : ConsumableItemSO
{
    [Header("Heal Over Time")]
    [Tooltip("Total HP restored over the full duration.")]
    public int totalHeal = 30;

    [Tooltip("Seconds the heal should take.")]
    public float duration = 3f;

    [Tooltip("Healing ticks per second (more = smoother).")]
    public int ticksPerSecond = 5;

    [Tooltip("If false, a new bandage replaces the previous one on the same target.")]
    public bool stackWithExisting = true;

    public override bool Use(GameObject target)
    {
        if (!target) return false;
        Debug.Log(target);
        // If you also want an instant top-up before HoT, keep healAmount in base and apply here if > 0:
        if (healAmount > 0)
        {
            var health = target.GetComponentInChildren<HealthComponent>();
            Debug.Log(health);
            if (health && !health.IsDead) health.Heal(healAmount);
        }

        // Start gradual healing
        var runner = HealOverTimeRunner.StartOn(target, Mathf.Max(0, totalHeal), Mathf.Max(0.01f, duration),
                                                Mathf.Max(1, ticksPerSecond), stackWithExisting, "Bandage");
        return runner != null;
    }

    // keep base item rules (stackable, icon/name from base, etc.)
    private new void OnValidate() => base.OnValidate();
}
