using System;
using Assets.Scripts.Common;
using UnityEngine;

namespace Assets.Scripts.Tools.PRHelper.Properties
{
    [Serializable]
    public class CollectionBinding
    {
        public string index;
        public UnityEngine.Object sourceObj;
        public string reflectName;

        public void Play(GameObject go)
        {
            index = sourceObj != null ? ReflectionUtil.Reflect(sourceObj, reflectName) : index;
        }
    }
}