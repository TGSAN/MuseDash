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
        public string fieldName;

        public void Play(GameObject go)
        {
            if (string.IsNullOrEmpty(index))
            {
                var cBNode = go.GetComponent<PRHelper>().nodes.Find(n => n.nodeType == NodeType.Model_CollectionBinding);
                index = cBNode != null ? cBNode.collectionBinding.index : (sourceObj != null ? ReflectionUtil.Reflect(sourceObj, fieldName) : index);
            }
        }
    }
}