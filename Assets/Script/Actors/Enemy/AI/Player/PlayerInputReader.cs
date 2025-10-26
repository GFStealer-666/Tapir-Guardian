using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInputReader : MonoBehaviour, IInputReader
{
    [SerializeField] private InputActionReference move;
    [SerializeField] private InputActionReference jump;
    [SerializeField] private InputActionReference block;
    [SerializeField] private InputActionReference shoot;

    [SerializeField] private InputActionReference interact; // <-- NEW (E)

    private void OnEnable()
    {
        move?.action.Enable(); 
        jump?.action.Enable();
        block?.action.Enable(); 
        shoot?.action.Enable();
        interact?.action.Enable(); // <-- NEW
    }

    private void OnDisable()
    {
        move?.action.Disable(); 
        jump?.action.Disable();
        block?.action.Disable(); 
        shoot?.action.Disable();
        interact?.action.Disable(); // <-- NEW
    }

    public InputSnapshot Read()
    {
        var s = new InputSnapshot();
        s.Move = move ? move.action.ReadValue<Vector2>() : Vector2.zero;
        s.JumpPressed   = jump     && jump.action.WasPerformedThisFrame();
        s.BlockPressed  = block    && block.action.WasPerformedThisFrame();
        s.ShootPressed  = shoot    && shoot.action.WasPerformedThisFrame();
        s.ShootHeld     = shoot    && shoot.action.IsPressed();
        s.InteractPressed = interact && interact.action.WasPerformedThisFrame(); // <-- NEW
        return s;
    }
}
