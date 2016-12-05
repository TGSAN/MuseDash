using UnityEngine;
using System.Collections.Generic;


public class FastPoolManager : MonoBehaviour
{
    /// <summary>
    /// Scene instance of the FastPoolManager
    /// </summary>
    public static FastPoolManager Instance
    { get; private set; }

    /// <summary>
    /// List of managed runtime pools
    /// </summary>
    public Dictionary<int, FastPool> Pools
    { get { return pools; } }

    [SerializeField]
    List<FastPool> predefinedPools;
    Dictionary<int, FastPool> pools;
    Transform curTransform;


    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            curTransform = GetComponent<Transform>();
            pools = new Dictionary<int, FastPool>();
        }
        else
            Debug.LogError("You cannot have more than one instance of FastPoolManager in the scene!");


        for (int i = 0; i < predefinedPools.Count; i++)
        {
            if (predefinedPools[i].Init(curTransform))
            {
                if (!pools.ContainsKey(predefinedPools[i].ID))
                    pools.Add(predefinedPools[i].ID, predefinedPools[i]);
                else
                    Debug.LogError("FastPoolManager has a several pools with the same ID. Please make sure that all your pools have unique IDs");
            }
        }

        predefinedPools.Clear();
    }

    void Start()
    {
        //Moved to awake method

        //for (int i = 0; i < predefinedPools.Count; i++)
        //{
        //    if (predefinedPools[i].Init(curTransform))
        //        pools.Add(predefinedPools[i].ID, predefinedPools[i]);
        //}

        //predefinedPools.Clear();
    }



    /// <summary>
    /// Create a new pool from provided component
    /// </summary>
    /// <typeparam name="T">Component type</typeparam>
    /// <param name="component">Component which game object will be cloned</param>
    /// <param name="preloadCount">How much items must be preloaded and cached</param>
    /// <param name="capacity">Cache size (maximum amount of the cached items). [0 - unlimited]</param>
    /// <param name="warmOnLoad">Load source prefab in the memory while scene is loading. A bit slower scene loading, but much faster instantiating of new objects in pool</param>
    /// <returns>FastPool instance</returns>
    public static FastPool CreatePoolC<T>(T component, bool warmOnLoad = true, int preloadCount = 0, int capacity = 0)
        where T : Component
    {
        if (component != null)
            return CreatePool(component.gameObject, warmOnLoad, preloadCount, capacity);
        else
            return null;
    }

    /// <summary>
    /// Create a new pool from provided prefab
    /// </summary>
    /// <param name="prefab">Prefab that will be cloned</param>
    /// <param name="preloadCount">How much items must be preloaded and cached</param>
    /// <param name="capacity">Cache size (maximum amount of the cached items). [0 - unlimited]</param>
    /// <param name="warmOnLoad">Load source prefab in the memory while scene is loading. A bit slower scene loading, but much faster instantiating of new objects in pool</param>
    /// <returns>FastPool instance</returns>
    public static FastPool CreatePool(GameObject prefab, bool warmOnLoad = true, int preloadCount = 0, int capacity = 0)
    {
        if (prefab != null)
        {
            if (!Instance.pools.ContainsKey(prefab.GetInstanceID()))
            {
                FastPool newPool = new FastPool(prefab, Instance.curTransform, warmOnLoad, preloadCount, capacity);
                Instance.pools.Add(prefab.GetInstanceID(), newPool);
                return newPool;
            }
            else
                return Instance.pools[prefab.GetInstanceID()];
        }
        else
        {
            Debug.LogError("Creating pool with null object");
            return null;
        }
    }
    /// <summary>
    /// Create a new pool for provided id (by default the InstanceID of the source prefab is used)
    /// </summary>
    /// <param name="id">Pool ID. You can provadi any number, but by default FastPool uses the InstanceID of the source prefab</param>
    /// <param name="prefab">Prefab that will be cloned</param>
    /// <param name="preloadCount">How much items must be preloaded and cached</param>
    /// <param name="capacity">Cache size (maximum amount of the cached items). [0 - unlimited]</param>
    /// <param name="warmOnLoad">Load source prefab in the memory while scene is loading. A bit slower scene loading, but much faster instantiating of new objects in pool</param>
    /// <returns>FastPool instance</returns>
    public static FastPool CreatePool(int id, GameObject prefab, bool warmOnLoad = true, int preloadCount = 0, int capacity = 0)
    {
        if (prefab != null)
        {
            if (!Instance.pools.ContainsKey(id))
            {
                FastPool newPool = new FastPool(id, prefab, Instance.curTransform, warmOnLoad, preloadCount, capacity);
                Instance.pools.Add(id, newPool);
                return newPool;
            }
            else
                return Instance.pools[id];
        }
        else
        {
            Debug.LogError("Creating pool with null object");
            return null;
        }
    }



    /// <summary>
    /// Returns pool for the specified prefab or creates it if needed (with default params)
    /// </summary>
    /// <param name="prefab">Source Prefab</param>
    /// <param name="createIfNotExists">Create a new pool if it doesn't exists for provided source prefab</param>
    /// <returns></returns>
    public static FastPool GetPool(GameObject prefab, bool createIfNotExists = true)
    {
        if (prefab != null)
        {
            if (Instance.pools.ContainsKey(prefab.GetInstanceID()))
                return Instance.pools[prefab.GetInstanceID()];
            else
                return CreatePool(prefab);
        }
        else
        {
            Debug.LogError("Trying to get pool for null object");
            return null;
        }
    }
    /// <summary>
    /// Returns pool for the specified prefab instance ID or creates it if needed (with default params)
    /// </summary>
    /// <param name="id">Pool ID. By default FastPool uses the InstanceID of the source prefab</param>
    /// <param name="prefab">Source Prefab. Will be used if pool for provided ID doesnot exists.\r\nCan be null if createIfNotExists is false</param>
    /// <param name="createIfNotExists">Create a new pool if it doesn't exists for provided id</param>
    /// <returns></returns>
    public static FastPool GetPool(int id, GameObject prefab, bool createIfNotExists = true)
    {
        if (Instance.pools.ContainsKey(id))
            return Instance.pools[id];
        else
            return CreatePool(id, prefab);
    }
    /// <summary>
    /// Returns pool for the specified prefab or creates it if needed (with default params)
    /// </summary>
    /// <param name="component">Any component of the source prefab</param>
    /// <param name="createIfNotExists">Create a new pool if it doesn't exists for game object of the provided component</param>
    /// <returns></returns>
    public static FastPool GetPool(Component component, bool createIfNotExists = true)
    {
        if (component != null)
        {
            GameObject prefab = component.gameObject;
            if (Instance.pools.ContainsKey(prefab.GetInstanceID()))
                return Instance.pools[prefab.GetInstanceID()];
            else
                return CreatePool(prefab);
        }
        else
        {
            Debug.LogError("Trying to get pool for null object");
            return null;
        }
    }



    /// <summary>
    /// Destroys provided pool and it's cached objects
    /// </summary>
    /// <param name="pool">Pool to destroy</param>
    public static void DestroyPool(FastPool pool)
    {
        pool.ClearCache();
        Instance.pools.Remove(pool.ID);
    }

}
