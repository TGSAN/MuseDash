using UnityEngine;
using System.Collections.Generic;


/// <summary>
/// FastPool notification type
/// </summary>
public enum PoolItemNotificationType
{
    /// <summary>
    /// Don't notify
    /// </summary>
    None,
    /// <summary>
    /// Notify via interface (make direct method call)
    /// </summary>
    Interface,
    /// <summary>
    /// Notify via SendMessage call
    /// </summary>
    SendMessage,
    /// <summary>
    /// Notify via BroadcastMessage call
    /// </summary>
    BroadcastMessage
}


[System.Serializable]
public class FastPool
{
    /// <summary>
    /// ID of the source Prefab
    /// </summary>
    public int ID
    { get; private set; }
    /// <summary>
    /// Name of the source Prefab
    /// </summary>
    public string Name
    { get { return sourcePrefab.name; } }
    /// <summary>
    /// Cached objects count
    /// </summary>
    public int Cached
    { get { return cache.Count; } }
    /// <summary>
    /// Shows if the pool has been successfully initialized. Has the same value as the last Init() call returns
    /// </summary>
    public bool IsValid
    { get; private set; }

    /// <summary>
    /// Prefab that would be used as source
    /// </summary>
    [SerializeField]
    [Tooltip("Prefab that would be used as source")]
    GameObject sourcePrefab;

    /// <summary>
    /// Cache size (maximum amount of the cached items).\r\n[0 - unlimited]
    /// </summary>
    [Tooltip("Cache size (maximum amount of the cached items).\r\n[0 - unlimited]")]
    public int Capacity;

    /// <summary>
    /// How much items must be cached at the start
    /// </summary>
    [Tooltip("How much items must be cached at the start")]
    public int PreloadCount;

    /// <summary>
    /// How to call events OnFastInstantiate and OnFastDestroy. Note, that if use choose notification via Interface, you must implement IFastPoolItem in any script on your sourcePrefab
    /// </summary>
    [Tooltip("How to call events OnFastInstantiate and OnFastDestroy. Note, that if use choose notification via Interface, you must implement IFastPoolItem in any script on your sourcePrefab")]
    public PoolItemNotificationType NotificationType;

    /// <summary>
    /// Load source prefab in the memory while scene is loading. A bit slower scene loading, but faster instantiating of new objects in pool
    /// </summary>
    [Tooltip("Load source prefab in the memory while scene is loading. A bit slower scene loading, but faster instantiating of new objects in pool")]
    public bool WarmOnLoad = true;

    /// <summary>
    /// Parent cached objects to FastPoolManager game object.\r\n[WARNING] Turning this option on will make objects cached a bit slower.
    /// </summary>
    [Tooltip("Parent cached objects to FastPoolManager game object.\r\n[WARNING] Turning this option on will make objects cached a bit slower.")]
    public bool ParentOnCache = false;

    /// <summary>
    /// Parent cached objects to FastPoolManager game object.\r\n[WARNING] Turning this option on will make objects cached a bit slower.
    /// </summary>
    [Tooltip("Use custom pool ID. By default it equals to the InstanceID of the source prefab.\r\n[WARNING] Be careful with this option.")]
    [SerializeField]
    bool useCustomID = false;

    [SerializeField]
    [HideInInspector]
    int customID = -1;

    //helper variable to display count of cached objects by property drawer
    [SerializeField]
    [HideInInspector]
    int cached_internal;


    Stack<GameObject> cache;
    Transform parentTransform;


