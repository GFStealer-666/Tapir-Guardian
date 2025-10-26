using System;
using NUnit.Framework;
using UnityEngine;

[DisallowMultipleComponent]
[RequireComponent(typeof(IHealth))]
public class DamageReceiver : MonoBehaviour , IDamageable
{
    private IStat Istats;
    private IHealth Ihealth;
    private IBlock Iblock;

    void Awake()
    {
        Istats = GetComponent<IStat>();
        Ihealth = GetComponent<IHealth>();
        Iblock = GetComponent<IBlock>();

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
        Debug.Log($"DamageReceiver {this.gameObject} ReceiveDamage: {damage.RawDamage} type:{damage.Type}");
        int defense = Istats != null ? Istats.GetStatTypeOf(StatType.Defense) : 0;
        int dmgAfterArmor = DamageSystem.Resolve(damage.RawDamage, defense);

        float blockMultiplier = 1f;
        if (damage.CanBeBlocked && Iblock != null)
        {
            blockMultiplier = Iblock.BlockMultiplier;
        }

        int finalDamage = Mathf.RoundToInt(dmgAfterArmor * blockMultiplier);

        Ihealth.TakeDamage(finalDamage);
    }
}
