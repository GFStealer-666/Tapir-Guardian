using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

[DisallowMultipleComponent]
public class GameOverUI : MonoBehaviour
{
    [Header("Behavior")]
    [SerializeField] private bool restartLevelOnClick = true;
    [SerializeField] private string mainMenuScene = "MainMenu";
    [SerializeField] private float clickDelay = 0.3f;
    [SerializeField] private InputActionReference mouse0;

    private bool _ready;

    private void Start()
    {
        mouse0?.action.Enable();
        Invoke(nameof(EnableClick), clickDelay);
    }

    private void EnableClick() => _ready = true;

    private void Update()
    {
        if (!_ready) return;


        if (mouse0 && mouse0.action.WasPerformedThisFrame())
        {
            Debug.Log("GameOverUI: Click detected, loading scene.");
            Time.timeScale = 1f;
            SceneManager.LoadScene(mainMenuScene, LoadSceneMode.Single);
        
        }
    }
}
