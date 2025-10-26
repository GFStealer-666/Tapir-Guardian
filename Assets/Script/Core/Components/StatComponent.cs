using System.Collections.Generic;
using UnityEngine;

using Enum = System.Enum;

// Current Level + stat progressionSO
[DisallowMultipleComponent]
public class StatComponent : MonoBehaviour, IStat
{
    [SerializeField] private StatProgressionSO progression;
    [SerializeField] private ILevel Ilevel;
    [SerializeField] private int StatMultiplier = 1; // incase no level 
    private readonly Dictionary<StatType, int> cache = new();
    // immiediately after levelup it assigned the stat value in to dictionary
    private void Awake()
    {
        Ilevel = GetComponent<ILevel>();
        RecalculateStats();
        if (Ilevel != null)
        {
            // every time levelup calculate the stat
            Ilevel.OnLevelChanged += (int unusedLevel) => RecalculateStats();
        }
    }

    void OnDestroy()
    {
        if(Ilevel != null)
        Ilevel.OnLevelChanged -= (int unusedLevel) => RecalculateStats();
    }
    public int GetStatTypeOf(StatType type)
        => cache.TryGetValue(type, out var statValue) ? statValue : 0;
    // find the type and get the value out if found value = V , if not value = 0

    private void RecalculateStats()
    {
        cache.Clear(); // clear dictionary

        int _level = Ilevel != null ? Ilevel.Level : StatMultiplier;
        // Find the if whether the component contain level or not (incase some entity doesn't use level)
        // if not just set the value to 1 else use the current level from component

        foreach (StatType statType in Enum.GetValues(typeof(StatType)))
        {
            // creating dictionary key = stat , value = state value
            // using the progressionSO to calculate the stat base on level
            cache[statType] = progression != null ? progression.GetStatValue(statType, _level) : 0;
        }

        var hp = GetComponent<IHealth>();
        if (hp is HealthComponent healthComponent) // if is health component cast the value to health
        {
            healthComponent.SyncMaxStat(cache[StatType.Health]); 
            // assign the value of maxhealth
        }
    }
}
