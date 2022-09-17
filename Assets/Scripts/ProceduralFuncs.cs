using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class ProceduralFuncs
{
    private static int Perlin3D(float x, float y, float z, float diviser)
    {
        x /= diviser;
        y /= diviser;
        z /= diviser;
        float ab = Mathf.PerlinNoise(x + Globals.seed, y + Globals.seed);
        float bc = Mathf.PerlinNoise(y + Globals.seed, z + Globals.seed);
        float ac = Mathf.PerlinNoise(x + Globals.seed, z + Globals.seed);

        float ba = Mathf.PerlinNoise(y + Globals.seed, x + Globals.seed);
        float cb = Mathf.PerlinNoise(z + Globals.seed, y + Globals.seed);
        float ca = Mathf.PerlinNoise(z + Globals.seed, x + Globals.seed);

        float abc = ab + bc + ac + ba + cb + ca;
        return (int)((abc / 6f) * 100f);
    }

    private static int Perlin2D(Vector2 pos, float offset, float scale)
    {
        return (int)(100f * Mathf.PerlinNoise((pos.x + 0.1f) / (float)Globals.chunkSize * scale + offset + Globals.seed, (pos.y + 0.1f) / (float)Globals.chunkSize * scale + offset + Globals.seed));
    }

    private static int getCaveCube(int x, int y, int z)
    {
        float perlin;

        perlin = Perlin3D(x, y, z, Globals.perlinCaveDiviser);
        if (perlin < 32)
            return (int)Items.air;
        return (int)Items.stone;
    }

    private static int getSurfaceCube(int x, int y, int z, IntVector3 coord)
    {
        int perlin;

        perlin = Perlin2D(new Vector2(x, z), 0f, 2f) / 6;
        if (y < perlin - 3)
            return (int)Items.stone;
        if (y < perlin)
            return (int)Items.dirt;
        if (y < perlin + 1)
            return (int)Items.grass;
        if (y == perlin + 1)
        {
            if (Perlin2D(new Vector2(x, z), 250f, Globals.treePlacementScale) > Globals.treeZoneThreshold)
            {
                if (Perlin2D(new Vector2(x, z), 250f, Globals.treePlacementScale) > Globals.treePlacementThreshold)
                {
                    Structures.generateMegaTree(x, y, z, coord);
                    return (int)Items.air;
                }
                return (int)Items.air;
            }
        }
        return (int)Items.air;
    }

    public static void addModification(VoxelMod mod, IntVector3 blockPos, IntVector3 currentChunkCoord)
    {
        IntVector3 chunkPos = Map.getChunkPosFromBlocPos(blockPos);
        lock (Map.mapLocker)
        {
            if (Map.map(chunkPos) == null)
                Map.map(chunkPos, new BasicChunk(chunkPos, Map.map(currentChunkCoord).mapGenerator));
            if (mod.id != (int)Items.air)
                Map.map(chunkPos).empty = false;
        }
        lock (Map.map(chunkPos).modifications)
            Map.map(chunkPos).modifications.Enqueue(mod);
    }

    public static int getCubeToPlace(int x, int y, int z, IntVector3 coord)
    {
        x += Globals.chunkSize * coord.x;
        y += Globals.chunkSize * coord.y;
        z += Globals.chunkSize * coord.z;
        if (y < 0)
            return getCaveCube(x, y, z);
        return getSurfaceCube(x, y, z, coord);
    }
}

public class VoxelMod
{
    public readonly IntVector3 pos;
    public readonly int id;

    public VoxelMod()
    {
        id = 0;
        pos = new IntVector3(0, 0, 0);
    }

    public VoxelMod(IntVector3 _pos, int _id)
    {
        pos = _pos;
        id = _id;
    }
}