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

    public ItemInfos getBlockAtPos(Vector3 pos)
    {
        Vector3 chunkPos = Globals.posToChunkCoord(pos);

        int x = Mathf.FloorToInt(pos.x) - (Mathf.FloorToInt(chunkPos.x) * Globals.chunkSize);
        int y = Mathf.FloorToInt(pos.y) - (Mathf.FloorToInt(chunkPos.y) * Globals.chunkSize);
        int z = Mathf.FloorToInt(pos.z) - (Mathf.FloorToInt(chunkPos.z) * Globals.chunkSize);

        Globals.Key key = new Globals.Key(chunkPos);
        if (map.ContainsKey(key) && map[key].preloaded)
            return cubeList.infosFromId[map[key].map[x, y, z]];
        return cubeList.infosFromId[(int)Items.air];
    }
}
