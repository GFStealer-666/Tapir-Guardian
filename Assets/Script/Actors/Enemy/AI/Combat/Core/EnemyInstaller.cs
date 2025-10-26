// Assets/Scripts/AI/EnemyInstaller.cs
using UnityEngine;

[DisallowMultipleComponent]
public class EnemyInstaller : MonoBehaviour
{
    [SerializeField] private EnemyRoot root;      // auto if null
    [SerializeField] private EnemyConfigSO config;  // assign your ScriptableObject
    [SerializeField] private bool fillHealthOnApply = true;

    void Awake()
    {
        Apply();
    }

    public void Apply()
    {
        root ??= GetComponent<EnemyRoot>();
        if (!root || !config) return;

        // ---- Health from config ----
        // Preferred: your Health implements a tiny "configuration" interface
        if (root.Ihealth is IConfigurableHealth cfgHealth)
        {
            cfgHealth.SetMax(config.MaxHealth, fillHealthOnApply);
        }
        else
        {
            // Fallback if you don't want a new interface:
            // try to touch the concrete HealthComponent only if present.
            var hc = root.GetComponentInChildren<HealthComponent>();
            if (hc != null)
            {
                hc.SetMax(config.MaxHealth, fillHealthOnApply); // add this method in HealthComponent
            }
            else
            {
                Debug.LogWarning("EnemyInstaller: no way to set max health found.", this);
            }
        }

        var speedStack = GetComponentInChildren<SpeedModifierStack>() ?? GetComponent<SpeedModifierStack>();
        if (speedStack != null)
        {
            speedStack.SetBase(config.MoveSpeed);   // add SetBase(float) to your stack
        }

        // ---- Optional: push attack params into whichever behaviour is present ----
        var melee = GetComponentInChildren<MeleeAttackBehaviour>();
        if (melee) melee.ApplyConfig(config.Attack, config.MeleeRange, config.MeleeCooldown, config.TargetMask);

        var ranged = GetComponentInChildren<RangedAttackBehaviour>();
        if (ranged) ranged.ApplyConfig(config.Attack, config.FireRange, config.BulletSpeed, config.FireCooldown, config.TargetMask);
    }
}
