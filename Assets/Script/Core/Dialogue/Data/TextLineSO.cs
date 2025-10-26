#if UNITY_EDITOR
using TMPro;
using UnityEditor;
#endif
using UnityEngine;

[CreateAssetMenu(fileName = "TextLine", menuName = "Popups/Text Line")]
public class TextLineSO : ScriptableObject
{
    [TextArea(1, 4)] public string text;
    [Min(0.1f)] public float duration = 5f;
}
