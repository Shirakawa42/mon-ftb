using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class DebugPanelGlobals
{
    public static int loadingChunks = 0;
    public static int loadedChunks = 0;
    public static IntVector3 currentChunk = new IntVector3(0, 0, 0);
    public static IntVector3 playerPos = new IntVector3(0, 0, 0);
    public static float lastLoadTime = .0f;
}
