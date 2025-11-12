using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInputReader : MonoBehaviour, IInputReader
{
    [SerializeField] private InputActionReference move;
    [SerializeField] private InputActionReference jump;
    [SerializeField] private InputActionReference block;
    [SerializeField] private InputActionReference shoot;

    [SerializeField] private InputActionReference interact;
    [SerializeField] private InputActionReference slot1;
    [SerializeField] private InputActionReference slot2;
    [SerializeField] private InputActionReference slotScroll;

    private void OnEnable()
    {
        move?.action.Enable(); 
        jump?.action.Enable();
        block?.action.Enable(); 
        shoot?.action.Enable();
        interact?.action.Enable();
        slot1?.action.Enable();
        slot2?.action.Enable();
        slotScroll?.action.Enable();
    }

    private void OnDisable()
    {
        move?.action.Disable(); 
        jump?.action.Disable();
        block?.action.Disable(); 
        shoot?.action.Disable();
        interact?.action.Disable();
        slot1?.action.Disable();
        slot2?.action.Disable();
        slotScroll?.action.Disable();
    }

    public InputSnapshot Read()
    {
        var s = new InputSnapshot();
        s.Move = move ? move.action.ReadValue<Vector2>() : Vector2.zero;
        s.JumpPressed   = jump     && jump.action.WasPerformedThisFrame();
        s.BlockPressed  = block    && block.action.WasPerformedThisFrame();
        s.ShootPressed  = shoot    && shoot.action.WasPerformedThisFrame();
        s.ShootHeld     = shoot    && shoot.action.IsPressed();
        s.InteractPressed = interact && interact.action.WasPerformedThisFrame();
        s.Slot1Pressed = slot1 && slot1.action.WasPerformedThisFrame();
        s.Slot2Pressed = slot2 && slot2.action.WasPerformedThisFrame();

        float scrollY = slotScroll ? slotScroll.action.ReadValue<Vector2>().y 
                           : 0f; // some bindings give Vector2; if float binding, use ReadValue<float>()
        s.ScrollDeltaY = scrollY;
        
        return s;
    }
}
