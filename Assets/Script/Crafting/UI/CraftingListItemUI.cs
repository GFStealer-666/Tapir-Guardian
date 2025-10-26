using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CraftingListItemUI : MonoBehaviour
{
    [Header("UI")]
    public Button button;
    public Image icon;
    public TMP_Text nameText;

    [HideInInspector] public CraftingRecipeSO boundRecipe;

    public void Bind(CraftingRecipeSO recipe)
    {
        boundRecipe = recipe;
        if (icon)     icon.sprite = recipe && recipe.outputItem ? recipe.outputItem.Icon : null;
        if (nameText) nameText.text = recipe && recipe.outputItem ? recipe.outputItem.DisplayName : "-";
    }
}
