using System;
using System.Linq;
using System.Reflection;
using Assets.Scripts.Common;
using Assets.Scripts.Tools.Commons;
using Assets.Scripts.Tools.Managers;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.Tools.PRHelper.Properties
{
    [Serializable]
    public class ImageBinding
    {
        public string name;
        public UnityEngine.Object sourceObj;
        public string fieldName;

        public string path;
        public string key;
        public string index;

        private Image m_Image;
        private string m_ResourcePath;

        public void Play(GameObject go)
        {
            var cBNode = go.GetComponent<PRHelper>().nodes.Find(n => n.nodeType == NodeType.Model_CollectionBinding);
            index = cBNode != null ? cBNode.collectionBinding.index : (sourceObj != null ? ReflectionUtil.Reflect(sourceObj, fieldName) : index);

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

            if (!m_Image)
            {
                m_Image = go.GetComponentsInChildren<Image>().ToList().Find(t => t.gameObject.name == name);
            }

            var spr = ResourcesLoader.Load<Sprite>(m_ResourcePath);
            if (spr)
            {
                m_Image.sprite = spr;
            }
        }
    }
}