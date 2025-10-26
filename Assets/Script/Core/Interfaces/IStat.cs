[System.Serializable]
public enum StatType { Health, Damage, Defense, MoveSpeed };
public interface IStat // Read Only
{
    int GetStatTypeOf(StatType stat);
}

