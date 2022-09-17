using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Globals
{
    // Optimisation
    public static readonly int preloadChunkThreads = 16;
    public static readonly int preload2ChunkThreads = 16;
    public static readonly int chunkPerFrame = 16;

    // Procedural
    public static readonly float seed = 384f;
    public static readonly int chunkSize = 48;
    public static float perlinCaveDiviser = 15f;
    public static readonly int maxWorldSize = 512;

    // Procedural Structures
    public static float treePlacementScale = 8f;
    public static int treeZoneThreshold = 70;
    public static int treePlacementThreshold = 90;
    public static int maxTreeHeight = 12;
    public static int minTreeHeight = 6;

    // Player infos
    public static readonly float playerHeight = 2f;
    public static IntVector3 playerChunk;

    // other
    public static readonly int textureAtlasSizeInBlocks = 16;
    public static object locker = new object();

    public static float normalizedBlockTextureSize
    {
        get { return 1f / (float)textureAtlasSizeInBlocks; }
    }

    public static IntVector3 posToChunkCoord(Vector3 pos)
    {
        return new IntVector3((int)Mathf.Floor(pos.x / Globals.chunkSize),
                           (int)Mathf.Floor(pos.y / Globals.chunkSize),
                           (int)Mathf.Floor(pos.z / Globals.chunkSize));
    }
}