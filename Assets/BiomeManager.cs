using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BiomeManager : MonoBehaviour
{
    public O_BiomeManager options;
    public List<BiomeData> Biomes => options.Biomes;
}
