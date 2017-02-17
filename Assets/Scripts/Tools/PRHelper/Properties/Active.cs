using System;
using UnityEngine;

namespace Assets.Scripts.Tools.PRHelper.Properties
{
    [Serializable]
    public class Active
    {
        public GameObject gameObject;
        public bool isActive;

        public void Play(GameObject go)
        {
            gameObject.SetActive(isActive);
        }
    }
}