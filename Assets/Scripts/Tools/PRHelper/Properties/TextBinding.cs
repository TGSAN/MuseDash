using System;
using System.Collections.Generic;
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
        public TextData[] datas;
        private const string tmpStr = "%s";
        public string value = "%s";

        private Text m_Txt;

        public void Play(GameObject go)
        {
            if (!m_Txt)
            {
                m_Txt = go.GetComponentsInChildren<Text>().ToList().Find(t => t.gameObject.name == name);
            }
            var strs = new List<string>();

            for (int i = 0; i < datas.Length; i++)
            {
                var textData = datas[i];
                var cBNode = go.GetComponent<PRHelper>().nodes.Find(n => n.nodeType == NodeType.Model_CollectionBinding);
                textData.index = cBNode != null ? cBNode.collectionBinding.index : (textData.reflectObj.sourceObj != null ? ReflectionUtil.Reflect(textData.reflectObj) : textData.index);
                var s = string.Empty;
                switch (textData.type)
                {
                    case SourceType.Json:
                        {
                            var jdata = ConfigManager.instance.Convert(textData.path);
                            if (jdata.IsArray)
                            {
                                var idx = int.Parse(textData.index);
                                if (i < jdata.Count)
                                {
                                    s = ConfigManager.instance.GetConfigStringValue(ConfigManager.instance.GetFileName(textData.path),
                    idx, textData.key);
                                }
                            }
                            else
                            {
                                var idx = textData.index.ToString();
                                if (jdata.Keys.Contains(idx))
                                {
                                    s = jdata[i][textData.key].ToString();
                                }
                            }
                        }
                        break;

                    case SourceType.Script:
                        {
                            s = textData.index;
                        }
                        break;

                    case SourceType.Enum:
                        {
                            var obj = CallbackManager.instance[textData.key];
                            var func = obj as Func<string>;
                            if (func == null)
                            {
                                var funcParam = obj as Func<object, string>;
                                if (funcParam != null) s = funcParam(textData.index);
                            }
                            else
                            {
                                s = func();
                            }
                        }
                        break;
                }

                strs.Add(s);
            }

            var outValue = string.Empty;
            for (int i = 0; i < strs.Count; i++)
            {
                var s = strs[i];
                var index = value.IndexOf(tmpStr, StringComparison.Ordinal);
                Debug.Log(index);
                var thisStr = value.Substring(0, index + tmpStr.Length - 1);
                if (i < strs.Count - 1)
                {
                    value = value.Substring(index + tmpStr.Length - 1, value.Length - thisStr.Length - 1);
                }
                outValue += thisStr.Replace("%s", s);
            }
            if (m_Txt)
            {
                m_Txt.text = outValue;
            }
        }

        public enum SourceType
        {
            Json,
            Enum,
            Script,
        }

        [Serializable]
        public class TextData
        {
            public SourceType type;
            public ReflectObject reflectObj;
            public string path;
            public string key;
            public string index;
        }
    }
}