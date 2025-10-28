using UnityEngine;

[DisallowMultipleComponent]
public class PlayerBrain : MonoBehaviour
{
    [SerializeField] private MonoBehaviour inputReaderBehaviour; // UnityInputReader
    private IInputReader input;

    [Header("Gameplay refs")]
    [SerializeField] private IMover2D mover;
    [SerializeField] private IBlock blocker;
    [SerializeField] private WeaponDriver weaponDriver;

    [Header("Inventory/Eq/Hotbar")]
    [SerializeField] private EquipmentComponent equipment;
    [SerializeField] private HotbarComponent hotbar;

    [Header("Interact")]
    [SerializeField] private InteractionScanner scanner;

    [Header("Control Lock")]
    [SerializeField] private PlayerControlLock controlLock;
    [Header("Audio")]
    [SerializeField] private PlayerAudioSoundEffect sfx;

    [Header("Footstep Settings")]
    [SerializeField, Min(0.05f)] private float baseStepInterval = 0.45f; // seconds at 1.0 speed
    [SerializeField, Min(0.01f)] private float minStepInterval = 0.22f;  // clamp floor
    [SerializeField, Min(0.01f)] private float speedForFastestSteps = 9f; // speed where interval hits min

    private float _stepTimer;

    private void Awake()
    {
        if (!sfx) sfx = GetComponent<PlayerAudioSoundEffect>() ?? GetComponentInChildren<PlayerAudioSoundEffect>();

        input = inputReaderBehaviour as IInputReader;

        if (!weaponDriver) weaponDriver = GetComponent<WeaponDriver>() ?? GetComponentInChildren<WeaponDriver>();
        if (!equipment)    equipment    = GetComponent<EquipmentComponent>() ?? GetComponentInChildren<EquipmentComponent>();
        if (!hotbar)       hotbar       = GetComponent<HotbarComponent>() ?? GetComponentInChildren<HotbarComponent>();

        if (mover == null)   mover   = GetComponent<IMover2D>() ?? GetComponentInParent<IMover2D>();
        if (blocker == null) blocker = GetComponent<IBlock>() ?? GetComponentInChildren<IBlock>() ?? GetComponentInParent<IBlock>();

        if (!scanner) scanner = GetComponentInChildren<InteractionScanner>();
        if (!controlLock) controlLock = GetComponent<PlayerControlLock>() ?? GetComponentInChildren<PlayerControlLock>();

        if(mover == null) { Debug.LogWarning("Mover missing"); }
    }

    private void Update()
    {
        var s = input?.Read() ?? default;

        // If UI is open / player frozen, skip movement+combat
        if (!controlLock || !controlLock.InputBlocked)
        {
            // Movement
            var d = s.Move;
            if (d.sqrMagnitude > 1f) d.Normalize();
            mover?.Move(new Vector2(d.x, 0f));
            if (s.JumpPressed) mover?.Jump();
            if (s.BlockPressed) blocker?.TryBlock();

            // Facing for shots/melee arcs
            weaponDriver?.UpdateFacingFromInput(d);

            // // Hotbar switch
            // if (s.HotbarPressedIndex > 0 && hotbar && equipment)
            // {
            //     bool ok = hotbar.ConfigureSlot(s.HotbarPressedIndex, equipment);
            //     // TODO: feedback if !ok
            // }

            // Attack
            if (s.ShootPressed || s.ShootHeld)
                weaponDriver?.TryAttack();
        }
        else
        {
            // when locked, force no movement
            mover?.Move(Vector2.zero);
        }

        // Interact (press E)
        if (s.InteractPressed)
        {
            var target = scanner ? scanner.Current : null;
            if (target != null && target.CanInteract())
            {
                target.Interact(this);
            }
        }
        // Cache a typed mover so we can read IsGrounded and velocity (optional)
        var linearMover = mover as LinearMover;

        // Jump — play sound only if we were grounded when the jump button is pressed
        if (!controlLock || !controlLock.InputBlocked)
        {
            var d = s.Move;
            if (d.sqrMagnitude > 1f) d.Normalize();

            // Move
            mover?.Move(new Vector2(d.x, 0f));

            // Jump + sound
            if (s.JumpPressed)
            {
                if (linearMover == null || linearMover.IsGrounded()) // if we can check grounding, ensure grounded
                    sfx?.PlayJump();
                mover?.Jump();
            }

            if (s.BlockPressed) blocker?.TryBlock();

            // Facing for shots/melee arcs
            weaponDriver?.UpdateFacingFromInput(d);

            // Attack
            if (s.ShootPressed || s.ShootHeld)
                weaponDriver?.TryAttack();

            // ---- Footstep cadence ----
            HandleFootsteps(d, linearMover);
        }
        else
        {
            // when locked, force no movement
            mover?.Move(Vector2.zero);
            _stepTimer = 0f; // reset cadence when control is locked
        }

        // Interact (press E)
        if (s.InteractPressed)
        {
            var target = scanner ? scanner.Current : null;
            if (target != null && target.CanInteract())
            {
                target.Interact(this);
            }
        }

    }
    
    private void HandleFootsteps(Vector2 inputDir, LinearMover lm)
{
    // Must be grounded and moving horizontally to produce steps
    if (!lm || !lm.IsGrounded())
    {
        _stepTimer = 0f;
        return;
    }

    // Horizontal speed estimate: prefer rigidbody velocity if available for better feel
    float absVx = 0f;
    if (lm)
    {
        // Uses rb.linearVelocityX from LinearMover's FixedUpdate result
        absVx = Mathf.Abs(lm.GetComponent<Rigidbody2D>().linearVelocityX);
    }
    else
    {
        // Fallback if mover is not LinearMover: estimate from input
        absVx = Mathf.Abs(inputDir.x);
    }

    // Not moving enough? no steps.
    const float moveThreshold = 0.05f;
    if (absVx <= moveThreshold)
    {
        _stepTimer = 0f;
        return;
    }

    // Map speed to step interval
    // Faster speed → shorter interval, clamped by minStepInterval
    float t = Mathf.InverseLerp(0f, Mathf.Max(speedForFastestSteps, 0.01f), absVx);
    float interval = Mathf.Lerp(baseStepInterval, minStepInterval, t);

    _stepTimer -= Time.deltaTime;
    if (_stepTimer <= 0f)
    {
        sfx?.PlayWalkStep();
        _stepTimer = interval;
    }
}


    // Public so UI can freeze/unfreeze player
    public void SetInputBlocked(bool blocked)
    {
        if (!controlLock) return;
        controlLock.InputBlocked = blocked;
    }
}
