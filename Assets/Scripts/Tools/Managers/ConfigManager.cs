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

        private readonly Dictionary<string, JsonData> m_Dictionary = new Dictionary<string, JsonData>();

        public JsonData this[string idx]
        {
            get
            {
                if (m_Dictionary.ContainsKey(idx))
                {
                    return m_Dictionary[idx];
                }
                var path = StringUtils.BeginBefore(configs.Find(c => c.fileName == idx).path, '.');
                var txt = ResourcesLoader.Load<TextAsset>(path);
                if (txt != null)
                {
                    var data = txt.text;
                    m_Dictionary.Add(idx, JsonMapper.ToObject(data));
                    //ResourcesLoader.Unload(txt);
                    return JsonMapper.ToObject(data);
                }
                Debug.Log(idx + "json not found");
                return null;
            }
        }

        public string GetFileName(string path)
        {
            return ConfigManager.instance.configs.Find(c => c.path == path).fileName;
        }

        public JsonData Convert(string path)
        {
            return this[GetFileName(path)];
        }

        public new static ConfigManager instance
        {
            get
            {
                m_Instance = m_Instance ?? (m_Instance = ResourcesLoader.Load<ConfigManager>(StringCommons.ConfigManagerInResources));
                return m_Instance;
            }
        }

        public int GetConfigIntValue(string fileName, int index, string key)
        {
            return int.Parse(GetConfigStringValue(fileName, index, key));
        }

        public int GetConfigIntValue(string fileName, string cmpKey, string targetKey, object cmpValue)
        {
            return int.Parse(GetConfigStringValue(fileName, cmpKey, targetKey, cmpValue));
        }

        public string GetConfigStringValue(string fileName, int index, string key)
        {
            //Debug.Log(fileName + "==" + index + "===" + key);
            var jData = this[fileName][index][key];
            var value = jData.ToString();
            return value;
        }

        public string GetConfigStringValue(string fileName, string cmpKey, string targetKey, object cmpValue)
        {
            var jData = GetConfigValue(fileName, cmpKey, targetKey, cmpValue);
            var value = jData.ToJson().Replace("\"", string.Empty);
            return value;
        }

        public float GetConfigFloatValue(string fileName, int index, string key)
        {
            return float.Parse(GetConfigStringValue(fileName, index, key));
        }

        public float GetConfigFloatValue(string fileName, string cmpKey, string targetKey, object cmpValue)
        {
            return float.Parse(GetConfigStringValue(fileName, cmpKey, targetKey, cmpValue));
        }

        private JsonData GetConfigValue(string fileName, int index, string key)
        {
            return this[fileName][index][key];
        }

        private JsonData GetConfigValue(string fileName, string cmpKey, string targetKey, object cmpValue)
        {
            var jd = this[fileName];
            for (int i = 0; i < jd.Count; i++)
            {
                var jsonData = jd[i];
                var data = jsonData[cmpKey];
                var value = data.ToJson().Replace("\"", string.Empty);
                if (value == cmpValue.ToString())
                {
                    return jsonData[targetKey];
                }
            }

            return null;
        }

        [Serializable]
        public class FileData
        {
            public string fileName;
            public string path;
        }
    }
}