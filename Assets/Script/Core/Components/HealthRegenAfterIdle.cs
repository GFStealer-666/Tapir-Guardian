using UnityEngine;

/// <summary>
/// Regenerates health after the player has avoided damage for a short delay.
/// Attach to the same GameObject that has HealthComponent.
/// </summary>
[RequireComponent(typeof(HealthComponent))]
[DisallowMultipleComponent]
public class HealthRegenAfterIdle : MonoBehaviour
{
    [Header("Regen Logic")]
    [Tooltip("Seconds you must avoid damage before regen starts.")]
    [SerializeField] private float delayAfterDamage = 3f;

    [Tooltip("Health points regenerated per second once regen is active.")]
    [SerializeField] private float hpPerSecond = 5f;

    [Tooltip("How often to apply regen ticks (smaller = smoother).")]
    [SerializeField] private float tickInterval = 0.2f;

    [SerializeField] private HealthComponent _health;
    private float _nextRegenTime;
    private int _lastKnownHP;
    private float _carry;               // carries fractional HP across ticks
    private Coroutine _loop;

    void Awake()
    {
        _health = GetComponent<HealthComponent>();
    }

    void OnEnable()
    {
        // Initialize & subscribe
        _lastKnownHP = _health.CurrentHealth;
        _health.OnHealthChanged += HandleHealthChanged;
        _health.OnDied += HandleDied;

        _loop ??= StartCoroutine(RegenLoop());
    }

    void OnDisable()
    {
        _health.OnHealthChanged -= HandleHealthChanged;
        _health.OnDied -= HandleDied;

        if (_loop != null) { StopCoroutine(_loop); _loop = null; }
    }

    private void HandleHealthChanged(int current, int max)
    {
        // If HP decreased => damage taken => push back the regen window
        if (current < _lastKnownHP)
            _nextRegenTime = Time.time + delayAfterDamage;

        // If HP increased via other heals, don't delay; regen loop will simply pause at full.
        _lastKnownHP = current;
    }

    private void HandleDied()
    {
        _nextRegenTime = Mathf.Infinity;
        this.enabled = false;
    }

    private System.Collections.IEnumerator RegenLoop()
    {
        var wait = new WaitForSeconds(tickInterval);

        while (true)
        {
            yield return wait;

            if (_health == null || _health.IsDead) continue;
            if (_health.CurrentHealth >= _health.MaxHealth) { _carry = 0f; continue; }
            if (Time.time < _nextRegenTime) continue;

            float hpToAddFloat = hpPerSecond * tickInterval + _carry;
            int hpToAdd = Mathf.FloorToInt(hpToAddFloat);
            _carry = hpToAddFloat - hpToAdd;

            if (hpToAdd > 0)
                _health.Heal(hpToAdd);
        }
    }

    #region Debug
    [ContextMenu("Debug: Pretend Took Damage Now")]
    private void Debug_DamageReset()
    {
        _nextRegenTime = Time.time + delayAfterDamage;
        Debug.Log("[HealthRegenAfterIdle] Regen delayed due to debug damage.");
    }
    #endregion
}
