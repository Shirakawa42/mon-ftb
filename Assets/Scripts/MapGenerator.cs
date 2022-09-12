using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading;
using System.Collections.Concurrent;
using System.Linq;
using System.Threading.Tasks;

public class MapGenerator : MonoBehaviour
{
    public int preloadRange;
    public int loadRange;
    public GameObject world;
    private Map map;
    private CubeList cubeList;
    private Vector3 playerChunkCoord;

    private List<BasicChunk> chunksToLoad = new List<BasicChunk>();
    private bool loadingChunks = false;
    private object locker = new object();
    private object locker2 = new object();

    void Start()
    {
        playerChunkCoord = Globals.posToChunkCoord(transform.position);
        map = world.GetComponent<Map>();
        cubeList = world.GetComponent<CubeList>();
        preloadRange *= Globals.chunkSize;
        loadRange *= Globals.chunkSize;
        Vector3 pos = transform.position;
        new Thread(() => loadChunks(pos)).Start();
    }

    void preloader(Vector3 pos)
    {
        int nbchunks = 0;
        BlockingCollection<BasicChunk> preloadQueue = new BlockingCollection<BasicChunk>();
        System.Diagnostics.Stopwatch watch = new System.Diagnostics.Stopwatch();

        watch.Start();

        List<Task> loadTasks = Enumerable.Range(0, Globals.preloadChunkThreads).Select(n => Task.Run(() =>
        {
            foreach (BasicChunk chunk in preloadQueue.GetConsumingEnumerable())
                chunk.preload();
        })).ToList();

        for (int x = -preloadRange; x < preloadRange; x += Globals.chunkSize)
        {
            int keyx = Mathf.FloorToInt((pos.x + x) / Globals.chunkSize);
            for (int y = -preloadRange; y < preloadRange; y += Globals.chunkSize)
            {
                int keyy = Mathf.FloorToInt((pos.y + y) / Globals.chunkSize);
                for (int z = -preloadRange; z < preloadRange; z += Globals.chunkSize)
                {
                    int keyz = Mathf.FloorToInt((pos.z + z) / Globals.chunkSize);
                    Globals.Key key = Globals.getKey(keyx, keyy, keyz);
                    if (map.map.ContainsKey(key) == false)
                    {
                        map.map[key] = new BasicChunk(cubeList, key.getVector3(), map);
                        preloadQueue.Add(map.map[key]);
                        nbchunks++;
                    }
                }
            }
        }
        preloadQueue.CompleteAdding();

        Debug.Log("chunks to preload: " + nbchunks + " in " + Globals.preloadChunkThreads + " threads.");
        Task.WaitAll(loadTasks.ToArray());
        Debug.Log("preloader done after " + watch.ElapsedMilliseconds + "ms");
        watch.Stop();
    }

    List<BasicChunk> preloader2(Vector3 pos)
    {
        List<BasicChunk> chunks = new List<BasicChunk>();
        System.Diagnostics.Stopwatch watch = new System.Diagnostics.Stopwatch();
        BlockingCollection<BasicChunk> preload2Queue = new BlockingCollection<BasicChunk>();

        watch.Start();

        List<Task> loadTasks = Enumerable.Range(0, Globals.preload2ChunkThreads)
            .Select(n => Task.Run(() =>
            {
                foreach (BasicChunk chunk in preload2Queue.GetConsumingEnumerable())
                    chunk.preload2();
            })).ToList();

        for (int x = -loadRange; x < loadRange; x += Globals.chunkSize)
        {
            int keyx = Mathf.FloorToInt((pos.x + x) / Globals.chunkSize);
            for (int y = -loadRange; y < loadRange; y += Globals.chunkSize)
            {
                int keyy = Mathf.FloorToInt((pos.y + y) / Globals.chunkSize);
                for (int z = -loadRange; z < loadRange; z += Globals.chunkSize)
                {
                    int keyz = Mathf.FloorToInt((pos.z + z) / Globals.chunkSize);
                    Globals.Key key = Globals.getKey(keyx, keyy, keyz);
                    BasicChunk chunk = map.map[key];
                    if (!chunk.preloaded2)
                    {
                        preload2Queue.Add(chunk);
                        chunks.Add(chunk);
                        DebugPanelGlobals.loadingChunks++;
                    }
                }
            }
        }
        preload2Queue.CompleteAdding();

        Debug.Log("chunks to preload2: " + chunks.Count + " in " + Globals.preload2ChunkThreads + " threads.  Added in " + watch.ElapsedMilliseconds + "ms");
        Task.WaitAll(loadTasks.ToArray());
        Debug.Log("preloader2 done after " + watch.ElapsedMilliseconds + "ms");
        watch.Stop();
        return chunks;
    }

    void loadChunks(Vector3 pos)
    {
        lock (locker2)
        {
            preloader(pos);
            List<BasicChunk> chunks = preloader2(pos);
            lock (locker)
                foreach (BasicChunk chunk in chunks)
                    chunksToLoad.Add(chunk);
        }
    }

    IEnumerator loadChunksSmoothly()
    {
        loadingChunks = true;

        while (chunksToLoad.Count > 0)
        {
            int i = 0;

            while (chunksToLoad.Count > 0 && i < 1)
            {
                while (chunksToLoad.Count > 0 && (chunksToLoad[0].loaded || chunksToLoad[0].empty))
                {
                    chunksToLoad.RemoveAt(0);
                    DebugPanelGlobals.loadingChunks--;
                }
                for (int j = 0; j < Globals.chunkPerFrame && chunksToLoad.Count > 0; j++)
                {
                    if (!chunksToLoad[0].loaded && !chunksToLoad[0].empty)
                        chunksToLoad[0].load();
                    chunksToLoad.RemoveAt(0);
                    DebugPanelGlobals.loadedChunks++;
                    DebugPanelGlobals.loadingChunks--;
                }
                i++;
            }
            yield return null;
        }
        loadingChunks = false;
    }

    void Update()
    {
        Vector3 chunkCoord = Globals.posToChunkCoord(transform.position);

        if (chunkCoord != playerChunkCoord)
        {
            Vector3 pos = transform.position;
            new Thread(() => loadChunks(pos)).Start();
            playerChunkCoord = chunkCoord;
            DebugPanelGlobals.currentChunk = playerChunkCoord;
        }
        if (chunksToLoad.Count > 0 && !loadingChunks)
        {
            StartCoroutine("loadChunksSmoothly");
        }
    }
}
