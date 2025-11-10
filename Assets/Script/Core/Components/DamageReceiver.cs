using System;
using NUnit.Framework;
using UnityEngine;

[DisallowMultipleComponent]
[RequireComponent(typeof(IHealth))]
public class DamageReceiver : MonoBehaviour , IDamageable
{
    private IStat Istats;
    private IHealth Ihealth;
    [SerializeField] private BlockComponent Iblock;
    public event Action<bool> OnBlocked;
    void Awake() 
    {
        Istats = GetComponent<IStat>();
        Ihealth = GetComponent<IHealth>();
        if(!Iblock) Iblock = GetComponent<BlockComponent>();

        if (Ihealth == null)
        {
            Debug.LogError("Health component missing, core component required");
        }
    }

    public void ReceiveHit(int raw)
    {
        ReceiveDamage(new DamageData(raw));
    }


    // Basically it will hit 
    public void ReceiveDamage(in DamageData damage)
    {
        Debug.Log($"ReceiveDamage: {damage.RawDamage} type:{damage.Type}");
        int defense = Istats != null ? Istats.GetStatTypeOf(StatType.Defense) : 0;
        int dmgAfterArmor = DamageSystem.Resolve(damage.RawDamage, defense);

        // --- Detect blocking ---
        bool didBlock = damage.CanBeBlocked && Iblock != null && Iblock.IsBlocking;
        bool didPerfectBlock = didBlock && Iblock.IsPerfectBlocking;

        float blockMultiplier = 1f;
        if (didBlock)
        {
            blockMultiplier = Iblock.BlockMultiplier;
        }

        int finalDamage = Mathf.RoundToInt(dmgAfterArmor * blockMultiplier);
        Ihealth.TakeDamage(finalDamage);

        // --- Notify visual/audio systems ---
        if (didBlock)
        {
            OnBlocked?.Invoke(false);
            if (didPerfectBlock)
            {
                OnBlocked?.Invoke(true);
                Debug.Log("<color=yellow>Perfect Block!</color>");
            }
            else
                Debug.Log("<color=cyan>Normal Block!</color>");
        }
        else
        {
            Debug.Log("<color=red>Direct Hit!</color>");
        }
    }

}
