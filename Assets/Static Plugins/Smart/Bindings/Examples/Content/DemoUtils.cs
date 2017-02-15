using Smart.Extensions;
using UnityEngine;

public class DemoUtils : MonoBehaviour
{
    public void InstantiateHere(GameObject prefab)
    {
        Instantiate(prefab).Reparent(this);
    }
}
