using UnityEngine;
using System.Collections;

public class AdvancedSpawn : MonoBehaviour
{
    //prefab that will be used as source for clones
    public GameObject sampleGameObject;


    //array where we keep our active clones
    GameObject[] hugeObjectsArray;
    FastPool fastPool;



    void Start()
    {
        hugeObjectsArray = new GameObject[1000];

        //Create a new pool for our sampleGameObject clones and let him to preload 1000 clones
        fastPool = FastPoolManager.CreatePool(sampleGameObject, true, 1000);


        //Or you may don't use the FastPoolManager and create and manage pool by yourself.
        //If you make your FastPool variable public - it will be visible at the inspector 
        //with the same UI as in the FastPoolManager. Look "Without Manager" example for details.
        //
        //fastPool = new FastPool(sampleGameObject, null, true, 1000);
    }



    public void Spawn()
    {
        //lets spawn 1000 objects
        for (int i = 0; i < 1000; i++)
        {
            //Just spawn a clone and remember it. 
            //If pool has cached objects - it will quickly activate it instead of instantiating a new one.
            hugeObjectsArray[i] = fastPool.FastInstantiate();
        }
    }


    public void DestroyObjects()
    {
        //lets despawn our 1000 objects
        for (int i = 0; i < 1000; i++)
        {
            //Cache our clone.
            fastPool.FastDestroy(hugeObjectsArray[i]);
        }
    }





    bool spawned = false;
    void OnGUI()
    {
        if (!spawned)
        {
            if (GUI.Button(new Rect(Screen.width * 0.5f - 75, Screen.height * 0.8f, 150, 50), "Spawn 1000 objects"))
            {
                Spawn();
                spawned = true;
            }
        }
        else
        {
            if (GUI.Button(new Rect(Screen.width * 0.5f - 75, Screen.height * 0.8f, 150, 50), "Destroy 1000 objects"))
            {
                DestroyObjects();
                spawned = false;
            }
        }
    }
}
