using System;
using UnityEngine;

public class DemoGlobalValues : MonoBehaviour
{
    public float SomeFloat;

    public void IncSomeFloat()
    {
        if (SomeFloat < 7) SomeFloat++;
    }

    public void DecSomeFloat()
    {
        if (SomeFloat > 0) SomeFloat--;
    }

    public string TimeString
    {
        get { return DateTime.Now.Millisecond.ToString("000"); }
    }

    public DemoCollectionItem[] SomeCollection
    {
        get
        {
            return GetComponentsInChildren<DemoCollectionItem>();
        }
    }
}