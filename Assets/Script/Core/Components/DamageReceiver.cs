using System;
using UnityEngine;

[DisallowMultipleComponent]
[RequireComponent(typeof(IHealth))]
public class DamageReceiver : MonoBehaviour, IDamageable
{
    private IStat   Istats;
    private IHealth Ihealth;

    [SerializeField] private BlockComponent Iblock;   // If absent => this entity cannot block

    [Header("Armor / Immunity")]
    [Tooltip("Any DamageType in this list will be completely ignored (0 damage).")]
    [SerializeField] private DamageType[] immuneTo;

    /// <summary>false = normal block, true = perfect block</summary>
    public event Action<bool> OnBlocked;
    public event Action<DamageType> OnImmune;

    /// <summary>Convenience: this entity can block only if a BlockComponent exists.</summary>
    public bool CanBlock => Iblock != null;

    void Awake()
    {
        Istats  = GetComponent<IStat>();
        Ihealth = GetComponent<IHealth>();
        if (!Iblock) Iblock = GetComponent<BlockComponent>();  // will stay null if none is attached

        if (Ihealth == null)
            Debug.LogError("Health component missing, core component required", this);
    }

    public void ReceiveHit(int raw)
    {
        ReceiveDamage(new DamageData(raw)); // defaults to Melee in your struct
    }

    public void ReceiveDamage(in DamageData damage)
    {
        Debug.Log($"ReceiveDamage: {damage.RawDamage} type:{damage.Type}");

        // --- IMMUNITY GATE ---
        if (IsImmuneTo(damage.Type))
        {
            OnImmune?.Invoke(damage.Type);
            Debug.Log($"<color=grey>Immune to {damage.Type} — no damage taken.</color>");
            return;
        }

        // --- ARMOR / DEFENSE ---
        int defense = Istats != null ? Istats.GetStatTypeOf(StatType.Defense) : 0;
        int dmgAfterArmor = DamageSystem.Resolve(damage.RawDamage, defense);

        // --- BLOCKING ---
        // Important: If there's NO BlockComponent, this entity CANNOT block at all,
        // even if the incoming damage is "blockable".
        bool didBlock        = false;
        bool didPerfectBlock = false;

        if (CanBlock && damage.CanBeBlocked)
        {
            didBlock        = Iblock.IsBlocking;
            didPerfectBlock = didBlock && Iblock.IsPerfectBlocking;
        }

        float blockMultiplier = 1f;
        if (didBlock)
        {
            blockMultiplier = Iblock.BlockMultiplier; // e.g., 0.5 for normal, maybe lower for perfect if your component handles it
        }
        int finalDamage = Mathf.Max(0, Mathf.RoundToInt(dmgAfterArmor * blockMultiplier));
        Ihealth.TakeDamage(damage);

        if (didBlock)
        {
            OnBlocked?.Invoke(false);
            if (didPerfectBlock)
            {
                OnBlocked?.Invoke(true);
                Debug.Log("<color=yellow>Perfect Block!</color>");
            }
            else
            {
                Debug.Log("<color=cyan>Normal Block!</color>");
            }
        }
        else
        {
            if (finalDamage <= 0)
            {
                Debug.Log("<color=grey>Negated by mitigation — 0 damage.</color>");
            }
            else
            {
                Debug.Log("<color=red>Direct Hit!</color>");
            }
        }
    }

    // ===== Helpers =====
    private bool IsImmuneTo(DamageType t)
    {
        if (immuneTo == null || immuneTo.Length == 0) return false;
        for (int i = 0; i < immuneTo.Length; i++)
            if (immuneTo[i] == t) return true;
        return false;
    }
}
