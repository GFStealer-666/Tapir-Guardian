using System;
using System.Collections.Generic;
using UnityEngine;

[DisallowMultipleComponent]
public class InventoryComponent : MonoBehaviour,IItemUseTargetProvider
{
    [Header("Owner")]
    [SerializeField] private GameObject owner; 

    private void Awake()
    {
        if (!owner) owner = gameObject;
    }

    public GameObject GetUseTarget() => owner ? owner : gameObject;
    [SerializeField] private ItemDatabaseSO database;
    [SerializeField] private List<ItemStack> stacks = new();
    public IReadOnlyList<ItemStack> Stacks => stacks;
    public event Action OnItemChanged;
    
    public bool Add(string itemId, int amount = 1)
    {
        if (database == null || string.IsNullOrEmpty(itemId) || amount <= 0) return false;
        var so = database.Get(itemId);
        if (!so) return false;

        if (so.stackable)
        {
            int i = stacks.FindIndex(s => s.ItemId == itemId);
            if (i < 0) stacks.Add(new ItemStack(itemId, amount));
            else
            {
                var s = stacks[i];
                s.Count += amount;

                // optional: enforce maxStack by splitting into multiple stacks
                if (so.maxStack > 0 && s.Count > so.maxStack)
                {
                    int remain = s.Count - so.maxStack;
                    s.Count = so.maxStack;
                    stacks[i] = s;
                    // create more stacks for the remainder
                    while (remain > 0)
                    {
                        int take = Mathf.Min(remain, so.maxStack);
                        stacks.Add(new ItemStack(itemId, take));
                        remain -= take;
                    }
                }
                else stacks[i] = s;
            }
        }
        else
        {
            // non-stackable => add 'amount' entries of 1
            for (int n = 0; n < amount; n++)
                stacks.Add(new ItemStack(itemId, 1));
        }
        OnItemChanged?.Invoke();
        return true;
    }

    public bool Consume(string itemId, int amount = 1)
    {
        if (database == null || string.IsNullOrEmpty(itemId) || amount <= 0) return false;
        var so = database.Get(itemId);
        if (!so) return false;
        Debug.Log($"Consuming {itemId}");
        if (so.stackable)
        {
            int need = amount;
            for (int i = 0; i < stacks.Count && need > 0; i++)
            {
                if (stacks[i].ItemId != itemId) continue;
                var s = stacks[i];
                int take = Mathf.Min(s.Count, need);
                s.Count -= take;
                need -= take;

                if (s.Count <= 0) { stacks.RemoveAt(i); i--; }
                else stacks[i] = s;
            }
            bool ok = need == 0;
            if (ok) OnItemChanged?.Invoke();
            return ok;
        }
        else
        {
            // non-stackable: need that many copies present
            int have = 0;
            foreach (var s in stacks) if (s.ItemId == itemId) have++;
            if (have < amount) return false;

            for (int i = stacks.Count - 1; i >= 0 && amount > 0; i--)
                if (stacks[i].ItemId == itemId) { stacks.RemoveAt(i); amount--; }

            OnItemChanged?.Invoke();
            return true;
        }
    }

    public int GetCount(string itemId)
    {
        int total = 0;
        foreach (var s in stacks) if (s.ItemId == itemId) total += s.Count;
        return total;
    }

    public bool Has(string itemId, int atLeast = 1) => GetCount(itemId) >= atLeast;

    public ItemSO Resolve(string itemId) => database ? database.Get(itemId) : null;
}


public interface IItemUseTargetProvider
{
    GameObject GetUseTarget();
}
