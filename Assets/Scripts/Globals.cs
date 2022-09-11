using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Globals
{
    public static readonly int chunkSize = 32;
    public static float perlinDiviser = 28.142f;
    public static readonly int worldHeight = 0;
    public static readonly int textureAtlasSizeInBlocks = 4;
    public static readonly int preloadChunkPerThread = 8;
    public static readonly int preload2ChunkPerThread = 512;

    public static float normalizedBlockTextureSize
    {
        get { return 1f / (float)textureAtlasSizeInBlocks; }
    }

    public static Vector3 posToChunkCoord(Vector3 pos)
    {
        return new Vector3((int)Mathf.Floor(pos.x / Globals.chunkSize),
                           (int)Mathf.Floor(pos.y / Globals.chunkSize),
                           (int)Mathf.Floor(pos.z / Globals.chunkSize));
    }

    public struct Key
    {
        public readonly int x;
        public readonly int y;
        public readonly int z;
        public Key(int item1, int item2, int item3)
        {
            x = item1;
            y = item2;
            z = item3;
        }

        public Key(float item1, float item2, float item3)
        {
            x = Mathf.FloorToInt(item1);
            y = Mathf.FloorToInt(item2);
            z = Mathf.FloorToInt(item3);
        }

        public Key(Vector3 pos)
        {
            x = Mathf.FloorToInt(pos.x);
            y = Mathf.FloorToInt(pos.y);
            z = Mathf.FloorToInt(pos.z);
        }

        public Vector3 getVector3()
        {
            return new Vector3(x, y, z);
        }
    }

    public static Key getKey(int x, int y, int z)
    {
        return new Key(x, y, z);
    }
}
