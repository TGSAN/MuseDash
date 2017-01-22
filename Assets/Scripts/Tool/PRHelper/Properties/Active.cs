using System;
using System.Collections.Generic;
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
            go.SetActive(isActive);
        }
    }
}