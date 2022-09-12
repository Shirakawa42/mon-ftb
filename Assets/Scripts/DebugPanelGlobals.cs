using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class DebugPanelGlobals
{
    public static int loadingChunks = 0;
    public static int loadedChunks = 0;
    public static Vector3 currentChunk = new Vector3(0, 0, 0);
    public static Vector3 playerPos = new Vector3(0, 0, 0);
}
