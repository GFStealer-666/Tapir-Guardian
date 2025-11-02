using Unity.VisualScripting;
using UnityEngine;

[CreateAssetMenu(fileName = "NewCureItem", menuName = "Game/Items/Cure Status")]
public class CureStatusItemSO : ConsumableItemSO
{
    [Header("Display")]
    public string displayNameOverride; // optional; fallback to base.DisplayName
    public Sprite iconOverride;        // optional; fallback to base.Icon

    [Header("Cure Rules")]
    [Tooltip("If empty, cures all dispellable. Otherwise, cures any status that has one of these tags.")]
    public string[] cureTags;

    public override bool Use(GameObject target)
    {
        var status = target ? target.GetComponentInChildren<StatusComponent>() : null;
        if (!status) return false;
        Debug.Log($"status are not null");
        var health = target ? target.GetComponentInChildren<HealthComponent>() : null;
        if (health)
        {
            Debug.Log($"healing :{healAmount} ");
            health.Heal(healAmount);
        }

        if (cureTags != null && cureTags.Length > 0)
        {
            Debug.Log($"Dispelling");
            int total = 0;
            foreach (var t in cureTags) total += status.RemoveByTag(t);
            return total > 0;
        }
        else
        {
            return status.RemoveDispellable() > 0;
        }

        
    }

    // Optional: keep ItemSO’s DisplayName/Icon in sync with overrides
    private new void OnValidate()
    {
        base.OnValidate(); // sets Kind/stack rules
        if (!string.IsNullOrEmpty(displayNameOverride)) DisplayName = displayNameOverride;
        if (iconOverride) Icon = iconOverride;
    }
}

// Specific preset that cures "Poison"
[CreateAssetMenu(fileName = "NewAntidote", menuName = "Game/Items/Antidote (Cure Poison)")]
public class AntidoteItemSO : CureStatusItemSO { }
