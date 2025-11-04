using System.Collections;
using UnityEngine;

[DisallowMultipleComponent]
public class HealOverTimeRunner : MonoBehaviour
{
    /// <summary>
    /// Starts a heal-over-time on 'target'. Returns the runner instance.
    /// If an existing runner with the same tag is present, you can choose to stack or replace.
    /// </summary>
    public static HealOverTimeRunner StartOn(GameObject target, int totalHeal, float duration, int ticksPerSecond = 5,
                                             bool stack = true, string effectTag = "Bandage")
    {
        if (!target) return null;

        // optional: avoid duplicates by tag
        var existing = target.GetComponent<HealOverTimeRunner>();
        if (existing && !stack)
        {
            Destroy(existing);
            existing = null;
        }
        var runner = existing ? existing : target.AddComponent<HealOverTimeRunner>();
        runner.totalHeal = Mathf.Max(0, totalHeal);
        runner.duration = Mathf.Max(0.01f, duration);
        runner.ticksPerSecond = Mathf.Max(1, ticksPerSecond);
        runner.effectTag = effectTag;

        runner.Restart();
        return runner;
    }

    [SerializeField] private int totalHeal;
    [SerializeField] private float duration;
    [SerializeField] private int ticksPerSecond = 5;
    [SerializeField] private string effectTag = "Bandage";

    private Coroutine routine;

    public void Restart()
    {
        if (routine != null) StopCoroutine(routine);
        routine = StartCoroutine(CoHeal());
    }

    private IEnumerator CoHeal()
    {
        var health = GetComponentInChildren<HealthComponent>();
        if (!health || totalHeal <= 0 || duration <= 0f) { Destroy(this); yield break; }

        int ticks = Mathf.Max(1, Mathf.RoundToInt(duration * ticksPerSecond));
        // distribute totalHeal across ticks (handle remainder at the end)
        int perTick = Mathf.Max(1, totalHeal / ticks);
        int healed = 0;

        float dt = 1f / ticksPerSecond;
        for (int i = 0; i < ticks; i++)
        {
            int toHeal = (i == ticks - 1) ? (totalHeal - healed) : perTick; // push remainder to last tick
            if (toHeal > 0 && !health.IsDead) health.Heal(toHeal);
            healed += toHeal;
            yield return new WaitForSeconds(dt);
        }
        Destroy(this);
    }
}
