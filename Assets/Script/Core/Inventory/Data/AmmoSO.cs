using UnityEngine;

[CreateAssetMenu(menuName = "Items/Ammo")]
public class AmmoSO : ItemSO
{
    private void OnValidate() { Kind = ItemKind.Ammo; stackable = true; maxStack = 9999; }
}
