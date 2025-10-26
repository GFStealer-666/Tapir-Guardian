using UnityEngine;

public enum StatusKind { Poison, Slow, Custom }
public enum StackPolicy { RefreshDuration, StackIntensity, IgnoreIfPresent }

[CreateAssetMenu(fileName = "NewStatus", menuName = "Game/Status")]
public class StatusSO : ScriptableObject
{
    [Header("Identity")]
    public string id;                 // e.g., "poison.basic", "slow.mud"
    public string displayName;
    public Sprite icon;
    public StatusKind kind = StatusKind.Custom;
    [Tooltip("Optional: group tag to allow items to dispel categories (e.g., 'Poison', 'Debuff')")]
    public string[] tags;

    [Header("Timing")]
    public float duration = 10f;      // seconds per application
    public float tickInterval = 1f;   // damage/logic tick; 0 = no periodic tick

    [Header("Effect Magnitude")]
    [Tooltip("Poison: damage per tick. Slow: 0..1 (e.g., 0.4 means 40% slow). Custom: free use.")]
    public float magnitude = 1f;

    [Header("Stacking")]
    public StackPolicy stackPolicy = StackPolicy.RefreshDuration;
    public int maxStacks = 1;         // used if StackIntensity

    [Header("Rules")]
    public bool dispellable = true;
    public bool showOnStatusBar = true;
}
