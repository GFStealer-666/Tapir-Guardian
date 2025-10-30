using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerSceneSpawner : MonoBehaviour
{
    void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
        SnapIfPossible(); // handles entering Play Mode in an already-loaded scene
    }

    void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode) => SnapIfPossible();

    void SnapIfPossible()
    {
        if (!PlayerLocator.TryGet(out var player)) return;

        var spawn = FindAnyObjectByType<PlayerSpawnPoint>();
        if (!spawn) return;

        // Move player (and optionally zero velocity if using Rigidbody2D)
        player.transform.position = spawn.transform.position;

        var rb = player.GetComponent<Rigidbody2D>();
        if (rb) rb.linearVelocity = Vector2.zero;
    }
}
