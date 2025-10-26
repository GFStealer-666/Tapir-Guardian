using UnityEngine;

[CreateAssetMenu(fileName = "Animal_", menuName = "TapirGuardian/Animal Data")]
public class AnimalDataSO : ScriptableObject
{
    [Header("IDs & Names")]
    public string id;                 // e.g., "tapir"
    public string commonNameTH;       // สมเสร็จ
    public string scientificName;     // Tapirus indicus
    public string family;             // Tapiridae

    [Header("Descriptions")]
    [TextArea(3, 6)] public string generalInfo;
    [TextArea(3, 6)] public string characteristics;
    public string conservationStatus;
    [TextArea(2, 5)] public string threats;
    [TextArea(2, 5)] public string diet;
    [TextArea(2, 5)] public string habitats; // simple comma-separated text

    [Header("Image")]
    public Sprite image;
    public Sprite realImage; 
}
