using UnityEngine;
using System.Collections;

public class MyNotifiedObject : MonoBehaviour, IFastPoolItem
{

    // Use this for initialization
    void Start()
    {
        name = "I'm Instantiated!";
    }

    // Update is called once per frame
    void Update()
    { }


    //If you select notification via Interface, you must inherit from the IFastPoolItem interface and implement it.
    //If you select notification via SendMessage or BroadcastMessage - just implement methods below.

    //This method will be called when object FastInstantiated
    public void OnFastInstantiate()
    {
        name = "I'm spawned!";
    }


    //This method will be called when object FastDestroyed
    public void OnFastDestroy()
    {
        name = "I'm cached...";
    }
}
