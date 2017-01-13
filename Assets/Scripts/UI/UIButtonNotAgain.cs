using System.Collections;
using UnityEngine;

public class UIButtonNotAgain : MonoBehaviour
{
    public Collider col;

    private void OnEnable()
    {
        DOTweenUtils.Delay(() =>
        {
            col.enabled = false;
        }, Time.deltaTime);
    }

    private void OnDisable()
    {
        col.enabled = true;
    }
}