using UnityEngine;
using System.Linq;
using System.Collections.Generic;
public static class InterfaceLookup
{
    public static T GetInterfaceInChildren<T>(this Component self, bool includeInactive = true) where T : class
    {
        foreach (var mb in self.GetComponentsInChildren<MonoBehaviour>(includeInactive))
        {
            if (mb is T t) return t;
        }
        return null;
    }
    public static T GetFirstInterfaceInChildren<T>(Transform root, bool includeInactive = true) where T : class
    {
        foreach (var mb in root.GetComponentsInChildren<MonoBehaviour>(includeInactive))
        {
            if (mb is T t) return t;
        }
        return null;
    }
    public static T[] GetInterfacesInChildren<T>(Transform root, bool includeInactive = true) where T : class
    {
        List<T> mbList = new List<T>();
        foreach (var mb in root.GetComponentsInChildren<MonoBehaviour>(includeInactive))
        {
            if (mb is T t) mbList.Add(t);
        }
        return mbList.ToArray();
    }
}
