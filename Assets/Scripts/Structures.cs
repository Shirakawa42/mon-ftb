using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Structures
{
    public static void generateMegaTree(int startx, int starty, int startz, IntVector3 currentChunkCoord)
    {
        for (int i = 0; i < 50; i++)
        {
            ProceduralFuncs.addModification(new VoxelMod(Map.getBlockPosInChunk(startx, starty + i, startz), (int)Items.wood), new IntVector3(startx, starty + i, startz), currentChunkCoord);
        }
    }
}
