using System.Collections;
using UnityEngine;

/// Make the tapir run left and leave the scene after a signal.
[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(Rigidbody2D))]
public class AnimalAnimation : MonoBehaviour
{
    [Header("Animation")]
    [Tooltip("Animator bool or trigger to enter the run animation.")]
    public string runParam = "IsRunning";     // set to your Animator parameter
    public bool runParamIsTrigger = false;    // true if it's a Trigger, false if it's a Bool

    [Header("Movement")]
    [SerializeField, Min(0.1f)] private float runSpeed = 5f;
    [SerializeField] private bool flipXWhenEscaping = true; // face left if your sprite uses flipX
    [SerializeField] private bool zeroGravityWhileRunning = true;

    [Header("Exit")]
    [Tooltip("How far past the left screen edge we consider 'off map' (world units).")]
    [SerializeField] private float leftMargin = 2f;
    [Tooltip("Destroy game object after exiting. Untick if you just want to disable it.")]
    [SerializeField] private bool destroyAfterExit = true;

    [Header("Optional: turn off collisions while escaping")]
    [SerializeField] private bool disableCollidersOnEscape = true;

    Animator _anim;
    Rigidbody2D _rb;
    SpriteRenderer _sr;
    Collider2D[] _cols;
    [SerializeField] private bool _escaping;

    void Awake()
    {
        _anim = GetComponent<Animator>();
        _rb   = GetComponent<Rigidbody2D>();
        _sr   = GetComponentInChildren<SpriteRenderer>();
        _cols = GetComponentsInChildren<Collider2D>();
    }

    /// Call this from your event (button, trigger, dialogue, etc.)
    public void TriggerEscape()
    {
        if (_escaping) return;
        _escaping = true;
        StartCoroutine(EscapeRoutine());
    }

    IEnumerator EscapeRoutine()
    {
        // Prep visuals & physics
        if (_sr && flipXWhenEscaping) _sr.flipX = true;      // face left
        if (zeroGravityWhileRunning)  _rb.gravityScale = 0f; // keep flat
        if (disableCollidersOnEscape) foreach (var c in _cols) c.enabled = false;

        // Enter run animation
        if (_anim)
        {
            if (runParamIsTrigger) _anim.SetTrigger(runParam);
            else _anim.SetBool(runParam, true);
        }

        // Compute world X to exit (slightly beyond left screen edge)
        var cam = Camera.main;
        float leftEdgeX = cam != null
            ? cam.ViewportToWorldPoint(new Vector3(0f, 0.5f, 0f)).x - leftMargin
            : transform.position.x - 30f; // fallback

        // Run left until off-screen
        while (transform.position.x > leftEdgeX)
        {
            _rb.linearVelocity = new Vector2(-runSpeed, 0f);
            yield return null;
        }

        // Stop & clean up
        _rb.linearVelocity = Vector2.zero;
        if (_anim && !runParamIsTrigger) _anim.SetBool(runParam, false);

        if (destroyAfterExit) Destroy(gameObject);
        else gameObject.SetActive(false);
    }
}
