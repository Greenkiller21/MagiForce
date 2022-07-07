using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(SeedManager))]
[RequireComponent(typeof(BiomeManager))]
public class TerrainController : MonoBehaviour
{
    public GameObject ChunkPrefab;
    public int ChunkSizeX = 50;
    public int ChunkSizeZ = 50;
    public int ChunkXCount = 4;
    public int ChunkZCount = 3;

    private SeedManager seedManager;
    private BiomeManager biomeManager;

    private int ReadyForSmoothTerrainCount;

    public void AddToReadyForSmoothTerrain()
    {
        ReadyForSmoothTerrainCount++;

        if (ReadyForSmoothTerrainCount == gameObject.GetComponentsInChildren<Chunk>(false).Length)
        {
            ReadyForSmoothTerrainCount = 0;
            SmoothTerrain();
        }
    }

    private void Awake()
    {
        seedManager = GetComponent<SeedManager>();
        biomeManager = GetComponent<BiomeManager>();
    }

    private void Start()
    {
        CreateTerrain();
    }

    void CreateTerrain()
    {
        for (int x = 0; x < ChunkXCount; x++)
        {
            for (int z = 0; z < ChunkZCount; z++)
            {
                CreateChunk(x * ChunkSizeX, z * ChunkSizeZ);
            }
        }
    }

    private void CreateChunk(float worldX, float worldZ)
    {
        var gameObject = Instantiate(ChunkPrefab, transform);
        var chunk = gameObject.GetComponent<Chunk>();

        chunk.SizeX = ChunkSizeX;
        chunk.SizeZ = ChunkSizeZ;
        chunk.transform.position = new Vector3(worldX, 0, worldZ);

        var chunkIndex = ChunkUtils.GetChunkIndex(worldX, worldZ, ChunkSizeX, ChunkSizeZ);
        gameObject.name = string.Format(ChunkUtils.ChunkNameFormat, chunkIndex.Item1, chunkIndex.Item2);
    }

    private void SmoothTerrain()
    {
        int smoothingNeighbors = 3;

        var chunks = gameObject.GetComponentsInChildren<Chunk>(false);
        foreach (Chunk chunk in chunks)
        {
            var smoothedHeightMap = new float[(chunk.SizeX + 1) * (chunk.SizeZ + 1)];
            for (int i = 0, z = 0; z <= chunk.SizeZ; z++)
            {
                for (int x = 0; x <= chunk.SizeX; x++)
                {
                    double heightSum = 0f;
                    int count = 0;
                    for (int zDelta = -smoothingNeighbors; zDelta <= smoothingNeighbors; zDelta++)
                    {
                        var currentZ = z + zDelta;

                        for (int xDelta = -smoothingNeighbors; xDelta <= smoothingNeighbors; xDelta++)
                        {
                            var currentX = x + xDelta;
                            count++;
                            if (currentX < 0 || currentX > chunk.SizeX || currentZ < 0 || currentZ > chunk.SizeZ)
                            {
                                var worldX = chunk.transform.position.x + currentX;
                                var worldZ = chunk.transform.position.z + currentZ;

                                var adjacentChunk = ChunkUtils.GetChunk(worldX, worldZ, chunk.SizeX, chunk.SizeZ);

                                if (adjacentChunk != null)
                                {
                                    var coords = adjacentChunk.WorldToLocal(worldX, worldZ);
                                    heightSum += adjacentChunk.heightMap[(int)coords.Item2 * ((int)adjacentChunk.SizeX + 1) + (int)coords.Item1];
                                }
                                else
                                {
                                    heightSum += ChunkUtils.GetHeight(worldX, worldZ, biomeManager.Biomes, seedManager);
                                }
                            }
                            else
                            {
                                heightSum += chunk.heightMap[currentZ * (chunk.SizeX + 1) + currentX];
                            }
                        }
                    }
                    smoothedHeightMap[i] = (float)(heightSum / count);
                    i++;
                }
            }

            for (int i = 0, z = 0; z <= chunk.SizeZ; z++)
            {
                for (int x = 0; x <= chunk.SizeX; x++)
                {
                    chunk.heightMap[i] = Mathf.Lerp(chunk.heightMap[i], smoothedHeightMap[i], 1);
                    i++;
                }
            }

            chunk.UpdateTerrain();
        }

        foreach (Chunk chunk in chunks)
        {

        }
    }
}
