using UnityEngine;

public class HerbUnlocker : MonoBehaviour
{
    [Header("Settings")]
    public HerbDataSO herbToUnlock;
    public bool unlockOnStart = false;
    public string playerTag = "Player";

    private void Start()
    {
        if (unlockOnStart)
            UnlockNow();
    }

    public void UnlockNow()
    {
        if (!herbToUnlock) return;
        var session = HerbCodexSession.Instance;
        if (session && session.Unlock(herbToUnlock.id))
            Debug.Log($"[HerbUnlocker] Unlocked herb: {herbToUnlock.nameTH}");
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag(playerTag))
        {
            UnlockNow();
            // Optionally disable trigger
            // gameObject.SetActive(false);
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag(playerTag))
        {
            UnlockNow();
            // gameObject.SetActive(false);
        }
    }
}
