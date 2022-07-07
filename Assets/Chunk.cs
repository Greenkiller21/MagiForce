using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[RequireComponent(typeof(MeshFilter))]
[RequireComponent(typeof(MeshCollider))]
public class Chunk : MonoBehaviour
{
    private Mesh mesh;
    private int[] triangles;
    private Color[] colors;

    public float[] heightMap;

    public int SizeX = 20;
    public int SizeZ = 20;

    private SeedManager seedManager;
    private BiomeManager biomeManager;

    private void Awake()
    {
        mesh = new Mesh();
        GetComponent<MeshFilter>().mesh = mesh;

        seedManager = GetComponentInParent<SeedManager>();
        seedManager.GenerateSeed();

        biomeManager = GetComponentInParent<BiomeManager>();
    }

    private void Start()
    {
        CreateTerrain();
        UpdateTerrain();

        GetComponentInParent<TerrainController>().AddToReadyForSmoothTerrain();
    }

    private void CreateTerrain()
    {
        if (biomeManager.Biomes.Count == 0)
        {
            Debug.LogError("No biomes !");
            return;
        }

        //colors = new Color[vertices.Length];
        heightMap = new float[(SizeX + 1) * (SizeZ + 1)];

        for (int i = 0, z = 0; z <= SizeZ; z++)
        {
            for (int x = 0; x <= SizeX; x++)
            {
                //float y = Mathf.PerlinNoise((SeedX + worldX) / (Frequency * 2f), (SeedZ + worldZ) / (Frequency * 2f)) * (Amplitude * 0.75f);

                //var biome = GetBiome(worldX, worldZ);
                float y = GetHeight(x, z);

                //y += Mathf.PerlinNoise((SeedX + x + transform.position.x) / (Frequency * 4f), (SeedZ + z + transform.position.z) / (Frequency * 4f)) * (Amplitude * 4f);

                heightMap[i] = y;
                i++;
            }
        }

        int vert = 0;
        int tris = 0;
        triangles = new int[SizeX * SizeZ * 6];
        for (int z = 0; z < SizeZ; z++)
        {
            for (int x = 0; x < SizeX; x++)
            {
                triangles[tris++] = vert + 0;
                triangles[tris++] = vert + SizeX + 1;
                triangles[tris++] = vert + 1;
                triangles[tris++] = vert + 1;
                triangles[tris++] = vert + SizeX + 1;
                triangles[tris++] = vert + SizeX + 2;

                vert++;
            }
            vert++;
        }
    }

    public BiomeData GetBiome(float localX, float localZ)
    {
        return ChunkUtils.GetBiome(transform.position.x + localX, transform.position.z + localZ, biomeManager.Biomes, seedManager);
    }

    public float GetHeight(float localX, float localZ)
    {
        return ChunkUtils.GetHeight(transform.position.x + localX, transform.position.z + localZ, biomeManager.Biomes, seedManager);
    }

    public (float, float) WorldToLocal(float worldX, float worldZ)
    {
        return (worldX - transform.position.x, worldZ - transform.position.z);
    }

    public void UpdateTerrain()
    {
        var vertices = new Vector3[(SizeX + 1) * (SizeZ + 1)];
        for (int i = 0, z = 0; z <= SizeZ; z++)
        {
            for (int x = 0; x <= SizeX; x++)
            {
                vertices[i] = new Vector3(x, heightMap[i], z);
                i++;
            }
        }

        mesh.Clear();

        mesh.vertices = vertices;
        mesh.triangles = triangles;
        mesh.colors = colors;

        mesh.RecalculateNormals();

        mesh.RecalculateBounds();
        MeshCollider meshCollider = gameObject.GetComponent<MeshCollider>();
        meshCollider.sharedMesh = mesh;
    }
}

public static class ChunkUtils
{
    public const string ChunkNameFormat = "Chunk {0},{1}";

    public static BiomeData GetBiome(float worldX, float worldZ, List<BiomeData> biomes, SeedManager seedManager)
    {
        float biomeHeightMap = Mathf.PerlinNoise((seedManager.SeedX + worldX) / 20f, (seedManager.SeedZ + worldZ) / 20f);
        biomeHeightMap = Mathf.Clamp01(biomeHeightMap);

        float totalProbability = biomes.Sum(biome => biome.SpawnProbability);

        float currentProbability = 0f;
        foreach (BiomeData biome in biomes)
        {
            currentProbability += biome.SpawnProbability / totalProbability;

            if (biomeHeightMap <= currentProbability)
            {
                return biome;
            }
        }

        Debug.LogError($"Biome for coordinate {worldX} {worldZ} not found (biomeHM : {biomeHeightMap}) !");
        return null;
    }

    public static float GetHeight(float worldX, float worldZ, List<BiomeData> biomes, SeedManager seedManager)
    {
        var biome = GetBiome(worldX, worldZ, biomes, seedManager);
        return Mathf.PerlinNoise((seedManager.SeedX + worldX) * biome.Frequency, (seedManager.SeedZ + worldZ) * biome.Frequency) * biome.Amplitude;
    }

    public static (int, int) GetChunkIndex(float worldX, float worldZ, float chunkSizeX, float chunkSizeZ)
    {
        return (Mathf.FloorToInt(worldX / chunkSizeX), Mathf.FloorToInt(worldZ / chunkSizeZ));
    }

    public static (int, int) GetChunkIndex(Chunk chunk)
    {
        return (Mathf.FloorToInt(chunk.transform.position.x / chunk.SizeX), Mathf.FloorToInt(chunk.transform.position.z / chunk.SizeZ));
    }

    public static Chunk GetChunk(float worldX, float worldZ, float chunkSizeX, float chunkSizeZ)
    {
        var chunkIndex = GetChunkIndex(worldX, worldZ, chunkSizeX, chunkSizeZ);
        var go = GameObject.Find(string.Format(ChunkNameFormat, chunkIndex.Item1, chunkIndex.Item2));
        return go?.GetComponent<Chunk>();
    }
}
