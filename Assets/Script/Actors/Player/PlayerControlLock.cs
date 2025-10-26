using UnityEngine;

[DisallowMultipleComponent]
public class PlayerControlLock : MonoBehaviour
{
    [Tooltip("If true, Time.timeScale = 0 when locked.")]
    public bool hardPauseTime = false;

    private bool _blocked;
    public bool InputBlocked
    {
        get => _blocked;
        set
        {
            if (_blocked == value) return;
            _blocked = value;
            if (hardPauseTime)
            {
                Time.timeScale = _blocked ? 0f : 1f;
            }
        }
    }
}
