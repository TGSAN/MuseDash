using UnityEngine;
using System.Collections;

public class CustomPoolIDSpawn : MonoBehaviour
{
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
            //First of all we need to get the pool for our custom pool ID.
            //If pool for this id does not exist and you want it will be created automatically - then provide the sourcePrefab 
            //and set "createIfNotExists" to true.
            //Otherwise you can set prefab parameter as null (in this case "createIfNotExists" must be false)
            FastPool fastPool = FastPoolManager.GetPool(1, null, false);
            
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
            //Get the pool for our custom pool ID.
            //If pool for this id does not exist and you want it will be created automatically - then provide the sourcePrefab 
            //and set "createIfNotExists" to true.
            //Otherwise you can set prefab parameter as null (in this case "createIfNotExists" must be false)
            FastPool fastPool = FastPoolManager.GetPool(1, null, false);
            
            //Cache our clone.
            fastPool.FastDestroy(hugeObjectsArray[i]);


            //Or you can do it all in one line:
            //  FastPoolManager.GetPool(1, null, false).FastDestroy(hugeObjectsArray[i]);
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
