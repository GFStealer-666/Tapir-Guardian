[System.Serializable]
public struct ItemStack
{
    public string ItemId;
    public int Count;

    public ItemStack(string id, int count) { ItemId = id; Count = count; }
    public bool IsValid => !string.IsNullOrEmpty(ItemId) && Count > 0;
}