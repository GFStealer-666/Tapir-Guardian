using System.Collections.Generic;
using UnityEngine;

public enum PopupCategory { PlayerDialogue, Notification, EnemyDialogue, Random }

[CreateAssetMenu(fileName = "TextSequence", menuName = "Popups/Text Sequence")]
public class TextSequenceSO : ScriptableObject
{
    public string sequenceId;
    public PopupCategory category = PopupCategory.PlayerDialogue;
    public bool loop = false;
    public List<TextLineSO> lines = new();
}
