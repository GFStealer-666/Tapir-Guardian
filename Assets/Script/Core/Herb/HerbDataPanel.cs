using UnityEngine;

public class HerbDataPanel : MonoBehaviour
{
    [SerializeField] private HerbDataView view;

    public void Show(HerbDataSO h)
    {
        if (h != null)
            view.Bind(h);
        view.Show(true);
    }

    public void Hide() => view.Show(false);
}
