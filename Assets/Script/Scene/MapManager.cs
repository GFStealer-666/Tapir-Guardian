using UnityEngine;

[DisallowMultipleComponent]
public class MapManager : MonoBehaviour
{
    public static MapManager Current { get; private set; }

    [Header("Scene/Map Identity")]
    public string mapId; // e.g., "Village", "Lake", etc.

    [Header("Optional: default spawn point if none specified")]
    public string defaultSpawnPointId = "Spawn_Default";

    void Awake()
    {
        // ensure exactly one per scene
        if (Current && Current != this)
        {
            Debug.LogWarning($"[MapManager] Duplicate detected in scene. Keeping the first.");
            Destroy(gameObject);
            return;
        }
        Current = this;

        // place the player at the designated spawn
        SnapPlayerToSpawn();
    }

    void OnDestroy()
    {
        if (Current == this) Current = null;
    }

    void SnapPlayerToSpawn()
    {
        if (!PlayerLocator.TryGet(out var player)) return;

        var id = string.IsNullOrEmpty(GameState.NextSpawnPointId)
                 ? defaultSpawnPointId
                 : GameState.NextSpawnPointId;

        var all = FindObjectsOfType<PlayerSpawnPoint>(true);
        PlayerSpawnPoint chosen = null;
        foreach (var sp in all)
        {
            if (sp.spawnId == id) { chosen = sp; break; }
        }
        if (!chosen && all.Length > 0) chosen = all[0];

        if (!chosen) return;

        player.transform.position = chosen.transform.position;
        var rb = player.GetComponent<Rigidbody2D>();
        if (rb) rb.linearVelocity = Vector2.zero;

        // clear the handoff
        GameState.NextSpawnPointId = null;
    }
}
