using UnityEngine;
using System.Collections;
using System.Collections.Generic;


public class FastPoolBenchmark : MonoBehaviour
{
    public GUIText Results;
    public GameObject sourcePrefab;

    GameObject[] spawnedObjects;
    System.Diagnostics.Stopwatch sw;

    int times = 1000;


    void RunFPBenchmark()
    {
        long testA, testB;

        if (spawnedObjects == null)
            spawnedObjects = new GameObject[times];


        sw = new System.Diagnostics.Stopwatch();

        sw.Reset();
        sw.Start();
        for (int i = 0; i < times; i++)
            spawnedObjects[i] = FastPoolManager.GetPool(sourcePrefab, true).FastInstantiate();
        sw.Stop();
        testA = sw.ElapsedMilliseconds;


        sw.Reset();
        sw.Start();
        for (int i = 0; i < times; i++)
            FastPoolManager.GetPool(sourcePrefab, false).FastDestroy(spawnedObjects[i]);
        sw.Stop();
        testB = sw.ElapsedMilliseconds;

        Results.text = string.Format("FastInstantiating 1000 cubes: {0}ms\r\nFastDestroying 1000 cubes: {1}ms", testA, testB);
    }

    void RunGenericBenchmark()
    {
        long testA, testB;

        if (spawnedObjects == null)
            spawnedObjects = new GameObject[times];


        sw = new System.Diagnostics.Stopwatch();

        sw.Reset();
        sw.Start();
        for (int i = 0; i < times; i++)
            spawnedObjects[i] = (GameObject)GameObject.Instantiate(sourcePrefab);
        sw.Stop();
        testA = sw.ElapsedMilliseconds;


        sw.Reset();
        sw.Start();
        for (int i = 0; i < times; i++)
            Destroy(spawnedObjects[i]);
        sw.Stop();
        testB = sw.ElapsedMilliseconds;

        Results.text = string.Format("Unity Instantiating 1000 cubes: {0}ms\r\nUnity Destroying 1000 cubes: {1}ms", testA, testB);
    }

    void OnGUI()
    {
        if (GUI.Button(new Rect(Screen.width / 2 - 50, Screen.height / 2 + 50, 100, 30), "Unity Test"))
            RunGenericBenchmark();

        if (GUI.Button(new Rect(Screen.width / 2 - 50, Screen.height / 2 + 85, 100, 30), "FastPool Test"))
            RunFPBenchmark();
            
    }
}
