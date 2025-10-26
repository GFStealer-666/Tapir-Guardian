using UnityEngine;

public static class DamageSystem
{
    public static int Resolve(int rawDamage, int defense)
    {
        if (rawDamage <= 0) return 0;
        float finalDamage = rawDamage * (100f / Mathf.Max(0, 100f - defense));
        return Mathf.Max(0, Mathf.RoundToInt(finalDamage));
    }
}
