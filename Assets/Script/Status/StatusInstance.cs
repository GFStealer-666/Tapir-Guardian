using UnityEngine;

[System.Serializable]
public sealed class StatusInstance
{
    public StatusSO def;
    public float remaining;
    public int stacks;
    private float _tickTimer;

    public StatusInstance(StatusSO def)
    {
        this.def = def;
        remaining = Mathf.Max(0f, def.duration);
        stacks = 1;
        _tickTimer = def.tickInterval;
    }

    public void RefreshDuration() => remaining = Mathf.Max(remaining, def.duration);
    public void AddStack()
    {
        stacks = Mathf.Clamp(stacks + 1, 1, Mathf.Max(1, def.maxStacks));
        remaining = Mathf.Max(remaining, def.duration);
    }

    public bool UpdateTimers(float dt, System.Action<StatusInstance> onTick)
    {
        remaining -= dt;
        if (def.tickInterval > 0f && onTick != null)
        {
            _tickTimer -= dt;
            if (_tickTimer <= 0f)
            {
                _tickTimer += def.tickInterval;
                onTick(this);
            }
        }
        return remaining <= 0f;
    }
}