    /// <summary>
    /// Creates a new instance of FastPool
    /// </summary>
    /// <param name="prefab">Prefab that would be used as source</param>
    /// <param name="rootTransform">Transform to parent cached objects on. Use null if you don't want to change parent</param>
    /// <param name="warmOnLoad">Load source prefab in the memory while scene is loading. A bit slower scene loading, but faster instantiating of new objects in pool</param>
    /// <param name="preloadCount">How much items must be cached at the start</param>
    /// <param name="capacity">Cache size (maximum amount of the cached items)  [0 - unlimited]</param>
    public FastPool(GameObject prefab, Transform rootTransform = null, bool warmOnLoad = true, int preloadCount = 0, int capacity = 0)
    {
        sourcePrefab = prefab;
        PreloadCount = preloadCount;
        Capacity = capacity;
        WarmOnLoad = warmOnLoad;
        Init(rootTransform);
    }
    /// <summary>
    /// Creates a new instance of FastPool
    /// </summary>
    /// <param name="id">Custom ID of this pool. By default it equals to the InstanceID of the source prefab</param>
    /// <param name="prefab">Prefab that would be used as source</param>
    /// <param name="rootTransform">Transform to parent cached objects on. Use null if you don't want to change parent</param>
    /// <param name="warmOnLoad">Load source prefab in the memory while scene is loading. A bit slower scene loading, but faster instantiating of new objects in pool</param>
    /// <param name="preloadCount">How much items must be cached at the start</param>
    /// <param name="capacity">Cache size (maximum amount of the cached items)  [0 - unlimited]</param>
    public FastPool(int id, GameObject prefab, Transform rootTransform = null, bool warmOnLoad = true, int preloadCount = 0, int capacity = 0)
    {
        useCustomID = true;
        customID = id;
        sourcePrefab = prefab;
        PreloadCount = preloadCount;
        Capacity = capacity;
        WarmOnLoad = warmOnLoad;
        Init(rootTransform);
    }


    /// <summary>
    /// Initialize pool with current parameters
    /// </summary>
    /// <param name="rootTransform">Transform to parent cached object on. Use null if you don't want to change parent</param>
    /// <returns></returns>
    public bool Init(Transform rootTransform)
    {
        if (sourcePrefab != null)
        {
            cached_internal = 0;
            cache = new Stack<GameObject>();
            parentTransform = rootTransform;
            ID = useCustomID ? customID : sourcePrefab.GetInstanceID();

            if (WarmOnLoad)
            {
                string name = string.Concat(sourcePrefab.name, "_SceneSource");
                sourcePrefab = MakeClone(Vector3.zero, Quaternion.identity, parentTransform);
                sourcePrefab.name = name;
                sourcePrefab.SetActive(false);
            }

            //Preload
            for (int i = cache.Count; i < PreloadCount; i++)
                FastDestroy(MakeClone(Vector3.zero, Quaternion.identity, null));

            IsValid = true;
        }
        else
        {
            Debug.LogError("Creating pool with null prefab!");
            IsValid = false;
        }

        return IsValid;
    }

    /// <summary>
    /// Unload all cached objects from memory
    /// </summary>
    public void ClearCache()
    {
        while (cache.Count > 0)
            GameObject.Destroy(cache.Pop());

        cached_internal = 0;
    }


    #region Instantiate
    /// <summary>
    /// Quickly instantiate GameObject from pool and return provided component from an instantiated GameObject.
    /// </summary>
    /// <typeparam name="T">Component to find on instantiated GameObject</typeparam>
    /// <param name="parent">Make instantiated GameObject the child of provided transform. Null if no parent needed</param>
    /// <returns></returns>
    public T FastInstantiate<T>(Transform parent = null)
        where T : Component
    {
        return FastInstantiate<T>(Vector3.zero, Quaternion.identity, parent);
    }

    /// <summary>
    /// Quickly instantiate GameObject from pool and return provided component from an instantiated GameObject.
    /// </summary>
    /// <typeparam name="T">Component to find on instantiated GameObject</typeparam>
    /// <param name="position">Position for the instantiated GameObject</param>
    /// <param name="rotation">Rotation of instantiated GameObject</param>
    /// <param name="parent">Make instantiated GameObject the child of provided transform. Null if no parent needed</param>
    /// <returns></returns>
    public T FastInstantiate<T>(Vector3 position, Quaternion rotation, Transform parent = null)
        where T : Component
    {
        GameObject clone = FastInstantiate(position, rotation, parent);

        return (clone != null) ? clone.GetComponent<T>() : null;
    }

    /// <summary>
    /// Quickly instantiate GameObject from pool.
    /// </summary>
    /// <param name="parent">Make instantiated GameObject the child of provided transform. Null if no parent needed</param>
    /// <returns></returns>
    public GameObject FastInstantiate(Transform parent = null)
    {
        return FastInstantiate(Vector3.zero, Quaternion.identity, parent);
    }

