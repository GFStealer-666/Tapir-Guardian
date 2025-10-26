using System.Collections.Generic;
using UnityEngine;

public class SpeedModifierStack : MonoBehaviour, IMoveSpeedProvider
{
    private struct SpeedEntry
    {
        public float multiplier;
        public string label;
    }
    [SerializeField] private float baseSpeed = 5f;
    [SerializeField] private float maxSpeed = 20f;
    [SerializeField] private Transform statSearchRoot;
    private IStat Istats;
    private readonly Dictionary<Component, SpeedEntry> modifier = new();
    void Awake()
    {
        var root = statSearchRoot ? statSearchRoot : this.transform;
        foreach (var mb in statSearchRoot.GetComponentsInChildren<MonoBehaviour>(true))
        {
            if (mb is IStat s)
            {
                Istats = s;
                break;
            }
        }
    }
    public void SetBase(float baseSpeed)
    {
        this.baseSpeed = baseSpeed;
    }
    public float EffectiveSpeed
    {
        get
        {
            float _speed = Istats == null ? baseSpeed : Mathf.Max(0, Istats.GetStatTypeOf(StatType.MoveSpeed));
            float m = 1f;
            foreach (var v in modifier.Values)
            {
                m *= Mathf.Clamp(v.multiplier, 0f, maxSpeed);
            }

            return _speed * m;

        }
    }

    public void SetModifier(Component source, float multiplier, string label = null)
    {
        modifier[source] = new SpeedEntry
        {
            multiplier = multiplier,
            label = label
        };
    }
    public void RemoveModifier(Component source) => modifier.Remove(source);
    

    // For debugging purpose
    public IEnumerable<(Component source, string label, float multiplier)> DebugSnapshot()
    {
        foreach (var kv in modifier) yield return (kv.Key, kv.Value.label, kv.Value.multiplier);
    }
}
