using System;
using Assets.Scripts.Common;
using UnityEngine;

namespace Assets.Scripts.Tools.PRHelper.Properties
{
    [Serializable]
    public class CollectionBinding
    {
        public string index;
        public ReflectObject reflectObj;

        public void Play(GameObject go)
        {
            index = reflectObj.sourceObj != null ? ReflectionUtil.Reflect(reflectObj) : index;
        }
    }
}