using System.Collections;
using UnityEngine;

[DisallowMultipleComponent]
[RequireComponent(typeof(Rigidbody2D))]
public class AnimalAnimation : MonoBehaviour
{
    [Header("Animation")]
    public string runParam = "IsRunning";   // Bool
    public bool runParamIsTrigger = false;  // leave false (Bool)
    public string idleParam = "IsIdle";     // optional

    [Header("Movement")]
    [SerializeField, Min(0.1f)] private float runSpeed = 5f;
    [SerializeField] private bool flipXWhenRunningLeft = true;
    [SerializeField] private bool zeroGravityWhileRunning = true;

    [Header("Destination")]
    public Transform targetPoint;
    [SerializeField] private float stopDistance = 0.1f;

    [Header("Collision")]
    [SerializeField] private bool disableCollidersWhileRunning = true;

    Animator _anim;
    Rigidbody2D _rb;
    SpriteRenderer _sr;
    Collider2D[] _cols;
    bool _escaping;
    float _savedGravity;

    void Awake()
    {
        _anim = GetComponentInChildren<Animator>();      // Animator on child (Visual)
        _rb   = GetComponent<Rigidbody2D>();
        _sr   = GetComponentInChildren<SpriteRenderer>();
        _cols = GetComponentsInChildren<Collider2D>();
    }

    public void TriggerEscape()
    {
        if (_escaping || !targetPoint) return;
        StartCoroutine(EscapeRoutine());
    }

    IEnumerator EscapeRoutine()
    {
        _escaping = true;

        // setup
        _savedGravity = _rb.gravityScale;
        if (zeroGravityWhileRunning) _rb.gravityScale = 0f;
        if (disableCollidersWhileRunning) foreach (var c in _cols) c.enabled = false;

        Vector3 targetPos = targetPoint.position;
        bool runningLeft = targetPos.x < transform.position.x;

        if (_sr && flipXWhenRunningLeft) _sr.flipX = runningLeft;

        // start run animation
        if (_anim)
        {
            if (runParamIsTrigger) _anim.SetTrigger(runParam);
            else _anim.SetBool(runParam, true);
            if (!string.IsNullOrEmpty(idleParam)) _anim.SetBool(idleParam, false);
        }

        // move with physics
        // Use velocity (simple). If you prefer MovePosition, swap the block below.
        while (Vector2.Distance(transform.position, targetPos) > stopDistance)
        {
            float dir = runningLeft ? -1f : 1f;
            _rb.linearVelocity = new Vector2(dir * runSpeed, 0f);   // << fixed here
            yield return null; // or WaitForFixedUpdate() if using MovePosition
        }

        // stop
        _rb.linearVelocity = Vector2.zero;

        // back to idle
        if (_anim && !runParamIsTrigger) _anim.SetBool(runParam, false);
        if (_anim && !string.IsNullOrEmpty(idleParam)) _anim.SetBool(idleParam, true);

        // restore
        if (disableCollidersWhileRunning) foreach (var c in _cols) c.enabled = true;
        if (zeroGravityWhileRunning) _rb.gravityScale = _savedGravity;

        _escaping = false;
    }
}
