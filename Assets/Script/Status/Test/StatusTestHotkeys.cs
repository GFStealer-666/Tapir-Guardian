// Assets/Scripts/Status/Debug/StatusTestHotkeys.cs
using UnityEngine;
public class StatusTestHotkeys : MonoBehaviour
{
    public StatusComponent target;
    public StatusSO poison;
    public StatusSO slow;

    void Update()
    {
        target.Apply(poison);
        target.Apply(slow);

    }
    
    public void ClearAllStatuses()
    {
        target.RemoveDispellable();
    }
}
