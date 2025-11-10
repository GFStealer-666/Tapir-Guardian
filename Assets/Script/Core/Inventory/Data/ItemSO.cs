using UnityEngine;

public enum ItemKind { Misc, WeaponMelee, WeaponGun, Ammo, Consumable, CraftingMaterial }
public enum EquipSlot { MainHand, SideHand }

public abstract class ItemSO : ScriptableObject
{
    public string Id;
    public string DisplayName;
    public string Description;
    public Sprite Icon;
    public ItemKind Kind;

    [Header("Stacking")]
    public bool stackable = true;  // weapons typically false
    public int  maxStack  = 999;   // if stackable; 1 for non-stackable

    public virtual bool CanEquip(EquipSlot slot) => false;
}
