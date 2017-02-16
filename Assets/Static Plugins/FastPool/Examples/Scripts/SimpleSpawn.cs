using UnityEngine;
using System.Collections;

public class SimpleSpawn : MonoBehaviour
{
    //prefab that will be used as source for clones
    public GameObject sampleGameObject;


    //array where we keep our active clones
    GameObject[] hugeObjectsArray;



    void Start()
    {
        hugeObjectsArray = new GameObject[1000];
    }



    public void Spawn()
    {
        //lets spawn 1000 objects
        for (int i = 0; i < 1000; i++)
        {
            //First of all we need to get the pool for our source game object.
            //If it's not exists - it will be created automatically with default settings.
            //Note that you must always provide the SOURCE game object and NOT a clone!
            FastPool fastPool = FastPoolManager.GetPool(sampleGameObject);
            
            //Just spawn a clone and remember it. 
            //If pool has cached objects - it will quickly activate it instead of instantiating a new one.
            hugeObjectsArray[i] = fastPool.FastInstantiate();


            //Or you can do it all in one line:
            //  hugeObjectsArray[i] = FastPoolManager.GetPool(sampleGameObject).FastInstantiate();
        }
    }


    public void DestroyObjects()
    {
        //lets despawn our 1000 objects
        for (int i = 0; i < 1000; i++)
        {
            //Get the pool for our source game object.
            //If it's not exists - it will be created automatically with default settings.
            //Note that you must always provide the SOURCE game object and NOT a clone!
            FastPool fastPool = FastPoolManager.GetPool(sampleGameObject);
            
            //Cache our clone.
            fastPool.FastDestroy(hugeObjectsArray[i]);


            //Or you can do it all in one line:
            //  FastPoolManager.GetPool(sampleGameObject).FastDestroy(hugeObjectsArray[i]);
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
