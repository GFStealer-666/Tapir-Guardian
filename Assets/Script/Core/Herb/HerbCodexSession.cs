using System;
using System.Collections.Generic;
using UnityEngine;

[DefaultExecutionOrder(-500)]
public class HerbCodexSession : MonoBehaviour
{
    public static HerbCodexSession Instance { get; private set; }

    private readonly HashSet<string> _unlocked = new();

    public event Action OnChanged;

    void Awake()
    {
        ClearAllHerbUnlocks();
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        LoadFromPlayerPrefs();
        OnChanged?.Invoke();
    }

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
    static void AutoCreate()
    {
        if (Instance == null)
        {
            var go = new GameObject("HerbCodexSession");
            go.AddComponent<HerbCodexSession>();
        }
    }

    public bool Unlock(string id)
    {
        if (string.IsNullOrWhiteSpace(id)) return false;
        bool added = _unlocked.Add(id);
        if (added)
        {
            SaveToPlayerPrefs(id);
            OnChanged?.Invoke();
            Debug.Log($"[HerbCodexSession] Unlocked {id}");
        }
        return added;
    }

    public bool IsUnlocked(string id)
    {
        if (string.IsNullOrWhiteSpace(id)) return false;
        return _unlocked.Contains(id);
    }

    public IEnumerable<string> AllUnlocked() => _unlocked;

    public void ClearAll()
    {
        foreach (var key in _unlocked)
            PlayerPrefs.DeleteKey($"HERB_UNLOCK_{key}");
        PlayerPrefs.Save();
        _unlocked.Clear();
        OnChanged?.Invoke();
    }

    private void SaveToPlayerPrefs(string id)
    {
        PlayerPrefs.SetInt($"HERB_UNLOCK_{id}", 1);
        PlayerPrefs.Save();
    }

    private void LoadFromPlayerPrefs()
    {
        // If you have a master list, load from there.
        // Otherwise, you can iterate known ids manually.
        // Example:
        string[] knownIds = { "ToraneeYen" /* add more later */ };

        foreach (var id in knownIds)
        {
            if (PlayerPrefs.GetInt($"HERB_UNLOCK_{id}", 0) == 1)
                _unlocked.Add(id);
        }
    }
    public void ClearAllHerbUnlocks()
    {
        // If you use a prefix for keys (recommended), delete only those keys.
        // If not, use PlayerPrefs.DeleteAll() during testing only.
        foreach (var key in PlayerPrefsKeysWithPrefix("HERB_UNLOCK_"))
        {
            Debug.Log("[HerbCodex] Deleting PlayerPrefs key: " + key);
            PlayerPrefs.DeleteKey(key);
        }
        PlayerPrefs.Save();
        _unlocked.Clear();
        Debug.Log("[HerbCodex] Cleared all herb unlocks.");
    }

    private static System.Collections.Generic.IEnumerable<string> PlayerPrefsKeysWithPrefix(string prefix)
    {
        var ids = new[] { "ToraneeYen", /* add all herb ids you use */ };
        foreach (var id in ids) yield return $"HERB_UNLOCK_{id}";
    }
}
