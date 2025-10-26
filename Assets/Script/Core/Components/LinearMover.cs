using UnityEngine;

[RequireComponent(typeof(Rigidbody2D), typeof(Collider2D))]
[DisallowMultipleComponent]
public class LinearMover : MonoBehaviour, IMover2D
{
    [Header("Speed")]
    [SerializeField] private SpeedModifierStack speedStack;
    [SerializeField] private float baseSpeed = 6f;
    public float MoveSpeed => speedStack ? speedStack.EffectiveSpeed : baseSpeed;

    [Header("Jump / Air Boost")]
    [SerializeField] private float jumpVelocity = 5f;
    [Tooltip("Multiplier applied while NOT grounded (e.g., 1.25 = 25% faster in air).")]
    [SerializeField] private float airSpeedMultiplier = 1.25f;

    [Header("Ground Check")]
    [SerializeField] private LayerMask groundMask;
    [SerializeField] private Vector2 groundCheckOffset = new(0f, -0.55f);
    [SerializeField] private float groundCheckRadius = 0.12f;

    private Rigidbody2D rb;
    private Vector2 wantedDir;
    [SerializeField] private bool isGrounded;

    [Header("Debug (read-only)")]
    [SerializeField] private float debugEffectiveSpeed; // MoveSpeed after all modifiers
    [SerializeField] private Vector2 debugVelocity;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        rb.constraints = RigidbodyConstraints2D.FreezeRotation;

        if (!speedStack)
            speedStack = GetComponentInParent<SpeedModifierStack>() ?? GetComponent<SpeedModifierStack>();
    }

    void FixedUpdate()
    {
        isGrounded = IsGrounded();

        if (speedStack)
        {
            if (!isGrounded)
                speedStack.SetModifier(this, Mathf.Max(airSpeedMultiplier, 0f), "AirBoost");
            else
                speedStack.RemoveModifier(this); 
        }


        float targetVx = Mathf.Clamp(wantedDir.x, -1f, 1f) * MoveSpeed;
        rb.linearVelocity = new Vector2(targetVx, rb.linearVelocityY);

        debugEffectiveSpeed = MoveSpeed;
        debugVelocity = rb.linearVelocity;
    }

    public void Move(Vector2 dir)
    {
        // Platformer: we only use X, but keep full signature
        wantedDir = dir;
    }

    public void Jump()
    {
        if (!isGrounded) return;
        rb.linearVelocity = new Vector2(rb.linearVelocityX, jumpVelocity);
        isGrounded = false; 
    }

    private bool IsGrounded()
    {
        Vector2 p = (Vector2)transform.position + groundCheckOffset;
        return Physics2D.OverlapCircle(p, groundCheckRadius, groundMask);
    }

#if UNITY_EDITOR
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Vector2 p = (Vector2)transform.position + groundCheckOffset;
        Gizmos.DrawWireSphere(p, groundCheckRadius);
    }
#endif
}
