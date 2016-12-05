using UnityEngine;
using System.Collections;

public class NoManagerSpawn : MonoBehaviour
{
    //our pool
    public FastPool fastPool;
    

    //array where we keep our active clones
    GameObject[] hugeObjectsArray;


    void Start()
    {
        hugeObjectsArray = new GameObject[1000];

        //Init our pool with the settings provided via inspector.
        //If you don't want to make your pool variable public, you
        //can create a new pool with needed parameters like this:
        //fastPool = new FastPool(.....);
        //in this case you don't need to call Init.
        fastPool.Init(transform);
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
