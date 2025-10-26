using UnityEngine;

public interface IAIState { void OnEnter(); void Tick(float dt); void OnExit(); }

[DisallowMultipleComponent]
public class StateMachine : MonoBehaviour
{
    public IAIState Current { get; private set; }

    [SerializeField, Tooltip("Read-only: shows the current state for debugging")]
    private string debugStateName = "None";
    public string DebugStateName => debugStateName;

    public System.Action<string> OnStateChanged;

    public void ChangeState(IAIState next)
    {
        if (Current == next) return;
        Current?.OnExit();
        Current = next;
        debugStateName = next != null ? next.GetType().Name : "None";
        OnStateChanged?.Invoke(debugStateName);
        Current?.OnEnter();
    }

    void Update() => Current?.Tick(Time.deltaTime);
}
