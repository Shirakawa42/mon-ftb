using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading;

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

    void preloadChunks(List<BasicChunk> chunks)
    {
        foreach (BasicChunk chunk in chunks)
            chunk.preload();
    }

    void preload2Chunks(List<BasicChunk> chunks)
    {
        foreach (BasicChunk chunk in chunks)
            chunk.preload2();
    }

    void preloader(Vector3 pos)
    {
        int nbchunks = 0;
        List<Thread> threads = new List<Thread>();
        List<BasicChunk> chunksToThread = new List<BasicChunk>();
        System.Diagnostics.Stopwatch watch = new System.Diagnostics.Stopwatch();

        watch.Start();
        for (int x = -preloadRange; x < preloadRange; x += Globals.chunkSize)
        {
            for (int y = -preloadRange; y < preloadRange; y += Globals.chunkSize)
            {
                for (int z = -preloadRange; z < preloadRange; z += Globals.chunkSize)
                {
                    int keyx = (int)Mathf.Floor((pos.x + x) / Globals.chunkSize);
                    int keyy = (int)Mathf.Floor((pos.y + y) / Globals.chunkSize);
                    int keyz = (int)Mathf.Floor((pos.z + z) / Globals.chunkSize);
                    Globals.Key key = Globals.getKey(keyx, keyy, keyz);
                    if (map.map.ContainsKey(key) == false)
                    {
                        map.map[key] = new BasicChunk(cubeList, key.getVector3(), map);
                        chunksToThread.Add(map.map[key]);
                        nbchunks++;
                    }
                    if (chunksToThread.Count >= Globals.preloadChunkPerThread)
                    {
                        List<BasicChunk> prout = chunksToThread;
                        Thread thread = new Thread(() => preloadChunks(prout));
                        thread.Start();
                        threads.Add(thread);
                        chunksToThread = new List<BasicChunk>();
                    }
                }
            }
        }
        if (chunksToThread.Count > 0)
        {
            Thread thread = new Thread(() => preloadChunks(chunksToThread));
            thread.Start();
            threads.Add(thread);
        }
        Debug.Log("chunks to preload: " + nbchunks + " in " + threads.Count + " threads.");
        foreach (Thread t in threads)
            t.Join();
        Debug.Log("preloader done after " + watch.ElapsedMilliseconds + "ms");
        watch.Stop();
    }

    List<BasicChunk> preloader2(Vector3 pos)
    {
        List<Thread> threads = new List<Thread>();
        List<BasicChunk> chunksToThread = new List<BasicChunk>();
        List<BasicChunk> chunks = new List<BasicChunk>();
        System.Diagnostics.Stopwatch watch = new System.Diagnostics.Stopwatch();

        watch.Start();
        for (int x = -loadRange; x < loadRange; x += Globals.chunkSize)
        {
            for (int y = -loadRange; y < loadRange; y += Globals.chunkSize)
            {
                for (int z = -loadRange; z < loadRange; z += Globals.chunkSize)
                {
                    int keyx = (int)Mathf.Floor((pos.x + x) / Globals.chunkSize);
                    int keyy = (int)Mathf.Floor((pos.y + y) / Globals.chunkSize);
                    int keyz = (int)Mathf.Floor((pos.z + z) / Globals.chunkSize);
                    Globals.Key key = Globals.getKey(keyx, keyy, keyz);
                    if (!map.map[key].preloaded2)
                    {
                        chunksToThread.Add(map.map[key]);
                        chunks.Add(map.map[key]);
                    }
                    if (chunksToThread.Count >= Globals.preload2ChunkPerThread)
                    {
                        List<BasicChunk> prout = chunksToThread;
                        Thread thread = new Thread(() => preload2Chunks(prout));
                        thread.Start();
                        threads.Add(thread);
                        chunksToThread = new List<BasicChunk>();
                    }
                }
            }
        }
        if (chunksToThread.Count > 0)
        {
            Thread thread = new Thread(() => preload2Chunks(chunksToThread));
            thread.Start();
            threads.Add(thread);
        }
        Debug.Log("chunks to preload2: " + chunks.Count + " in " + threads.Count + " threads.");
        foreach (Thread t in threads)
            t.Join();
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
                if (!chunksToLoad[0].loaded && !chunksToLoad[0].empty)
                    chunksToLoad[0].load();
                chunksToLoad.RemoveAt(0);
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
        }
        if (chunksToLoad.Count > 0 && !loadingChunks)
        {
            StartCoroutine("loadChunksSmoothly");
        }
    }
}
