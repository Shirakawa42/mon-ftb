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
    public Material material;
    public Camera cam;

    [Range(0.95f, 0f)]
    public float globalLightLevel;
    public Color day;
    public Color night;

    private IntVector3 oldPlayerChunkCoord;
    private List<BasicChunk> chunksToLoad = new List<BasicChunk>();
    private List<BasicChunk> loadedChunks = new List<BasicChunk>();
    private bool loadingChunks = false;
    private bool preloadingChunks = false;

    void Start()
    {
        oldPlayerChunkCoord = Globals.posToChunkCoord(transform.position);
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

        for (int x = -preloadRange * Globals.chunkSize; x < preloadRange * Globals.chunkSize; x += Globals.chunkSize)
        {
            int keyx = Mathf.FloorToInt((pos.x + x) / Globals.chunkSize);
            for (int y = -preloadRange * Globals.chunkSize; y < preloadRange * Globals.chunkSize; y += Globals.chunkSize)
            {
                int keyy = Mathf.FloorToInt((pos.y + y) / Globals.chunkSize);
                for (int z = -preloadRange * Globals.chunkSize; z < preloadRange * Globals.chunkSize; z += Globals.chunkSize)
                {
                    int keyz = Mathf.FloorToInt((pos.z + z) / Globals.chunkSize);
                    lock (Map.mapLocker)
                    {
                        if (Map.map(keyx, keyy, keyz) == null || !Map.map(keyx, keyy, keyz).preloaded)
                        {
                            if (Map.map(keyx, keyy, keyz) == null)
                                Map.map(keyx, keyy, keyz, new BasicChunk(new IntVector3(keyx, keyy, keyz), this));
                            preloadQueue.Add(Map.map(keyx, keyy, keyz));
                            nbchunks++;
                        }
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

        for (int x = -loadRange * Globals.chunkSize; x < loadRange * Globals.chunkSize; x += Globals.chunkSize)
        {
            int keyx = Mathf.FloorToInt((pos.x + x) / Globals.chunkSize);
            for (int y = -loadRange * Globals.chunkSize; y < loadRange * Globals.chunkSize; y += Globals.chunkSize)
            {
                int keyy = Mathf.FloorToInt((pos.y + y) / Globals.chunkSize);
                for (int z = -loadRange * Globals.chunkSize; z < loadRange * Globals.chunkSize; z += Globals.chunkSize)
                {
                    int keyz = Mathf.FloorToInt((pos.z + z) / Globals.chunkSize);
                    BasicChunk chunk = Map.map(keyx, keyy, keyz);
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
        DebugPanelGlobals.lastLoadTime = watch.ElapsedMilliseconds;
        watch.Stop();
        return chunks;
    }

    void loadChunks(Vector3 pos)
    {
        preloadingChunks = true;
        preloader(pos);
        List<BasicChunk> chunks = preloader2(pos);
        lock (chunksToLoad)
            foreach (BasicChunk chunk in chunks)
                chunksToLoad.Add(chunk);
        preloadingChunks = false;
    }

    IEnumerator loadChunksSmoothly()
    {
        loadingChunks = true;

        while (chunksToLoad.Count > 0)
        {
            while (chunksToLoad.Count > 0 && (chunksToLoad[0].loaded || chunksToLoad[0].empty))
            {
                chunksToLoad.RemoveAt(0);
                DebugPanelGlobals.loadingChunks--;
            }
            for (int j = 0; j < Globals.chunkPerFrame && chunksToLoad.Count > 0; j++)
            {
                if (!chunksToLoad[0].loaded && !chunksToLoad[0].empty)
                {
                    chunksToLoad[0].load();
                    loadedChunks.Add(chunksToLoad[0]);
                }
                chunksToLoad.RemoveAt(0);
                DebugPanelGlobals.loadingChunks--;
            }
            DebugPanelGlobals.loadedChunks = loadedChunks.Count;
            yield return null;
        }
        loadingChunks = false;
    }

    void Update()
    {
        Globals.playerChunk = Globals.posToChunkCoord(transform.position);
        
        Shader.SetGlobalFloat("globalLightLevel", globalLightLevel);
        cam.backgroundColor = Color.Lerp(day, night, globalLightLevel);
        if (Globals.playerChunk != oldPlayerChunkCoord && !preloadingChunks)
        {
            Vector3 pos = transform.position;

            new Thread(() => loadChunks(pos)).Start();

            oldPlayerChunkCoord = Globals.playerChunk;
            DebugPanelGlobals.currentChunk = oldPlayerChunkCoord;
        }
        if (chunksToLoad.Count > 0 && !loadingChunks)
        {
            StartCoroutine("loadChunksSmoothly");
        }
    }
}
