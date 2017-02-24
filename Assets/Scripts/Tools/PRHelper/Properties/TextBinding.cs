using System;
using System.Linq;
using System.Reflection;
using Assets.Scripts.Common;
using Assets.Scripts.Tools.Managers;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.Tools.PRHelper.Properties
{
    [Serializable]
    public class TextBinding
    {
        public string name;
        public SourceType type;
        public UnityEngine.Object sourceObj;
        public string reflectName;

        public string path;
        public string key;
        public string index;

        private const string tmpStr = "%s";
        public string value = "%s";

        private Text m_Txt;

        public void Play(GameObject go)
        {
            if (!m_Txt)
            {
                m_Txt = go.GetComponentsInChildren<Text>().ToList().Find(t => t.gameObject.name == name);
                var cBNode = go.GetComponent<PRHelper>().nodes.Find(n => n.nodeType == NodeType.Model_CollectionBinding);
                index = cBNode != null ? cBNode.collectionBinding.index : (sourceObj != null ? ReflectionUtil.Reflect(sourceObj, reflectName) : index);
            }

            var str = string.Empty;
            switch (type)
            {
                case SourceType.Json:
                    {
                        var jdata = ConfigManager.instance.Convert(path);
                        if (jdata.IsArray)
                        {
                            var i = int.Parse(index);
                            if (jdata.Count < i)
                            {
                                str = ConfigManager.instance.GetConfigStringValue(ConfigManager.instance.GetFileName(path),
                i, key);
                            }
                        }
                        else
                        {
                            var i = index.ToString();
                            if (jdata.Keys.Contains(i))
                            {
                                str = jdata[i][key].ToString();
                            }
                        }
                    }
                    break;

                case SourceType.Script:
                    {
                        str = index;
                    }
                    break;

                case SourceType.Enum:
                    {
                        var obj = CallbackManager.instance[key];
                        var func = obj as Func<string>;
                        if (func == null)
                        {
                            var funcParam = obj as Func<object, string>;
                            if (funcParam != null) str = funcParam(index);
                        }
                        else
                        {
                            str = func();
                        }
                    }
                    break;
            }
            var strVale = value.Replace(tmpStr, str);
            if (m_Txt)
            {
                m_Txt.text = strVale;
            }
        }

        public enum SourceType
        {
            Json,
            Enum,
            Script,
        }
    }
}