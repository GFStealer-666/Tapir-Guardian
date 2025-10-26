// using UnityEngine;

// public class DebugWeaponSwapper : MonoBehaviour
// {
//     [Header("Refs")]
//     [SerializeField] private EquipmentComponent equipment;
//     [SerializeField] private EquipmentUI equipmentUI;

//     [Header("Test Guns")]
//     [SerializeField] private WeaponGunSO pistol9mm;
//     [SerializeField] private WeaponGunSO shotgun;
//     // you can add more guns here or leave null if you don't have them yet

//     private void Awake()
//     {
//         // try auto-find if not wired
//         if (!equipment)
//             equipment = FindObjectOfType<EquipmentComponent>();
//         if (!equipmentUI)
//             equipmentUI = FindObjectOfType<EquipmentUI>();
//     }

//     private void Update()
//     {
//         // Key 1: equip pistol9mm into MainHand
//         if (Input.GetKeyDown(KeyCode.Alpha1))
//         {
//             if (pistol9mm != null)
//             {
//                 equipment.EquipMainGun(pistol9mm);
//                 equipmentUI?.Refresh();
//                 Debug.Log("[DebugWeaponSwapper] Equipped pistol9mm to MainHand.");
//             }
//             else
//             {
//                 Debug.LogWarning("[DebugWeaponSwapper] pistol9mm not assigned.");
//             }
//         }

//         // Key 2: equip shotgun into MainHand
//         if (Input.GetKeyDown(KeyCode.Alpha2))
//         {
//             if (shotgun != null)
//             {
//                 equipment.EquipMainGun(shotgun);
//                 equipmentUI?.Refresh();
//                 Debug.Log("[DebugWeaponSwapper] Equipped shotgun to MainHand.");
//             }
//             else
//             {
//                 Debug.LogWarning("[DebugWeaponSwapper] shotgun not assigned.");
//             }
//         }

//         // Key 3: unequip gun (force melee-only mode)
//         if (Input.GetKeyDown(KeyCode.Alpha3))
//         {
//             equipment.EquipMainGun(null);
//             equipmentUI?.Refresh();
//             Debug.Log("[DebugWeaponSwapper] MainHand cleared. Player will fallback to SideHand melee.");
//         }
//     }
// }
