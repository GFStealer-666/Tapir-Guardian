using UnityEngine;
using UnityEngine.SceneManagement;
public class SimpleSceneLoader : MonoBehaviour
{
    [SerializeField] private string sceneName; // Set this in Inspector

    private void OnEnable()
    {
        if (!string.IsNullOrEmpty(sceneName))
        {
            SceneManager.LoadScene(sceneName);
        }
        else
        {
            Debug.LogWarning("No scene name assigned in LoadSceneOnEnable!");
        }
    }
}
