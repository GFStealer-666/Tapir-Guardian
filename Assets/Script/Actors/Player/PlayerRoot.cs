using UnityEngine;

[DisallowMultipleComponent]
public class PlayerRoot : MonoBehaviour
{
    public static PlayerRoot Instance { get; private set; }

    void Awake()
    {
        PlayerLocator.Report(gameObject);
    }

    void OnDestroy()
    {
        if (Instance == this) PlayerLocator.Clear();
    }
}