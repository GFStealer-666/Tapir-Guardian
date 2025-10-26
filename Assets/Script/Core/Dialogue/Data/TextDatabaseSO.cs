using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "TextDatabase", menuName = "Popups/Text Database")]
public class TextDatabaseSO : ScriptableObject
{
    [Tooltip("Register all sequences for this scene here (player, notifications, enemy, random).")]
    public List<TextSequenceSO> sequences = new();

    public TextSequenceSO FindById(string id) => sequences.Find(s => s.sequenceId == id);
    public List<TextSequenceSO> ByCategory(PopupCategory cat) => sequences.FindAll(s => s.category == cat);
}
