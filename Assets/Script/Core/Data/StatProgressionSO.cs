using System.Collections.Generic;
using UnityEngine;

// caculate the stat inside here
[CreateAssetMenu(menuName = "Game/Stats/Stat Progression")]
public class StatProgressionSO : ScriptableObject
{
    [System.Serializable]
    public struct StatRow
    {
        public StatType Type;
        public int BaseValue;
        public float PerLevel;
    }
    [Tooltip("Per-entity base/growth for each stat")]
    public List<StatRow> Stats = new List<StatRow>();
    public int GetStatValue(StatType type, int level)
    {
        foreach (var stat in Stats)
        {
            if (type == stat.Type)
            {
                return Mathf.Max(0, Mathf.RoundToInt(stat.BaseValue + (stat.PerLevel * (level - 1))));
                // the formula is base value of level 1 +  (current level * stat per level)
            }
        }
        return 0;
    }
}
