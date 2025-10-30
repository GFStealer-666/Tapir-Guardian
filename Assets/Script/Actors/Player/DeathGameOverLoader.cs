using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

[DisallowMultipleComponent]
public class DeathGameOverLoader : MonoBehaviour
{
    [Header("Refs")]
    [SerializeField] private HealthComponent health;     // player HealthComponent
    [SerializeField] private PlayerBrain playerBrain;    // to lock input

    [Header("Game Over")]
    [SerializeField] private string gameOverSceneName = "GameOver"; // add to Build Settings!
    [SerializeField, Min(0f)] private float delayBeforePopup = 0.6f;
    [SerializeField] private bool loadAdditive = true;   // popup-style overlay
    [SerializeField] private bool pauseTimeOnPopup = true;

    private bool _fired;

    private void Awake()
    {
        if (!health)      health      = GetComponentInChildren<HealthComponent>();
        if (!playerBrain) playerBrain = GetComponentInChildren<PlayerBrain>();
    }

    private void OnEnable()
    {
        if (health != null)
            health.OnDied += HandleDied;
    }

    private void OnDisable()
    {
        if (health != null)
            health.OnDied -= HandleDied;
    }

    private void HandleDied()
    {
        if (_fired) return;
        _fired = true;
        // lock player input immediately
        playerBrain?.SetInputBlocked(true);
        StartCoroutine(ShowGameOverRoutine());
    }

    private IEnumerator ShowGameOverRoutine()
    {
        if (delayBeforePopup > 0f)
            yield return new WaitForSeconds(delayBeforePopup);

        if (loadAdditive)
        {
            var op = SceneManager.LoadSceneAsync(gameOverSceneName, LoadSceneMode.Additive);
            while (!op.isDone) yield return null;

            if (pauseTimeOnPopup) Time.timeScale = 0f; // freeze world after UI is up
        }
        else
        {
            // hard switch to a dedicated GameOver scene (no overlay)
            if (pauseTimeOnPopup) Time.timeScale = 1f; // make sure timescale is sane for new scene
            SceneManager.LoadScene(gameOverSceneName, LoadSceneMode.Single);
            this.gameObject.SetActive(false); // disable self to avoid weirdness
        }
    }
}
