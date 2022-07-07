using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "BiomeData", menuName = "ScriptableObjects/BiomeData")]
public class BiomeData : ScriptableObject
{
    public string BiomeName;
    public float SpawnProbability;
    public float Amplitude;
    public float Frequency;
    public Color Color;
}
