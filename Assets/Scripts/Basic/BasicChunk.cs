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
    private List<Color> colors = new List<Color>();

    public MapGenerator mapGenerator;
    public IntVector3 coord;
    public bool preloaded = false;
    public bool preloaded2 = false;
    public bool loaded = false;
    public int[,,] map = new int[Globals.chunkSize, Globals.chunkSize, Globals.chunkSize];
    public bool empty = true;
    public bool gameObjectInited = false;
    public Queue<VoxelMod> modifications = new Queue<VoxelMod>();

    public BasicChunk(IntVector3 _coord, MapGenerator _mapGenerator)
    {
        coord = _coord;
        mapGenerator = _mapGenerator;
    }

    public void preload2()
    {
        lock (modifications)
        {
            if (modifications.Count > 0)
            {
                while (modifications.Count > 0)
                {
                    VoxelMod v = modifications.Dequeue();
                    if (map[v.pos.x, v.pos.y, v.pos.z] == (int)Items.air)
                        map[v.pos.x, v.pos.y, v.pos.z] = v.id;
                }
            }
        }

        ClearMesh();
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
        if (preloaded2)
        {
            if (!gameObjectInited)
                initGameObject();
            CreateMesh();
        }

        preloaded2 = true;
    }

    public void load()
    {
        initGameObject();
        CreateMesh();
        loaded = true;
    }

    public void preload()
    {
        for (int x = 0; x < Globals.chunkSize; x++)
        {
            for (int y = 0; y < Globals.chunkSize; y++)
            {
                for (int z = 0; z < Globals.chunkSize; z++)
                {
                    map[x, y, z] = ProceduralFuncs.getCubeToPlace(x, y, z, coord);
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
            return CubeList.infosFromId[Map.map(coord.x - 1, coord.y, coord.z).map[Globals.chunkSize - 1, y, z]].opaque;
        if (x > Globals.chunkSize - 1)
            return CubeList.infosFromId[Map.map(coord.x + 1, coord.y, coord.z).map[0, y, z]].opaque;
        if (y < 0)
            return CubeList.infosFromId[Map.map(coord.x, coord.y - 1, coord.z).map[x, Globals.chunkSize - 1, z]].opaque;
        if (y > Globals.chunkSize - 1)
            return CubeList.infosFromId[Map.map(coord.x, coord.y + 1, coord.z).map[x, 0, z]].opaque;
        if (z < 0)
            return CubeList.infosFromId[Map.map(coord.x, coord.y, coord.z - 1).map[x, y, Globals.chunkSize - 1]].opaque;
        if (z > Globals.chunkSize - 1)
            return CubeList.infosFromId[Map.map(coord.x, coord.y, coord.z + 1).map[x, y, 0]].opaque;
        return CubeList.infosFromId[map[x, y, z]].opaque;
    }

    void AddBasicCubeDatas(Vector3 pos, int x, int y, int z)
    {
        bool opaque = CubeList.infosFromId[map[x, y, z]].opaque;

        for (int j = 0; j < 6; j++)
        {
            if (!checkSides(pos + BasicCube.faceChecks[j]))
            {
                vertices.Add(pos + BasicCube.cubeVertices[BasicCube.cubeIndices[j, 0]]);
                vertices.Add(pos + BasicCube.cubeVertices[BasicCube.cubeIndices[j, 1]]);
                vertices.Add(pos + BasicCube.cubeVertices[BasicCube.cubeIndices[j, 2]]);
                vertices.Add(pos + BasicCube.cubeVertices[BasicCube.cubeIndices[j, 3]]);

                AddTexture(CubeList.infosFromId[map[x, y, z]].faces[j]);

                float lightLevel;

                int yPos = (int)pos.y + 1;
                bool inShade = false;
                while (yPos < Globals.chunkSize)
                {
                    if (map[(int)pos.x, yPos, (int)pos.z] != 0)
                    {
                        inShade = true;
                        break;
                    }

                    yPos++;
                }

                if (inShade)
                    lightLevel = 0.5f;
                else
                    lightLevel = 0f;
                colors.Add(new Color(0, 0, 0, lightLevel));
                colors.Add(new Color(0, 0, 0, lightLevel));
                colors.Add(new Color(0, 0, 0, lightLevel));
                colors.Add(new Color(0, 0, 0, lightLevel));

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
        meshRenderer.material = mapGenerator.material;
        chunkObject.transform.SetParent(mapGenerator.world.transform);
        chunkObject.transform.position = new Vector3(coord.x * Globals.chunkSize, coord.y * Globals.chunkSize, coord.z * Globals.chunkSize);
        chunkObject.name = "Chunk " + coord.x + " " + coord.y + " " + coord.z;
        gameObjectInited = true;
    }

    void CreateMesh()
    {
        Mesh mesh = new Mesh();
        mesh.vertices = vertices.ToArray();
        mesh.triangles = triangles.ToArray();
        mesh.uv = uvs.ToArray();
        mesh.colors = colors.ToArray();
        mesh.RecalculateNormals();
        meshFilter.mesh = mesh;
    }

    void ClearMesh()
    {
        vertexIndex = 0;
        vertices.Clear();
        triangles.Clear();
        uvs.Clear();
        colors.Clear();
    }

    public void EditVoxel(Vector3 pos, int newID)
    {
        IntVector3 IntVector3 = Globals.posToChunkCoord(pos);

        int x = Mathf.FloorToInt(pos.x) - IntVector3.x * Globals.chunkSize;
        int y = Mathf.FloorToInt(pos.y) - IntVector3.y * Globals.chunkSize;
        int z = Mathf.FloorToInt(pos.z) - IntVector3.z * Globals.chunkSize;

        map[x, y, z] = newID;
        preload2();
        UpdateNeighbors(x, y, z, IntVector3);
    }

    void UpdateNeighbors(int x, int y, int z, IntVector3 IntVector3)
    {
        Vector3 thisVoxel = new Vector3(x, y, z);

        for (int p = 0; p < 6; p++)
        {
            Vector3 currentVoxel = thisVoxel + BasicCube.faceChecks[p];

            if (!IsVoxelInChunk((int)currentVoxel.x, (int)currentVoxel.y, (int)currentVoxel.z))
            {
                IntVector3 toTry = IntVector3 + BasicCube.faceChecks[p];
                Map.map((int)toTry.x, (int)toTry.y, (int)toTry.z).preload2();
            }
        }
    }

    bool IsVoxelInChunk(int x, int y, int z)
    {
        if (x < 0 || x > Globals.chunkSize - 1 || y < 0 || y > Globals.chunkSize - 1 || z < 0 || z > Globals.chunkSize - 1)
            return false;
        else
            return true;
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