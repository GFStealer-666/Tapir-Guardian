using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "AnimalDatabase", menuName = "TapirGuardian/Animal Database")]
public class AnimalDatabaseSO : ScriptableObject
{
    public List<AnimalDataSO> animals = new();
}
