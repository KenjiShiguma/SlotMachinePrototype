// Author(s): Kermit Mitchell III, Derived from Brackeys: https://youtu.be/tdSmKaJvCoA
// Start Date: 08/09/2019 8:05 AM | Last Edited: 08/09/2019 11:30 AM
// This script is a Singleton that helps with object spawning/despawning via pooling; improves frame rates

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectPoolManager : MonoBehaviour
{
    //[System.Serializable]
    public class Pool
    {
        public string objType; // the name of the object type
        public GameObject prefab; // the prefab of the object
        public int size; // the size of the pool, if known ahead of time
        public GameObject container;
        public bool isExpandable = false; // true if the pool size can increase during runtime
        public Queue<GameObject> objPool = new Queue<GameObject>(); // the queue of items

    }

    #region Singleton
    public static ObjectPoolManager instance;
    #endregion

    [SerializeField] private Dictionary<string, Pool> pools; // all pools of objects
    //private Dictionary<string, Queue<GameObject>> poolDict;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }

        //if (instance != this)
        //{
        //    Destroy(this);
        //}

    }

    private void Start()
    {
        if (pools == null)
        {
            pools = new Dictionary<string, Pool>();
        }

        //if (poolDict == null)
        //{
        //    poolDict = new Dictionary<string, Queue<GameObject>>();
        //}

        // Add all of the pools here:
        Pool pool = new Pool();

        pool.objType = "Egg";
        pool.prefab = Resources.Load("_Prefabs/Egg") as GameObject;
        pool.size = 24;
        pools.Add(pool.objType, pool);

        SetupAllPools();

    }

    private void SetupAllPools()
    {
        // Spawn each pool accordingly
        foreach (KeyValuePair<string, Pool> keyValue in pools)//Pool pool in pools)
        {
            // Create a container for each of the instantiated objects
            keyValue.Value.container = new GameObject();
            keyValue.Value.container.transform.position = Vector3.zero;
            keyValue.Value.container.name = keyValue.Value.objType + "Pool";

            // Create objects and store them in the queue
            //Queue<GameObject> objectPool = new Queue<GameObject>();
            for (int i = 0; i < keyValue.Value.size; i++)
            {
                GameObject obj = Instantiate(keyValue.Value.prefab);
                obj.name = keyValue.Value.objType + "(" + (i+1) + ")";
                obj.transform.parent = keyValue.Value.container.transform;
                obj.SetActive(false);
                keyValue.Value.objPool.Enqueue(obj);
            }

            // Lock in the newly created objects
            //poolDict.Add(keyValue.Value.objType, objectPool);
        }
    }

    public GameObject SpawnFromPool(string objType, Vector3 pos, Quaternion rot)
    {
        if (!pools.ContainsKey(objType))
        {
            Debug.LogError("Time: " + Time.time + " | " + "Pool with tag " + objType + " doesn't exist!");
            return null;
        }

        Pool pool = pools[objType];
        GameObject spawned = null;

        if (pool.objPool.Peek().activeSelf)
        {
            Debug.LogWarning("Time: " + Time.time + " | " + "All objects are in use! Creating a new obj...");
            
            spawned = Instantiate(pool.prefab);
            pool.size++;
            spawned.name = pool.objType + "(" + (pool.objPool.Count + 1) + ")";
            spawned.transform.parent = pool.container.transform;
            spawned.SetActive(false);

        }
        else
        {
            spawned = pool.objPool.Dequeue();
        }


        spawned.transform.position = pos;
        spawned.transform.rotation = rot;
        spawned.SetActive(true);

        pool.objPool.Enqueue(spawned);

        return spawned;
        
    }

}
