using System;
using System.Linq;
using Assets.Scripts.Common;
using Assets.Scripts.Tools.Commons;
using Assets.Scripts.Tools.Managers;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Assets.Scripts.Tools.PRHelper.Properties
{
    [Serializable]
    public class AudioBinding
    {
        public string name;
        public ReflectObject reflectObj;

        public string path;
        public string key;
        public string index;
        private string m_ResourcePath;
        private AudioSource m_AudioSource;

        public bool isAutoPlay;

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
            m_ResourcePath = resourcePath;
            if (!m_AudioSource)
            {
                m_AudioSource = go.GetComponentsInChildren<AudioSource>().ToList().Find(t => t.gameObject.name == name);
            }

            var clip = ResourcesLoader.Load<AudioClip>(m_ResourcePath);
            if (clip)
            {
                m_AudioSource.clip = clip;
                if (isAutoPlay)
                {
                    m_AudioSource.Play();
                }
            }
        }
    }
}