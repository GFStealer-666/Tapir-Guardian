using System;
public interface ILevel
{
    int Level { get; }
    int CurrentExp { get; }
    int ExpToNext { get;}
    void AddExp(int xp);
    event Action<int> OnLevelChanged;
}