    /// <summary>
    /// Quickly instantiate GameObject from pool.
    /// </summary>
    /// <param name="position">Position for the instantiated GameObject</param>
    /// <param name="rotation">Rotation of instantiated GameObject</param>
    /// <param name="parent">Make instantiated GameObject the child of provided transform. Null if no parent needed</param>
    /// <returns></returns>
    public GameObject FastInstantiate(Vector3 position, Quaternion rotation, Transform parent = null)
    {
        GameObject clone;

        //find not null item in the cache
        while (cache.Count > 0)
        {
            clone = cache.Pop();
            cached_internal--;

            if (clone != null)
            {
                Transform tr = clone.transform;
                tr.localPosition = position;
                tr.localRotation = rotation;
                if (parent != null)
#if UNITY_4_5 || UNITY_4_6
                    tr.parent = parent;
#else
                    tr.SetParent(parent, false);
#endif

                clone.SetActive(true);
                switch (NotificationType)
                {
                    case PoolItemNotificationType.Interface:
#if UNITY_4_5 || UNITY_4_6
                        Component[] pItems = clone.GetComponents(typeof(IFastPoolItem));
                        for (int i = 0; i < pItems.Length; i++)
                            ((IFastPoolItem)pItems[i]).OnFastInstantiate();
#else
                        IFastPoolItem[] pItems = clone.GetComponents<IFastPoolItem>();
                        for (int i = 0; i < pItems.Length; i++)
                            pItems[i].OnFastInstantiate();
#endif
                        break;
                    case PoolItemNotificationType.SendMessage:
                        clone.SendMessage("OnFastInstantiate");
                        break;
                    case PoolItemNotificationType.BroadcastMessage:
                        clone.BroadcastMessage("OnFastInstantiate");
                        break;
                }

                return clone;
            }
            else
                Debug.LogWarning("The pool with the " + sourcePrefab.name + " prefab contains null entry. Don't destroy cached items manually!");
        }


        clone = MakeClone(position, rotation, parent);
        if (WarmOnLoad)
            clone.SetActive(true);

        return clone;
    }
    #endregion


    #region Destroy
    /// <summary>
    /// Disable GameObject of provided Component and put it into the pool for later use. If the pool size reached capacity limit - GameObject will be simply Destroyed.
    /// </summary>
    /// <typeparam name="T">Component of the GameObject that you want to put into the pool</typeparam>
    /// <param name="sceneObject"></param>
    public void FastDestroy<T>(T sceneObject)
        where T : Component
    {
        if (sceneObject != null)
            FastDestroy(sceneObject.gameObject);
        else
            Debug.LogWarning("Attempt to destroy a null object");
    }

    /// <summary>
    /// Disable provided GameObject and put it into the pool for later use. If the pool size reached capacity limit - GameObject will be simply Destroyed.
    /// </summary>
    /// <param name="sceneObject">GameObject to put into the pool</param>
    public void FastDestroy(GameObject sceneObject)
    {
        if (sceneObject != null)
        {
            if (cache.Count < Capacity || Capacity <= 0)
            {
                cache.Push(sceneObject);
                cached_internal++;
                if (ParentOnCache)
#if UNITY_4_5 || UNITY_4_6
                    sceneObject.transform.parent = parentTransform;
#else
                    sceneObject.transform.SetParent(parentTransform, false);
#endif

                switch (NotificationType)
                {
                    case PoolItemNotificationType.Interface:
#if UNITY_4_5 || UNITY_4_6
                        Component[] pItems = sceneObject.GetComponents(typeof(IFastPoolItem));
                        for (int i = 0; i < pItems.Length; i++)
                            ((IFastPoolItem)pItems[i]).OnFastDestroy();
#else
                        IFastPoolItem[] pItems = sceneObject.GetComponents<IFastPoolItem>();
                        for (int i = 0; i < pItems.Length; i++)
                            pItems[i].OnFastDestroy();
#endif
                        break;
                    case PoolItemNotificationType.SendMessage:
                        sceneObject.SendMessage("OnFastDestroy");
                        break;
                }

                sceneObject.SetActive(false);
            }
            else
                GameObject.Destroy(sceneObject);
        }
        else
            Debug.LogWarning("Attempt to destroy a null object");
    }
    #endregion




    GameObject MakeClone(Vector3 position, Quaternion rotation, Transform parent)
    {
        GameObject clone = (GameObject)GameObject.Instantiate(sourcePrefab, position, rotation);

#if UNITY_4_5 || UNITY_4_6
        clone.transform.parent = parent;
#else
        clone.transform.SetParent(parent, false);
#endif

        return clone;
    }

}

