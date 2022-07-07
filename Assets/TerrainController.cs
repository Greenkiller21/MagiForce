using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TerrainController : MonoBehaviour
{
    public GameObject TerrainMeshPrefab;
    public int ChunkSize = 50;
    public int ChunkXCount = 4;
    public int ChunkZCount = 3;

    private float seedX;
    private float seedZ;

    void Start()
    {
        seedX = Random.Range(0f, 999999f);
        seedZ = Random.Range(0f, 999999f);

        CreateTerrain();
    }

    void CreateTerrain()
    {
        var basePointX = (1 - (ChunkXCount + 1) / 2) * ChunkSize;
        var basePointZ = (1 - (ChunkZCount + 1) / 2) * ChunkSize;

        for (int x = 0; x < ChunkXCount; x++)
        {
            for (int z = 0; z < ChunkZCount; z++)
            {
                CreateTerrainMesh(basePointX + x * ChunkSize, basePointZ + z * ChunkSize);
            }
        }
    }

    void CreateTerrainMesh(int x, int z)
    {
        var gameObject = Instantiate(TerrainMeshPrefab);
        var terrainMesh = gameObject.GetComponent<TerrainMesh>();
        terrainMesh.SeedX = seedX;
        terrainMesh.SeedZ = seedZ;

        terrainMesh.SizeX = terrainMesh.SizeZ = ChunkSize;

        terrainMesh.transform.position = new Vector3(transform.position.x + x, transform.position.y, transform.position.z + z);
        terrainMesh.Frequency = 15f;
        terrainMesh.Amplitude = 8f;

        gameObject.transform.SetParent(this.transform);
    }
}
