using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Collections.Concurrent;

public static class Map
{
    public static object mapLocker = new object();
    private static BasicChunk[,,] mapa = new BasicChunk[Globals.maxWorldSize, Globals.maxWorldSize, Globals.maxWorldSize];

    public static BasicChunk map(int x, int y, int z)
    {
        return mapa[x + Globals.maxWorldSize / 2, y + Globals.maxWorldSize / 2, z + Globals.maxWorldSize / 2];
    }

    public static BasicChunk map(IntVector3 coord)
    {
        return mapa[coord.x + Globals.maxWorldSize / 2, coord.y + Globals.maxWorldSize / 2, coord.z + Globals.maxWorldSize / 2];
    }

    public static void map(int x, int y, int z, BasicChunk value)
    {
        mapa[x + Globals.maxWorldSize / 2, y + Globals.maxWorldSize / 2, z + Globals.maxWorldSize / 2] = value;
    }

    public static void map(IntVector3 pos, BasicChunk value)
    {
        mapa[pos.x + Globals.maxWorldSize / 2, pos.y + Globals.maxWorldSize / 2, pos.z + Globals.maxWorldSize / 2] = value;
    }

    public static ItemInfos getBlockAtPos(Vector3 pos)
    {
        IntVector3 intVector3 = Globals.posToChunkCoord(pos);

        int x = Mathf.FloorToInt(pos.x) - intVector3.x * Globals.chunkSize;
        int y = Mathf.FloorToInt(pos.y) - intVector3.y * Globals.chunkSize;
        int z = Mathf.FloorToInt(pos.z) - intVector3.z * Globals.chunkSize;

        if (map(intVector3.x, intVector3.y, intVector3.z) != null && map(intVector3.x, intVector3.y, intVector3.z).preloaded)
            return CubeList.infosFromId[map(intVector3.x, intVector3.y, intVector3.z).map[x, y, z]];
        return CubeList.infosFromId[(int)Items.air];
    }

    public static IntVector3 getBlockPosInChunk(IntVector3 blocPos)
    {
        return new IntVector3(blocPos.x % Globals.chunkSize, blocPos.y % Globals.chunkSize, blocPos.z % Globals.chunkSize);
    }

    public static IntVector3 getBlockPosInChunk(int x, int y, int z)
    {
        x += Globals.chunkSize * (Globals.maxWorldSize / 2);
        y += Globals.chunkSize * (Globals.maxWorldSize / 2);
        z += Globals.chunkSize * (Globals.maxWorldSize / 2);
        return new IntVector3(x % Globals.chunkSize, y % Globals.chunkSize, z % Globals.chunkSize);
    }

    public static IntVector3 getChunkPosFromBlocPos(IntVector3 blocPos)
    {
        return new IntVector3(Mathf.FloorToInt((float)blocPos.x / Globals.chunkSize), Mathf.FloorToInt((float)blocPos.y / Globals.chunkSize), Mathf.FloorToInt((float)blocPos.z / Globals.chunkSize));
    }
}
