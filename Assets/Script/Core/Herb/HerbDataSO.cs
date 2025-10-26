using UnityEngine;

[CreateAssetMenu(fileName = "Herb_", menuName = "TapirGuardian/Herb Data")]
public class HerbDataSO : ScriptableObject
{
    [Header("IDs & Names")]
    public string id;                 // unique, e.g., "k_roscoeana"
    public string nameTH;             // ว่านธรณีเย็น
    public string nameEN;             // optional

    [Header("Science")]
    public string scientificName;     // Kaempferia roscoeana Wall.
    public string family;             // optional

    [Header("Descriptions")]
    [TextArea(3, 6)] public string propertiesText;
    [TextArea(3, 6)] public string usageText;      

    [TextArea(5,10)] public string finaldescriptionText; 

    [Header("Image")]
    public Sprite image;
}
