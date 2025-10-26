using UnityEngine;

/// <summary>
/// Attach this to any object that should unlock an animal (e.g., a rescue trigger).
/// Calls the global AnimalCodexSession to register the unlock.
/// </summary>
public class AnimalDataUnlocker : MonoBehaviour
{
    [Header("References")]
    [Tooltip("Which animal to unlock when this trigger happens.")]
    public AnimalDataSO animalToUnlock;

    [Tooltip("If true, unlocks automatically on Start (for testing only).")]
    public bool unlockOnStart = false;

    [Tooltip("Tag of the player object that can trigger the unlock.")]
    public string playerTag = "Player";

    private void Start()
    {
        if (unlockOnStart)
            UnlockNow();
    }

    /// <summary>Call this from your gameplay logic to unlock the assigned animal.</summary>
    public void UnlockNow()
    {
        if (!animalToUnlock) return;

        // use the global session singleton
        var session = AnimalCodexSession.Instance;
        if (session && session.Unlock(animalToUnlock.id))
        {
            Debug.Log($"[AnimalUnlocker] Unlocked {animalToUnlock.commonNameTH}");
        }
    }
}
