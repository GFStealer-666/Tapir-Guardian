using System.Collections;
using UnityEngine;

/// Make the tapir run left and leave the scene after a signal.
[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(Rigidbody2D))]
public class AnimalAnimation : MonoBehaviour
{
    [Header("Animation")]
    [Tooltip("Animator parameter to trigger run animation.")]
    public string runParam = "IsRunning";     // Bool or Trigger
    public bool runParamIsTrigger = false;

    [Tooltip("Animator parameter for idle animation (optional). Leave empty if handled automatically.")]
    public string idleParam = "IsIdle";       // optional Bool, leave blank if not used

    [Header("Movement")]
    [SerializeField, Min(0.1f)] private float runSpeed = 5f;
    [SerializeField] private bool flipXWhenRunningLeft = true;
    [SerializeField] private bool zeroGravityWhileRunning = true;

    [Header("Destination")]
    [Tooltip("The point the tapir should run to.")]
    public Transform targetPoint;

    [Tooltip("Distance threshold to stop running (in world units).")]
    [SerializeField] private float stopDistance = 0.1f;

    [Header("Collision")]
    [SerializeField] private bool disableCollidersWhileRunning = true;

    Animator _anim;
    Rigidbody2D _rb;
    SpriteRenderer _sr;
    Collider2D[] _cols;
    bool _escaping;

    void Awake()
    {
        _anim = GetComponent<Animator>();
        _rb   = GetComponent<Rigidbody2D>();
        _sr   = GetComponentInChildren<SpriteRenderer>();
        _cols = GetComponentsInChildren<Collider2D>();
    }

    /// Call this when the event is triggered (for example, cage opened)
    public void TriggerEscape()
    {
        if (!_escaping && targetPoint)
            StartCoroutine(EscapeRoutine());
    }

    IEnumerator EscapeRoutine()
    {
        _escaping = true;

        // Setup
        if (zeroGravityWhileRunning) _rb.gravityScale = 0f;
        if (disableCollidersWhileRunning) foreach (var c in _cols) c.enabled = false;

        Vector3 targetPos = targetPoint.position;
        bool runningLeft = targetPos.x < transform.position.x;

        if (_sr && flipXWhenRunningLeft)
            _sr.flipX = runningLeft;

        // Start run animation
        if (_anim)
        {
            if (runParamIsTrigger) _anim.SetTrigger(runParam);
            else _anim.SetBool(runParam, true);
        }

        // Move until near target
        while (Vector2.Distance(transform.position, targetPos) > stopDistance)
        {
            float dir = runningLeft ? -1f : 1f;
            _rb.linearVelocity = new Vector2(dir * runSpeed, 0f);
            yield return null;
        }

        // Stop
        _rb.linearVelocity = Vector2.zero;

        // Back to idle
        if (_anim)
        {
            if (!runParamIsTrigger)
                _anim.SetBool(runParam, false);
            if (!string.IsNullOrEmpty(idleParam))
                _anim.SetBool(idleParam, true);
        }

        // Restore colliders
        if (disableCollidersWhileRunning)
            foreach (var c in _cols) c.enabled = true;

        _escaping = false;
    }
}
