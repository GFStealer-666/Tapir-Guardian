using UnityEngine;

public enum DamageType {Melee, Ranged, NonLethal}

public struct DamageData
{
    public int RawDamage;
    public DamageType Type;
    public GameObject Source;
    public bool CanBeBlocked;


    // Default melee damage
    public DamageData(int rawDamage, DamageType type = DamageType.Melee
    , GameObject source = null, bool canBeBlocked = true)
    {
        RawDamage = rawDamage; Type = type; Source = source; CanBeBlocked = canBeBlocked;
    }
}
