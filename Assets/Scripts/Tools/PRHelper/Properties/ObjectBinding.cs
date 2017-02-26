using System;
using System.Linq;
using System.Reflection;
using Assets.Scripts.Common;
using Assets.Scripts.Tools.Commons;
using Assets.Scripts.Tools.Managers;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Assets.Scripts.Tools.PRHelper.Properties
{
    [Serializable]
    public class ObjectBinding
    {
        public string name;
        public ReflectObject reflectObj;

        public string path;
        public string key;
        public string index;

        private GameObject m_Obj;
        private string m_ResourcePath;

        public void Play(GameObject go)
        {
            var cBNode = go.GetComponent<PRHelper>().nodes.Find(n => n.nodeType == NodeType.Model_CollectionBinding);
            index = cBNode != null ? cBNode.collectionBinding.index : (reflectObj.sourceObj != null ? ReflectionUtil.Reflect(reflectObj) : index);

            var resourcePath = ConfigManager.instance.GetConfigStringValue(ConfigManager.instance.GetFileName(path),
                int.Parse(index), key);
            if (m_ResourcePath == resourcePath)
            {
                return;
            }
            else
            {
                m_ResourcePath = resourcePath;
            }

            if (m_Obj)
            {
                Object.Destroy(m_Obj);
            }
            var parent = go.GetComponentsInChildren<Transform>().ToList().Find(t => t.gameObject.name == name);
            if (parent)
            {
                m_Obj = Object.Instantiate(ResourcesLoader.Load<GameObject>(m_ResourcePath)) as GameObject;
                m_Obj.SetActive(true);
                if (m_Obj)
                {
                    m_Obj.transform.SetParent(parent, false);
                    m_Obj.transform.localPosition = Vector3.zero;
                }
            }
        }
    }
}