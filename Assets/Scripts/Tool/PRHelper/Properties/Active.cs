using System;
using UnityEngine;

namespace Assets.Scripts.Tool.PRHelper.Properties
{
    [Serializable]
    public class Active
    {
        public GameObject go;
        public bool isActive;

        public void Play()
        {
            Debug.Log("Set Active");
            go.SetActive(isActive);
        }
    }
}