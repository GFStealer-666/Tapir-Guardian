using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary>One ingredient line: icon + "name  have/need"</summary>
public class IngredientRowUI : MonoBehaviour
{
    public Image icon;
    public TMP_Text lineText;

    /// <param name="displayName">e.g., "ว่านชงชะงั้ง"</param>
    /// <param name="have">player has</param>
    /// <param name="need">required</param>
    public void Bind(Sprite sp, string displayName, int have, int need)
    {
        if (icon) icon.sprite = sp;
        if (lineText)
        {
            // Example: "ว่านชงชะงั้ง 0 / 1"
            lineText.text = $"{displayName} {Mathf.Max(0, have)} / {Mathf.Max(1, need)}";
        }
    }
}
