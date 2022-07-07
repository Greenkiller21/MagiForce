using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SeedManager : MonoBehaviour
{
    public O_SeedManager options;
    public float SeedX { get; private set; }
    public float SeedZ { get; private set; }

    public void GenerateSeed()
    {
        SeedX = Random.Range(options.minSeed, options.maxSeed);
        SeedZ = Random.Range(options.minSeed, options.maxSeed);
    }

    public void LoadSeed(float seedX, float seedZ)
    {
        SeedX = seedX;
        SeedZ = seedZ;
    }
}
