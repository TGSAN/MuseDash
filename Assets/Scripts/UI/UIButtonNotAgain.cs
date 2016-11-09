using System.Collections;
using UnityEngine;

public class UIButtonNotAgain : MonoBehaviour
{
    public Collider col;

    private void OnEnable()
    {
        col.enabled = false;
    }

    private void OnDisable()
    {
        col.enabled = true;
    }
}