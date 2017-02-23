using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using Assets.Scripts.Common;
using Assets.Scripts.Tools.Commons;
using LitJson;

using UnityEngine;

namespace Assets.Scripts.Tools.Managers
{
    public class ConfigManager : SingletonScriptObject<ConfigManager>
    {
        [SerializeField]
        public List<FileData> configs;

        private readonly Dictionary<string, string> m_Dictionary = new Dictionary<string, string>();

        public JsonData this[string idx]
        {
            get
            {
                if (m_Dictionary.ContainsKey(idx))
                {
                    return JsonMapper.ToObject(m_Dictionary[idx]);
                }

                var path = StringUtils.BeginBefore(configs.Find(c => c.fileName == idx).path, '.');
                var txt = ResourcesLoader.Load<TextAsset>(path);
                if (txt != null)
                {
                    var data = txt.text;
                    ResourcesLoader.Unload(txt);
                    m_Dictionary.Add(idx, data);
                    return JsonMapper.ToObject(data);
                }
                Debug.Log(idx + "json not found");
                return null;
            }
        }

        public JsonData Convert(string path)
        {
            var fileName = ConfigManager.instance.configs.Find(c => c.path == path).fileName;
            return this[fileName];
        }

        public new static ConfigManager instance
        {
            get
            {
                m_Instance = m_Instance ?? (m_Instance = ResourcesLoader.Load<ConfigManager>(StringCommons.ConfigManagerInResources));
                return m_Instance;
            }
        }

        [Serializable]
        public class FileData
        {
            public string fileName;
            public string path;
        }
    }
}