using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "BiomeManager", menuName = "ScriptableObjects/Options/BiomeManager")]
public class O_BiomeManager : ScriptableObject
{
    public List<BiomeData> Biomes;
}
