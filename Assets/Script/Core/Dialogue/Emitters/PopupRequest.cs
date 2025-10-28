using UnityEngine;

public readonly struct PopupRequest
{
    public readonly string Text;
    public readonly float Duration;
    public readonly PopupCategory Category;
    public readonly int Priority; // bigger = more important
    public readonly bool IsPersistent; // <— new flag

    public PopupRequest(string text, float duration, PopupCategory category, int priority = 0, bool isPersistent = false)
    {
        Text = text;
        Duration = Mathf.Max(0.1f, duration);
        Category = category;
        Priority = priority;
        IsPersistent = isPersistent;
    }

}
