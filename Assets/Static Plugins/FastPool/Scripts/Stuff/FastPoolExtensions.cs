using UnityEngine;
using System.Collections;

public static class FastPoolExtensions
{
    public static GameObject FastInstantiate(this GameObject sourcePrefab, Transform parentTransform = null)
    {
        return FastPoolManager.GetPool(sourcePrefab, true).FastInstantiate(parentTransform);
    }
    public static GameObject FastInstantiate(this GameObject sourcePrefab, Vector3 position, Quaternion rotation, Transform parentTransform = null)
    {
        return FastPoolManager.GetPool(sourcePrefab, true).FastInstantiate(position, rotation, parentTransform);
    }
    public static GameObject FastInstantiate(this GameObject sourcePrefab, int poolID, Transform parentTransform = null)
    {
        return FastPoolManager.GetPool(poolID, sourcePrefab, true).FastInstantiate(parentTransform);
    }
    public static GameObject FastInstantiate(this GameObject sourcePrefab, int poolID, Vector3 position, Quaternion rotation, Transform parentTransform = null)
    {
        return FastPoolManager.GetPool(poolID, sourcePrefab, true).FastInstantiate(position, rotation, parentTransform);
    }


    public static void FastDestroy(this GameObject objectToDestroy, GameObject sourcePrefab)
    {
        FastPoolManager.GetPool(sourcePrefab, true).FastDestroy(objectToDestroy);
    }
    public static void FastDestroy(this GameObject objectToDestroy, Component sourcePrefab)
    {
        FastPoolManager.GetPool(sourcePrefab, true).FastDestroy(objectToDestroy);
    }
    public static void FastDestroy(this GameObject objectToDestroy, FastPool pool)
    {
        pool.FastDestroy(objectToDestroy);
    }
    public static void FastDestroy(this GameObject objectToDestroy, int poolID)
    {
        FastPoolManager.GetPool(poolID, null, false).FastDestroy(objectToDestroy);
    }
}
