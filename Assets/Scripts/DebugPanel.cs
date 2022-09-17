using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DebugPanel : MonoBehaviour
{
    public Text fps;
    public Text currentChunk;
    public Text currentPos;
    public Text loadedChunks;
    public Text loadingChunks;
    public Text lastLoadTime;

    private float refreshTime = .25f;

    void Update()
    {
        if (refreshTime >= .25f)
        {
            fps.text = "FPS: " + Mathf.FloorToInt(1.0f / Time.smoothDeltaTime);
            currentChunk.text = "Current Chunk: " + Mathf.FloorToInt(DebugPanelGlobals.currentChunk.x) + " "
                    + Mathf.FloorToInt(DebugPanelGlobals.currentChunk.y) + " "
                    + Mathf.FloorToInt(DebugPanelGlobals.currentChunk.z);
            currentPos.text = "Current pos: " +  + Mathf.FloorToInt(DebugPanelGlobals.playerPos.x) + " "
                    + Mathf.FloorToInt(DebugPanelGlobals.playerPos.y) + " "
                    + Mathf.FloorToInt(DebugPanelGlobals.playerPos.z);
            loadedChunks.text = "Loaded Chunks: " + DebugPanelGlobals.loadedChunks;
            loadingChunks.text = "Loading Chunks: " + DebugPanelGlobals.loadingChunks;
            lastLoadTime.text = "Last loading time: " + (DebugPanelGlobals.lastLoadTime / 1000f).ToString("0.0") + "sec";
            refreshTime = 0f;
        }
        else
            refreshTime += Time.deltaTime;
    }
}
