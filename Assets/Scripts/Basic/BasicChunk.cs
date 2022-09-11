using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BasicChunk
{
    private GameObject chunkObject;
    private MeshRenderer meshRenderer;
    private MeshFilter meshFilter;
    private int vertexIndex = 0;
    private List<Vector3> vertices = new List<Vector3>();
    private List<int> triangles = new List<int>();
    private List<Vector2> uvs = new List<Vector2>();
    private CubeList cubeList;

    public Vector3 coord;
    public bool preloaded = false;
    public bool preloaded2 = false;
    public bool loaded = false;
    public int[,,] map = new int[Globals.chunkSize, Globals.chunkSize, Globals.chunkSize];
    public bool empty = true;
    private object locker = new object();
    private Map worldMap;

    public BasicChunk(CubeList cubeList, Vector3 chunkCoord, Map worldMap)
    {
        this.cubeList = cubeList;
        this.coord = chunkCoord;
        this.worldMap = worldMap;
    }

    public void preload2()
    {
        for (int x = 0; x < Globals.chunkSize; x++)
        {
            for (int y = 0; y < Globals.chunkSize; y++)
            {
                for (int z = 0; z < Globals.chunkSize; z++)
                {
                    if (map[x, y, z] != 0)
                    {
                        AddBasicCubeDatas(new Vector3(x, y, z), x, y, z);
                    }
                }
            }
        }
        preloaded2 = true;
    }

    public void load()
    {
        initGameObject();
        CreateMesh();
        loaded = true;
    }

    private float Perlin3D(float x, float y, float z, float diviser)
    {
        x /= diviser;
        y /= diviser;
        z /= diviser;
        float ab = Mathf.PerlinNoise(x, y);
        float bc = Mathf.PerlinNoise(y, z);
        float ac = Mathf.PerlinNoise(x, z);

        float ba = Mathf.PerlinNoise(y, x);
        float cb = Mathf.PerlinNoise(z, y);
        float ca = Mathf.PerlinNoise(z, x);

        float abc = ab + bc + ac + ba + cb + ca;
        return abc / 6f;
    }
    private int getCubeToPlace(int x, int y, int z)
    {
        float perlin;

        if (y + coord.y * Globals.chunkSize < Globals.worldHeight)
        {
            perlin = Perlin3D(coord.x * Globals.chunkSize + x, coord.y * Globals.chunkSize + y, coord.z * Globals.chunkSize + z, Globals.perlinDiviser);
            if (perlin < .42f)
                return (int)Items.air;
            return (int)Items.stone;
        }
        perlin = Mathf.PerlinNoise((coord.x * Globals.chunkSize + x) / Globals.perlinDiviser, (coord.z * Globals.chunkSize + z) / Globals.perlinDiviser);
        if (coord.y * Globals.chunkSize + y < perlin * 15f)
            return (int)Items.stone;
        if (coord.y * Globals.chunkSize + y < perlin * 25f)
            return (int)Items.grass;
        return (int)Items.air;
    }

    public void preload()
    {
        for (int x = 0; x < Globals.chunkSize; x++)
        {
            for (int y = 0; y < Globals.chunkSize; y++)
            {
                for (int z = 0; z < Globals.chunkSize; z++)
                {
                    map[x, y, z] = getCubeToPlace(x, y, z);
                    if (map[x, y, z] != (int)Items.air)
                        empty = false;
                }
            }
        }
        preloaded = true;
    }

    bool checkSides(Vector3 pos)
    {
        int x = Mathf.FloorToInt(pos.x);
        int y = Mathf.FloorToInt(pos.y);
        int z = Mathf.FloorToInt(pos.z);

        if (x < 0)
            return cubeList.infosFromId[worldMap.map[Globals.getKey(Mathf.FloorToInt(coord.x - 1f), Mathf.FloorToInt(coord.y), Mathf.FloorToInt(coord.z))].map[Globals.chunkSize - 1, y, z]].opaque;
        if (x > Globals.chunkSize - 1)
            return cubeList.infosFromId[worldMap.map[Globals.getKey(Mathf.FloorToInt(coord.x + 1f), Mathf.FloorToInt(coord.y), Mathf.FloorToInt(coord.z))].map[0, y, z]].opaque;
        if (y < 0)
            return cubeList.infosFromId[worldMap.map[Globals.getKey(Mathf.FloorToInt(coord.x), Mathf.FloorToInt(coord.y - 1f), Mathf.FloorToInt(coord.z))].map[x, Globals.chunkSize - 1, z]].opaque;
        if (y > Globals.chunkSize - 1)
            return cubeList.infosFromId[worldMap.map[Globals.getKey(Mathf.FloorToInt(coord.x), Mathf.FloorToInt(coord.y + 1f), Mathf.FloorToInt(coord.z))].map[x, 0, z]].opaque;
        if (z < 0)
            return cubeList.infosFromId[worldMap.map[Globals.getKey(Mathf.FloorToInt(coord.x), Mathf.FloorToInt(coord.y), Mathf.FloorToInt(coord.z - 1f))].map[x, y, Globals.chunkSize - 1]].opaque;
        if (z > Globals.chunkSize - 1)
            return cubeList.infosFromId[worldMap.map[Globals.getKey(Mathf.FloorToInt(coord.x), Mathf.FloorToInt(coord.y), Mathf.FloorToInt(coord.z + 1f))].map[x, y, 0]].opaque;
        return cubeList.infosFromId[map[x, y, z]].opaque;
    }

    void AddBasicCubeDatas(Vector3 pos, int x, int y, int z)
    {
        for (int j = 0; j < 6; j++)
        {
            if (!checkSides(pos + BasicCube.faceChecks[j]))
            {
                vertices.Add(pos + BasicCube.cubeVertices[BasicCube.cubeIndices[j, 0]]);
                vertices.Add(pos + BasicCube.cubeVertices[BasicCube.cubeIndices[j, 1]]);
                vertices.Add(pos + BasicCube.cubeVertices[BasicCube.cubeIndices[j, 2]]);
                vertices.Add(pos + BasicCube.cubeVertices[BasicCube.cubeIndices[j, 3]]);

                AddTexture(cubeList.infosFromId[map[x, y, z]].faces[j]);

                triangles.Add(vertexIndex);
                triangles.Add(vertexIndex + 1);
                triangles.Add(vertexIndex + 2);
                triangles.Add(vertexIndex + 2);
                triangles.Add(vertexIndex + 1);
                triangles.Add(vertexIndex + 3);
                vertexIndex += 4;
            }
        }
    }

    void initGameObject()
    {
        chunkObject = new GameObject();
        meshFilter = chunkObject.AddComponent<MeshFilter>();
        meshRenderer = chunkObject.AddComponent<MeshRenderer>();
        meshRenderer.material = this.cubeList.textureMaterial;
        chunkObject.transform.SetParent(cubeList.transform);
        chunkObject.transform.position = new Vector3(coord.x * Globals.chunkSize, coord.y * Globals.chunkSize, coord.z * Globals.chunkSize);
        chunkObject.name = "Chunk " + coord.x + " " + coord.y + " " + coord.z;
    }

    void CreateMesh()
    {
        Mesh mesh = new Mesh();
        mesh.vertices = vertices.ToArray();
        mesh.triangles = triangles.ToArray();
        mesh.uv = uvs.ToArray();
        mesh.RecalculateNormals();
        meshFilter.mesh = mesh;
    }

    void AddTexture(int textureID)
    {
        float y = textureID / Globals.textureAtlasSizeInBlocks;
        float x = textureID - (y * Globals.textureAtlasSizeInBlocks);

        x *= Globals.normalizedBlockTextureSize;
        y *= Globals.normalizedBlockTextureSize;

        y = 1f - y - Globals.normalizedBlockTextureSize;

        uvs.Add(new Vector2(x, y));
        uvs.Add(new Vector2(x, y + Globals.normalizedBlockTextureSize));
        uvs.Add(new Vector2(x + Globals.normalizedBlockTextureSize, y));
        uvs.Add(new Vector2(x + Globals.normalizedBlockTextureSize, y + Globals.normalizedBlockTextureSize));
    }
}