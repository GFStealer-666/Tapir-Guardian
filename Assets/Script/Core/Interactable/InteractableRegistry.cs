using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

using Debug = UnityEngine.Debug;
public static class InteractableRegistry
{
    private static readonly List<IInteractable> _all = new List<IInteractable>();
    public static IReadOnlyList<IInteractable> All => _all;

    public static void Register(IInteractable i)
    {
        Debug.Log($"Registering interactable: {i}");
        if (i != null && !_all.Contains(i))
            _all.Add(i);
    }

    public static void Unregister(IInteractable i)
    {
        if (i != null)
            _all.Remove(i);
    }
}
