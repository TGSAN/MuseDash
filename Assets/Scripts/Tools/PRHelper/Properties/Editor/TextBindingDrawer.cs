using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Assets.Scripts.Tools.Commons;
using Assets.Scripts.Tools.Managers;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.Tools.PRHelper.Properties.Editor
{
    [CustomPropertyDrawer(typeof(TextBinding))]
    public class TextBindingDrawer : PropertyDrawer
    {
        private float m_Height = 15;
        private float m_Gap = 5;

        public override void OnGUI(Rect rect, SerializedProperty property, GUIContent label)
        {
            var parent = Selection.activeGameObject;
            var txts = new List<Text>();
            var txt = parent.GetComponent<Text>();
            if (txt != null) txts.Add(txt);

            var childTxts = parent.GetComponentsInChildren<Text>();
            childTxts.ToList().ForEach(txts.Add);

            rect = EditorUtils.MakePopupField(property, "name", new GUIContent("Text Name"),
                childTxts.Select(c => c.name).ToArray(), rect, m_Gap, m_Height, false, null, false, "None");
            rect = EditorUtils.MakePopupField(property, "type", new GUIContent("Source Type"),
                Enum.GetNames(typeof(TextBinding.SourceType)), rect, m_Gap, m_Height, true);

            var sourceType =
                (TextBinding.SourceType)
                    Enum.ToObject(typeof(TextBinding.SourceType), property.FindPropertyRelative("type").enumValueIndex);
            switch (sourceType)
            {
                case TextBinding.SourceType.Json:
                    {
                        rect = EditorUtils.MakePopupField(property, "path", new GUIContent("Json Path"),
                ConfigManager.instance.configs.Select(c => c.path).ToArray(), rect, m_Gap, m_Height);
                        var fileName = ConfigManager.instance.configs.Find(c => c.path == property.FindPropertyRelative("path").stringValue).fileName;
                        var jdata = ConfigManager.instance[fileName];
                        if (jdata == null) break;
                        var isArray = jdata.IsArray || jdata.Keys.Contains("0") || jdata.Keys.Contains("1");
                        if (!isArray)
                        {
                            rect = EditorUtils.MakePopupField(property, "key", new GUIContent("Json Key"),
                             jdata.Keys.ToArray(), rect, m_Gap, m_Height);
                            property.FindPropertyRelative("value").stringValue =
                                (string)jdata[property.FindPropertyRelative("key").stringValue];
                        }
                        else
                        {
                            var value = property.FindPropertyRelative("value");
                            var key = property.FindPropertyRelative("key");
                            var go = property.FindPropertyRelative("sourceObj").objectReferenceValue as GameObject;
                            go = go ?? parent;

                            var originRect = rect;
                            rect = new Rect(150, rect.y, 100, rect.height);
                            rect = EditorUtils.MakePropertyField("sourceObj", property, rect, m_Gap, m_Height, new GUIContent(string.Empty));
                            rect = originRect;
                            rect = EditorUtils.MakeObjectField(go, property, "fieldName", new GUIContent("Index"), rect,
                                   m_Gap, m_Height);
                            rect = EditorUtils.MakePopupField(property, "key", new GUIContent("Json Key"),
                         jdata[0].Keys.ToArray(), rect, m_Gap, m_Height);
                            var fieldName = property.FindPropertyRelative("fieldName").stringValue;
                            var strs = fieldName.Split('/');
                            UnityEngine.Object obj = go;
                            if (strs[0] != "GameObject")
                            {
                                obj = go.GetComponent(strs[0]);
                            }
                            var type = obj.GetType();
                            var field = type.GetField(strs[strs.Length - 1], BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);
                            if (field != null)
                            {
                                var index = field.GetValue(obj);

                                if (jdata.IsArray)
                                {
                                    var i = int.Parse(index.ToString());
                                    if (jdata.Count < i)
                                    {
                                        value.stringValue = (string)jdata[i][key.stringValue];
                                    }
                                }
                                else
                                {
                                    var i = index.ToString();
                                    if (jdata.Keys.Contains(i))
                                    {
                                        value.stringValue = (string)jdata[i][key.stringValue];
                                    }
                                }
                            }
                        }
                    }
                    break;

                case TextBinding.SourceType.Script:
                    {
                        var go = property.FindPropertyRelative("sourceObj").objectReferenceValue as GameObject;
                        go = go ?? parent;
                        var value = property.FindPropertyRelative("value");

                        var originRect = rect;
                        rect = new Rect(150, rect.y, 100, rect.height);
                        rect = EditorUtils.MakePropertyField("sourceObj", property, rect, m_Gap, m_Height, new GUIContent(string.Empty));
                        rect = originRect;
                        rect = EditorUtils.MakeObjectField(go, property, "fieldName", new GUIContent("Member"), rect,
                               m_Gap, m_Height);
                        var fieldName = property.FindPropertyRelative("fieldName").stringValue;
                        var strs = fieldName.Split('/');
                        UnityEngine.Object obj = go;
                        if (strs[0] != "GameObject")
                        {
                            obj = go.GetComponent(strs[0]);
                        }
                        var type = obj.GetType();
                        var field = type.GetField(strs[strs.Length - 1], BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);
                        if (field != null)
                        {
                            var index = field.GetValue(obj);
                            value.stringValue = index.ToString();
                        }
                        else
                        {
                            var ppt = type.GetProperty(strs[strs.Length - 1], BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);
                            var index = ppt.GetValue(obj, null);
                            value.stringValue = index.ToString();
                        }
                    }
                    break;

                case TextBinding.SourceType.Enum:
                    {
                        var go = property.FindPropertyRelative("sourceObj").objectReferenceValue as GameObject;
                        go = go ?? parent;
                        var value = property.FindPropertyRelative("value");

                        rect = EditorUtils.MakePopupField(property, "key", new GUIContent("Enum Key"),
                                ConstanceManager.instance.keys, rect, m_Gap, m_Height, false, null, true);
                        var key = property.FindPropertyRelative("key").stringValue;
                        var obj = ConstanceManager.instance[key];
                        var func = obj as Func<string>;
                        if (func == null)
                        {
                            var funcParam = obj as Func<object, string>;
                            var originRect = rect;
                            rect = new Rect(150, rect.y, 100, rect.height);
                            rect = EditorUtils.MakePropertyField("sourceObj", property, rect, m_Gap, m_Height, new GUIContent(string.Empty));
                            rect = originRect;
                            rect = EditorUtils.MakeObjectField(go, property, "fieldName", new GUIContent("Member"), rect,
                                   m_Gap, m_Height);
                            var fieldName = property.FindPropertyRelative("fieldName").stringValue;
                            var strs = fieldName.Split('/');
                            UnityEngine.Object bindingObj = go;
                            if (strs[0] != "GameObject")
                            {
                                bindingObj = go.GetComponent(strs[0]);
                            }
                            var type = bindingObj.GetType();
                            var field = type.GetField(strs[strs.Length - 1], BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);
                            if (field != null)
                            {
                                var index = field.GetValue(bindingObj);
                                if (funcParam != null) value.stringValue = funcParam(index);
                            }
                        }
                        else
                        {
                            value.stringValue = func();
                        }
                    }
                    break;
            }
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            var sourceType = (TextBinding.SourceType)
                       Enum.ToObject(typeof(TextBinding.SourceType), property.FindPropertyRelative("type").enumValueIndex);
            var extra = 0;
            switch (sourceType)
            {
                case TextBinding.SourceType.Json:
                    {
                        extra = 20;
                        var fileName = ConfigManager.instance.configs.Find(c => c.path == property.FindPropertyRelative("path").stringValue).fileName;
                        var jdata = ConfigManager.instance[fileName];
                        if (jdata != null)
                        {
                            var isArray = jdata.IsArray || jdata.Keys.Contains("0") || jdata.Keys.Contains("1");
                            extra = isArray ? 60 : 40;
                        }
                    }
                    break;

                case TextBinding.SourceType.Script:
                    {
                        extra = 25;
                    }
                    break;

                case TextBinding.SourceType.Enum:
                    {
                        var key = property.FindPropertyRelative("key").stringValue;
                        var obj = ConstanceManager.instance[key];
                        var func = obj as Func<string>;
                        if (func == null)
                        {
                            extra = 45;
                        }
                        else
                        {
                            extra = 25;
                        }
                    }
                    break;
            }

            return 40 + extra;
        }
    }
}