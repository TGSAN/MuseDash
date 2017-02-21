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
        public string fieldName;

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
            }

            var collectionBindingNode = go.GetComponent<PRHelper>().nodes.Find(n => n.nodeType == NodeType.Model_CollectionBinding);
            index = collectionBindingNode != null ? collectionBindingNode.collectionBinding.index : ReflectionUtil.Reflect(sourceObj, fieldName);

            var str = string.Empty;
            switch (type)
            {
                case SourceType.Json:
                    {
                        var jdata = ConfigManager.instance.GetFromFilePath(path);
                        if (jdata.IsArray)
                        {
                            var i = int.Parse(index.ToString());
                            if (jdata.Count < i)
                            {
                                str = (string)jdata[i][key];
                            }
                        }
                        else
                        {
                            var i = index.ToString();
                            if (jdata.Keys.Contains(i))
                            {
                                str = (string)jdata[i][key];
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
                        var obj = ConstanceManager.instance[key];
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