using UnityEngine;

[CreateAssetMenu(menuName = "Crafting/Recipe", fileName = "NewRecipe")]
public class CraftingRecipeSO : ScriptableObject
{
    [Header("Inputs")]
    public RecipeIngredient[] inputs;

    [Header("Output")]
    public ItemSO outputItem;
    public int outputCount = 1;

    [Header("Timing")]
    [Min(0f)] public float craftSeconds = 2f; // 0 = instant

    private void OnValidate()
    {
        if (inputs != null)
            for (int i = 0; i < inputs.Length; i++)
                inputs[i].OnValidate();

        if (outputCount < 1) outputCount = 1;
    }
}
