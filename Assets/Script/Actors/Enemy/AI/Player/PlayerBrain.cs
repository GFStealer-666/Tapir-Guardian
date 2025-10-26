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
    [SerializeField] private InteractionScanner scanner; // <-- NEW

    [Header("Control Lock")]
    [SerializeField] private PlayerControlLock controlLock; // <-- NEW

    private void Awake()
    {
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
            if (s.JumpPressed)  mover?.Jump();
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
    }

    // Public so UI can freeze/unfreeze player
    public void SetInputBlocked(bool blocked)
    {
        if (!controlLock) return;
        controlLock.InputBlocked = blocked;
    }
}
