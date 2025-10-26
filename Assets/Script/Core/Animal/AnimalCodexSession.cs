using System;
using System.Collections.Generic;
using UnityEngine;

[DefaultExecutionOrder(-500)] // make sure it exists early
public class AnimalCodexSession : MonoBehaviour
{
    public static AnimalCodexSession Instance { get; private set; }

    // Internal runtime set
    private readonly HashSet<string> _unlocked = new();

    // Inspector-visible mirror of _unlocked
    [Header("Debug / Inspector View")]
    [Tooltip("Current unlocked animal IDs (runtime mirror of _unlocked). You can edit this in Play Mode.")]
    [SerializeField] private List<string> unlockedDebugList = new();

    [Header("Debug Tools")]
    [Tooltip("When true, these IDs will be added as unlocked on Awake (for testing).")]
    [SerializeField] private List<string> forceUnlockOnStart = new();

    [Tooltip("If true, session will start empty and ignore any forceUnlockOnStart. Good for clean testing.")]
    [SerializeField] private bool startClean = false;

    public event Action OnChanged;

    // ---------------------------------------------------------------------
    // LIFECYCLE / SINGLETON
    // ---------------------------------------------------------------------
    void Awake()
    {
        // singleton pattern
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        // init runtime state
        if (startClean)
        {
            _unlocked.Clear();
            unlockedDebugList.Clear();
        }
        else
        {
            // preload test animals from forceUnlockOnStart
            foreach (var id in forceUnlockOnStart)
            {
                if (string.IsNullOrWhiteSpace(id)) continue;
                _unlocked.Add(id);
                Debug.Log($"[AnimalCodexSession] Force unlocked on start: {id}");
            }

            // sync mirror for inspector
            SyncListFromSet();
        }

        // notify listeners that we're initialized
        OnChanged?.Invoke();
    }

    // this method will auto-spawn the session the first time a scene loads
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
    static void AutoCreate()
    {
        if (Instance == null)
        {
            var go = new GameObject("AnimalCodexSession");
            go.AddComponent<AnimalCodexSession>();
        }
    }

    // ---------------------------------------------------------------------
    // PUBLIC API (runtime use in game)
    // ---------------------------------------------------------------------
    public bool Unlock(string id)
    {
        if (string.IsNullOrWhiteSpace(id)) return false;

        bool added = _unlocked.Add(id);
        if (added)
        {
            SyncListFromSet();
            OnChanged?.Invoke();
            Debug.Log($"[AnimalCodexSession] Unlock -> {id}");
        }
        return added;
    }

    public bool LockRemove(string id)
    {
        if (string.IsNullOrWhiteSpace(id)) return false;

        bool removed = _unlocked.Remove(id);
        if (removed)
        {
            SyncListFromSet();
            OnChanged?.Invoke();
            Debug.Log($"[AnimalCodexSession] LockRemove -> {id}");
        }
        return removed;
    }

    public bool IsUnlocked(string id)
    {
        return !string.IsNullOrWhiteSpace(id) && _unlocked.Contains(id);
    }

    public IEnumerable<string> AllUnlocked()
    {
        return _unlocked;
    }

    public void ResetSession()
    {
        _unlocked.Clear();
        SyncListFromSet();
        OnChanged?.Invoke();
        Debug.Log("[AnimalCodexSession] ResetSession -> cleared all unlocks");
    }

    // ---------------------------------------------------------------------
    // DEBUG SYNC (Inspector ↔ Runtime)
    // ---------------------------------------------------------------------

    // push _unlocked → unlockedDebugList (so Inspector shows latest)
    private void SyncListFromSet()
    {
        unlockedDebugList.Clear();
        unlockedDebugList.AddRange(_unlocked);
    }

    // pull unlockedDebugList → _unlocked (lets you edit in Inspector in Play Mode)
    private void SyncSetFromList()
    {
        _unlocked.Clear();
        foreach (var id in unlockedDebugList)
        {
            if (!string.IsNullOrWhiteSpace(id))
                _unlocked.Add(id.Trim());
        }
    }

#if UNITY_EDITOR
    // Unity calls this every time you change values in inspector
    void OnValidate()
    {
        // if the object is not active in the scene yet (e.g. prefab time), skip runtime sync
        if (Application.isPlaying && Instance == this)
        {
            // If dev edits unlockedDebugList while in Play mode,
            // push those changes back into the actual runtime set.
            SyncSetFromList();
            OnChanged?.Invoke();
        }
    }
#endif
}
