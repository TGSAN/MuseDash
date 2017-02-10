using System;
using UnityEngine;

namespace Assets.Scripts.Tools.PRHelper.Properties
{
    [Serializable]
    public class Active
    {
        public GameObject go;
        public bool isActive;

        public void Play()
        {
            go.SetActive(isActive);
        }
    }
}