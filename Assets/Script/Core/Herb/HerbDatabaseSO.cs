using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "HerbDatabase", menuName = "TapirGuardian/Herb Database")]
public class HerbDatabaseSO : ScriptableObject
{
    public List<HerbDataSO> herbs = new();
}
