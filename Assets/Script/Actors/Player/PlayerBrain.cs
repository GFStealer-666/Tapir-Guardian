using UnityEngine;

[DisallowMultipleComponent]
public class PlayerBrain : MonoBehaviour
{
    [Header("Input")]
    [SerializeField] private MonoBehaviour inputReaderBehaviour; // must implement IInputReader
    private IInputReader input;

    [Header("Gameplay refs")]
    [SerializeField] private IMover2D mover;
    [SerializeField] private BlockComponent blocker;
    [SerializeField] private HealthComponent healthComponent;
    [Header("Equipment/Combat")]
    [SerializeField] private WeaponDriver weaponDriver;
    [SerializeField] private WeaponSelectorController weaponSelectorController;

    [Header("Inventory/Eq/Hotbar")]
    [SerializeField] private EquipmentComponent equipment;
    [Header("Interact")]
    [SerializeField] private InteractionScanner scanner;

    [Header("Control Lock")]
    [SerializeField] private PlayerControlLock controlLock;
    [SerializeField] private bool lockPlayerOnStart = false;

    [Header("Audio")]
    [SerializeField] private PlayerAudioSoundEffect sfx;
    [Header("Animation")]
    [SerializeField] private PlayerAnimator2D animator2D;   

    [Header("Footstep Settings")]
    [SerializeField, Min(0.05f)] private float baseStepInterval = 0.45f; // seconds at slow speed
    [SerializeField, Min(0.01f)] private float minStepInterval  = 0.22f; // fastest cadence
    [SerializeField, Min(0.01f)] private float speedForFastestSteps = 9f;

    private float _stepTimer;

    // cached helpers
    private LinearMover _linearMover;          // if mover is LinearMover
    private Rigidbody2D _rb;                   // for velocity-based footsteps

    private void Awake()
    {

        if (!sfx) sfx = GetComponentInChildren<PlayerAudioSoundEffect>();
        if (!weaponDriver) weaponDriver = GetComponentInChildren<WeaponDriver>();
        if (!equipment) equipment = GetComponentInChildren<EquipmentComponent>();
        if (!scanner) scanner = GetComponentInChildren<InteractionScanner>();
        if (!controlLock) controlLock = GetComponentInChildren<PlayerControlLock>();
        if (!animator2D) animator2D = GetComponentInChildren<PlayerAnimator2D>();
        if (!healthComponent) healthComponent = GetComponentInChildren<HealthComponent>();

        input = inputReaderBehaviour as IInputReader;

        mover ??= GetComponent<IMover2D>() ?? GetComponentInParent<IMover2D>();


        _linearMover = mover as LinearMover;
        _rb = GetComponent<Rigidbody2D>();

        if (mover == null)
        {
            Debug.LogWarning($"{nameof(PlayerBrain)}: IMover2D missing.");
        }

    }
    void OnEnable()
    {
        healthComponent.OnDied += () =>
        {
            SetInputBlocked(true);
        };
    }

    void Start()
    {
        if(lockPlayerOnStart)
        {
            SetInputBlocked(true);
        }
    }

    private void Update()
    {
        var s = input?.Read() ?? default;

        // If locked: stop, clear cadence, and skip gameplay
        if (controlLock && controlLock.InputBlocked)
        {
            mover?.Move(Vector2.zero);
            _stepTimer = 0f;
            return;
        }

        Vector2 move = s.Move;
        if (move.sqrMagnitude > 1f) move.Normalize();

        mover?.Move(new Vector2(move.x, 0f));

        if (s.JumpPressed)
        {
            // play jump sound only when grounded (if we can check)
            if (_linearMover == null || _linearMover.IsGrounded())
                sfx?.PlayJump();
            mover?.Jump();
        }

        if (s.BlockPressed)
        {
            blocker?.TryBlock();
        }

        animator2D?.SetFacingByInput(move.x);
        // Attack (tap or hold)
        if (s.ShootPressed || s.ShootHeld)
        {
            weaponDriver?.OnAttackInput();
        }

        if (s.Slot1Pressed)
        {
            weaponSelectorController?.SelectIndex(0);
        }
        if (s.Slot2Pressed)
        {
            weaponSelectorController?.SelectIndex(1);
        }
        // ---- Footsteps ----
        HandleFootsteps(move);

        // ---- Interact ----
        if (s.InteractPressed)
        {
            var target = scanner ? scanner.Current : null;
            if (target != null && target.CanInteract())
            {
                target.Interact(this);
            }
        }
        
        const float threshold = 0.1f;
        if (s.ScrollDeltaY > threshold)  weaponSelectorController.SelectNext(+1);
        if (s.ScrollDeltaY < -threshold) weaponSelectorController.SelectNext(-1);
    }

    private void HandleFootsteps(Vector2 inputDir)
    {
        // must be grounded and moving
        if (_linearMover == null || !_linearMover.IsGrounded())
        {
            _stepTimer = 0f;
            return;
        }

        // prefer rigidbody velocity for cadence
        float absVx = 0f;
        if (_rb) absVx = Mathf.Abs(_rb.linearVelocityX);
        else     absVx = Mathf.Abs(inputDir.x);

        const float moveThreshold = 0.05f;
        if (absVx <= moveThreshold)
        {
            _stepTimer = 0f;
            return;
        }

        // map speed -> interval (faster => shorter)
        float t = Mathf.InverseLerp(0f, Mathf.Max(speedForFastestSteps, 0.01f), absVx);
        float interval = Mathf.Lerp(baseStepInterval, minStepInterval, t);

        _stepTimer -= Time.deltaTime;
        if (_stepTimer <= 0f)
        {
            sfx?.PlayWalkStep();
            _stepTimer = interval;
        }
    }

    // Public so UI/dialogue can freeze/unfreeze player
    public void SetInputBlocked(bool blocked)
    {
        if (!controlLock) return;
        controlLock.InputBlocked = blocked;
        if (blocked)
        {
            mover?.Move(Vector2.zero);
            _stepTimer = 0f;
        }
    }
    public void ReleasePlayerInput()
    {
        controlLock.InputBlocked = false;
    }
}
