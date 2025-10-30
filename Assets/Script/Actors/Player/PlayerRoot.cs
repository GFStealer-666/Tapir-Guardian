using UnityEngine;

[DisallowMultipleComponent]
public class PlayerRoot : MonoBehaviour
{
    public static PlayerRoot Instance { get; private set; }

    void Awake()
    {
        if (Instance && Instance != this)
        {
            Destroy(gameObject);     // avoid duplicate players
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject); // keeps this GO + all children
        PlayerLocator.Report(gameObject);
    }

    void OnDestroy()
    {
        if (Instance == this) PlayerLocator.Clear();
    }
}