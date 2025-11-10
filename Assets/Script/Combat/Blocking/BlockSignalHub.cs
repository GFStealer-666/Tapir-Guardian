using System;
using UnityEngine;

/// <summary>
/// Bridges BlockComponent state to simple events and allows the damage system
/// to report an actual blocked hit (normal/perfect) without modifying BlockComponent.
/// </summary>
[DisallowMultipleComponent]
public class BlockSignalHub : MonoBehaviour
{
    [SerializeField] private BlockComponent block;
    [SerializeField] private DamageReceiver damageReceiver;
    public event Action OnBlockStarted;
    public event Action OnBlockEnded;
    public event Action<bool> OnBlockHit; // bool isPerfect

    private bool prevBlocking;

    private void Reset()
    {
        if (!block) block = GetComponentInParent<BlockComponent>() ?? GetComponent<BlockComponent>();
    }

    private void Awake()
    {
        if (!block) block = GetComponentInParent<BlockComponent>() ?? GetComponent<BlockComponent>();
    }
    private void OnEnable()
    {
        damageReceiver.OnBlocked += ReportBlockHit;
    }
    private void OnDisable()
    {
        damageReceiver.OnBlocked -= ReportBlockHit;
    }
    private void Update()
    {
        if (!block) return;

        bool isBlocking = block.IsBlocking;

        // Rising edge => player pressed block (or block started) -> show shield icon
        if (!prevBlocking && isBlocking)
            OnBlockStarted?.Invoke();

        // Falling edge => block ended -> hide/reset
        if (prevBlocking && !isBlocking)
            OnBlockEnded?.Invoke();

        prevBlocking = isBlocking;
    }

    /// <summary>
    /// Call this from your damage/attack resolution when an incoming hit
    /// is actually blocked by the player. Pass whether it is perfect.
    /// </summary>
    /// 
    public void ReportBlockHit(bool isPerfect)
    {
        OnBlockHit?.Invoke(isPerfect);
    }
}
