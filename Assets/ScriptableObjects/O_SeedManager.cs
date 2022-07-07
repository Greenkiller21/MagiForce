using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "SeedManager", menuName = "ScriptableObjects/Options/SeedManager")]
public class O_SeedManager : ScriptableObject
{
    public float minSeed = 1000000f;
    public float maxSeed = 9999999f;
}
