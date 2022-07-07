using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[RequireComponent(typeof(MeshFilter))]
[RequireComponent(typeof(MeshCollider))]
public class TerrainMesh : MonoBehaviour
{
    private Mesh mesh;

    private Vector3[] vertices;
    private int[] triangles;
    private Color[] colors;

    public int SizeX = 20;
    public int SizeZ = 20;

    public float Frequency = 15f;
    public float Amplitude = 8f;

    public float SeedX;
    public float SeedZ;

    public List<BiomeData> Biomes;

    void Start()
    {
        mesh = new Mesh();
        GetComponent<MeshFilter>().mesh = mesh;
        CreateTerrain();
        transform.position = new Vector3(transform.position.x - SizeX / 2.0f, transform.position.y, transform.position.z - SizeZ / 2.0f);
        UpdateTerrain();
    }

    void Update()
    {
        //UpdateTerrain();
    }

    void CreateTerrain()
    {
        if (Biomes.Count == 0)
        {
            Debug.LogError("No biomes !");
            return;
        }

        vertices = new Vector3[(SizeX + 1) * (SizeZ + 1)];
        colors = new Color[vertices.Length];

        for (int i = 0, z = 0; z <= SizeZ; z++)
        {
            for (int x = 0; x <= SizeX; x++)
            {
                float biomeHeatMap = Mathf.PerlinNoise((SeedX + x + transform.position.x) / (Frequency * 2f), (SeedZ + z + transform.position.z) / (Frequency * 2f));
                float y = Mathf.PerlinNoise((SeedX + x + transform.position.x) / (Frequency * 2f), (SeedZ + z + transform.position.z) / (Frequency * 2f)) * (Amplitude * 0.75f);

                float totalProbability = Biomes.Sum(biome => biome.SpawnProbability);

                float currentProbability = 0f;
                foreach (BiomeData biome in Biomes)
                {
                    currentProbability += biome.SpawnProbability / totalProbability;

                    if (biomeHeatMap <= currentProbability)
                    {
                        y += Mathf.PerlinNoise((SeedX + x + transform.position.x) / (Frequency * biome.Frequency), (SeedZ + z + transform.position.z) / (Frequency * biome.Frequency)) * (Amplitude * biome.Amplitude);
                        break;
                    }
                }

                //y += Mathf.PerlinNoise((SeedX + x + transform.position.x) / (Frequency * 4f), (SeedZ + z + transform.position.z) / (Frequency * 4f)) * (Amplitude * 4f);

                vertices[i] = new Vector3(x, y, z);
                i++;
            }
        }

        triangles = new int[SizeX * SizeZ * 6];

        int vert = 0;
        int tris = 0;
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

    private void UpdateTerrain()
    {
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
