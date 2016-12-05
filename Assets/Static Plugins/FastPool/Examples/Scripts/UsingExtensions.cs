using UnityEngine;
using System.Collections;

public class UsingExtensions : MonoBehaviour
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

            //Just call FastInstantiate method on your source prefab
            //and all work will be done automatically (get/create pool, etc.)
            //Note that you must always call this method on the SOURCE game object and NOT on a clone!
            hugeObjectsArray[i] = sampleGameObject.FastInstantiate();
        }
    }


    public void DestroyObjects()
    {
        //lets despawn our 1000 objects
        for (int i = 0; i < 1000; i++)
        {
            //Cache our clone to the sampleGameObject's pool.
            hugeObjectsArray[i].FastDestroy(sampleGameObject);
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
