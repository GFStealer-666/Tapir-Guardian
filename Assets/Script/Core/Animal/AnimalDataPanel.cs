using UnityEngine;

public class AnimalDataPanel : MonoBehaviour
{
    [SerializeField] private AnimalDataView view;

    public void Show(AnimalDataSO a)
    {
        view.Bind(a);
        view.Show(true);
    }

    public void Hide() => view.Show(false);
}
