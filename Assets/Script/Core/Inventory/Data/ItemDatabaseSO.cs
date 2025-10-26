using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Items/Item Database")]
public class ItemDatabaseSO : ScriptableObject
{
    public List<ItemSO> items = new();

    private Dictionary<string, ItemSO> map;

    private void OnEnable()
    {
        map = new Dictionary<string, ItemSO>(items.Count);
        foreach (var i in items)
            if (i && !string.IsNullOrEmpty(i.Id))
                map[i.Id] = i;
    }

    public ItemSO Get(string id)
    {
        if (string.IsNullOrEmpty(id) || map == null) return null;
        return map.TryGetValue(id, out var so) ? so : null;
    }
}