using UnityEngine;
using System;

public static class PlayerLocator
{
    public static GameObject Player { get; private set; }
    public static event Action<GameObject> OnPlayerSet;

    public static void Report(GameObject playerGO)
    {
        Player = playerGO;
        OnPlayerSet?.Invoke(Player);
        Debug.Log($"PlayerLocator: Player reported: {playerGO.name}");
    }

    public static void Clear()
    {
        Player = null;
        OnPlayerSet?.Invoke(null);
    }

    public static bool TryGet(out GameObject p)
    {
        p = Player;
        return p != null;
    }
}
