using UnityEngine;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(Collider2D))]
public class SceneLoadTrigger2D : MonoBehaviour
{
    [Header("Load")]
    public string sceneToLoad;
    public LoadSceneMode loadMode = LoadSceneMode.Single; // usually Single
    public string nextSceneSpawnId = "Spawn_Default";
    [SerializeField] private PlayerControlLock playerlock;

    [Header("Filter")]
    public string playerTag = "Player";

    [Header("Optional fade (assign a UIFader in the scene/persistent UI)")]
    public UIFader fader;
    public float fadeOutTime = 0.25f;

    Collider2D _col;

    void Reset()
    {
        _col = GetComponent<Collider2D>();
        _col.isTrigger = true;
    }

    void Awake()
    {
        _col = GetComponent<Collider2D>();
        if (_col) _col.isTrigger = true;
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag(playerTag)) return;
        if (string.IsNullOrEmpty(sceneToLoad)) return;
        playerlock.InputBlocked = true;
        // set spawn handoff for the next scene
        GameState.NextSpawnPointId = nextSceneSpawnId;

        if (fader)
        {
            // run fade + load
            StartCoroutine(LoadWithFade());
        }
        else
        {
            SceneManager.LoadScene(sceneToLoad, loadMode);
        }
    }

    System.Collections.IEnumerator LoadWithFade()
    {
        if (fader) yield return fader.FadeOutCoroutine(fadeOutTime);
        SceneManager.LoadScene(sceneToLoad, loadMode);
    }
}
