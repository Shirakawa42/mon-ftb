using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Map : MonoBehaviour
{
    public Dictionary<Globals.Key, BasicChunk> map = new Dictionary<Globals.Key, BasicChunk>();
    public GameObject player;
    
    private CubeList cubeList;

    void Start()
    {
        cubeList = GetComponent<CubeList>();
    }

    public bool checkForVoxel(Vector3 pos)
    {
        int xCheck = Mathf.FloorToInt(pos.x);
        int yCheck = Mathf.FloorToInt(pos.y);
        int zCheck = Mathf.FloorToInt(pos.z);

        int xChunk = xCheck / Globals.chunkSize;
        int yChunk = yCheck / Globals.chunkSize;
        int zChunk = zCheck / Globals.chunkSize;

        xCheck -= (xChunk * Globals.chunkSize);
        yCheck -= (yChunk * Globals.chunkSize);
        zCheck -= (zChunk * Globals.chunkSize);

        Globals.Key key = new Globals.Key(Globals.posToChunkCoord(pos));
        if (map.ContainsKey(key))
            return cubeList.infosFromId[map[key].map[xCheck, yCheck, zCheck]].opaque;
        return false;
    }
}
