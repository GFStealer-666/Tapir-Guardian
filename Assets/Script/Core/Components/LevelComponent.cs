using UnityEngine;
using System;

[DisallowMultipleComponent]
public class LevelComponent : MonoBehaviour, ILevel
{
    [Tooltip("Player Level")]
    [SerializeField] private int level = 1;
    [SerializeField] private int currentExp = 0;
    [Tooltip("Level Scaling")]
    [SerializeField] private int baseExp = 50;
    [SerializeField] private int perLevelExp = 25;
    public int Level => level;
    public int CurrentExp => currentExp;
    public int ExpToNext => RequiredExpFor(level);
    public event Action<int> OnLevelChanged;

    public void AddExp(int amount)
    {
        currentExp += Mathf.Max(0, amount); // check if it not negative number
        while (currentExp >= ExpToNext)
        {
            currentExp -= ExpToNext; // if the exp is more than this xp level value leftover transfer to next level
            level += 1;
            OnLevelChanged?.Invoke(level);
        }
    }

    // formula = base XP of everylevel + level * perLevelExp 
    // mostlikely this could scale base on level like 25 50 75 
    private int RequiredExpFor(int level) => Mathf.Max(1, baseExp + (perLevelExp * (level - 1)));

}
