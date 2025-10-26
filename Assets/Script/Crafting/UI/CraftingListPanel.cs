using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CraftingListPanel : MonoBehaviour
{
    [Header("Data")]
    [SerializeField] private List<CraftingRecipeSO> recipes = new();     // fill in Inspector

    [Header("UI")]
    [SerializeField] private RectTransform contentRoot;                  // ScrollView content
    [SerializeField] private CraftingListItemUI itemPrefab;              // prefab with icon+name Button

    [Header("Detail panel to notify")]
    [SerializeField] private CraftingDetailPanel detailPanel;            // assign the right-side panel

    private readonly List<CraftingListItemUI> _spawned = new();

    private void OnEnable()
    {
        Rebuild();
        if (recipes.Count > 0 && detailPanel) detailPanel.SetRecipe(recipes[0]);
    }

    public void SetRecipes(IEnumerable<CraftingRecipeSO> list)
    {
        recipes = new List<CraftingRecipeSO>(list);
        if (isActiveAndEnabled) Rebuild();
    }

    private void Rebuild()
    {
        // clear
        for (int i = _spawned.Count - 1; i >= 0; i--)
            if (_spawned[i]) Destroy(_spawned[i].gameObject);
        _spawned.Clear();

        if (!contentRoot || !itemPrefab) return;

        // build
        foreach (var r in recipes)
        {
            var it = Instantiate(itemPrefab, contentRoot);
            Debug.Log("Adding crafting recipe to list: " + r);
            it.Bind(r);
            it.button.onClick.AddListener(() =>
            {
                if (detailPanel) detailPanel.SetRecipe(it.boundRecipe);
            });
            _spawned.Add(it);
        }
    }
}
