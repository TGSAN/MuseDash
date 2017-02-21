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
            var hasRoot = parent.GetComponent<PRHelper>().nodes.Exists(n => n.nodeType == NodeType.Model_CollectionBinding);
            var sourceType = (TextBinding.SourceType)
                    Enum.ToObject(typeof(TextBinding.SourceType), property.FindPropertyRelative("type").enumValueIndex);
            switch (sourceType)
            {
                case TextBinding.SourceType.Json:
                    {
                        rect = EditorUtils.MakePopupField(property, "path", new GUIContent("Json Path"),
                ConfigManager.instance.configs.Select(c => c.path).ToArray(), rect, m_Gap, m_Height);
                        var jdata = ConfigManager.instance.GetFromFilePath(property.FindPropertyRelative("path").stringValue);
                        if (jdata == null) break;
                        var isArray = jdata.IsArray || jdata.Keys.Contains("0") || jdata.Keys.Contains("1");
                        if (!isArray)
                        {
                            rect = EditorUtils.MakePopupField(property, "key", new GUIContent("Json Key"),
                             jdata.Keys.ToArray(), rect, m_Gap, m_Height);
                        }
                        else
                        {
                            var go = property.FindPropertyRelative("sourceObj").objectReferenceValue as GameObject;
                            go = go ?? parent;

                            rect = EditorUtils.MakePopupField(property, "key", new GUIContent("Json Key"),
                         jdata[0].Keys.ToArray(), rect, m_Gap, m_Height);
                            if (!hasRoot)
                            {
                                rect = EditorUtils.MakePropertyField("sourceObj", property, rect, m_Gap, m_Height);
                                if (property.FindPropertyRelative("sourceObj").objectReferenceValue != null)
                                {
                                    rect = EditorUtils.MakeObjectField(go, property, "fieldName", new GUIContent("Index"),
                                        rect,
                                        m_Gap, m_Height);
                                }
                            }
                        }
                    }
                    break;

                case TextBinding.SourceType.Script:
                    {
                        var go = property.FindPropertyRelative("sourceObj").objectReferenceValue as GameObject;
                        go = go ?? parent;

                        rect = EditorUtils.MakePropertyField("sourceObj", property, rect, m_Gap, m_Height);
                        if (property.FindPropertyRelative("sourceObj").objectReferenceValue != null)
                        {
                            rect = EditorUtils.MakeObjectField(go, property, "fieldName", new GUIContent("Member"), rect,
                              m_Gap, m_Height);
                        }
                    }
                    break;

                case TextBinding.SourceType.Enum:
                    {
                        var go = property.FindPropertyRelative("sourceObj").objectReferenceValue as GameObject;
                        go = go ?? parent;
                        rect = EditorUtils.MakePopupField(property, "key", new GUIContent("Enum Key"),
                                ConstanceManager.instance.keys, rect, m_Gap, m_Height, false, null, true);

                        var key = property.FindPropertyRelative("key").stringValue;
                        var obj = ConstanceManager.instance[key];
                        var func = obj as Func<string>;
                        if (func == null)
                        {
                            if (!hasRoot)
                            {
                                rect = EditorUtils.MakePropertyField("sourceObj", property, rect, m_Gap, m_Height);
                                if (property.FindPropertyRelative("sourceObj").objectReferenceValue != null)
                                {
                                    rect = EditorUtils.MakeObjectField(go, property, "fieldName", new GUIContent("Member"), rect,
                                       m_Gap, m_Height);
                                }
                            }
                        }
                    }
                    break;
            }
            rect = EditorUtils.MakePropertyField("value", property, rect, m_Gap, m_Height);
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            var parent = Selection.activeGameObject;
            var hasRoot = parent.GetComponent<PRHelper>().nodes.Exists(n => n.nodeType == NodeType.Model_CollectionBinding);
            var sourceType = (TextBinding.SourceType)
                       Enum.ToObject(typeof(TextBinding.SourceType), property.FindPropertyRelative("type").enumValueIndex);
            var extra = 0;
            switch (sourceType)
            {
                case TextBinding.SourceType.Json:
                    {
                        extra = 40;
                        var jdata = ConfigManager.instance.GetFromFilePath(property.FindPropertyRelative("path").stringValue);
                        if (jdata != null)
                        {
                            var isArray = jdata.IsArray || jdata.Keys.Contains("0") || jdata.Keys.Contains("1");
                            extra = isArray ? 80 : 40;
                            extra = property.FindPropertyRelative("sourceObj").objectReferenceValue != null
                                ? extra + 20
                                : extra;
                        }
                        if (hasRoot)
                        {
                            extra = 60;
                        }
                    }
                    break;

                case TextBinding.SourceType.Script:
                    {
                        extra = property.FindPropertyRelative("sourceObj").objectReferenceValue != null ? 60 : 40;
                    }
                    break;

                case TextBinding.SourceType.Enum:
                    {
                        var key = property.FindPropertyRelative("key").stringValue;
                        var obj = ConstanceManager.instance[key];
                        var func = obj as Func<string>;
                        if (func == null)
                        {
                            extra = property.FindPropertyRelative("sourceObj").objectReferenceValue != null ? 80 : 60;
                        }
                        else
                        {
                            extra = 20;
                        }
                        if (hasRoot)
                        {
                            extra = 40;
                        }
                    }
                    break;
            }

            return 40 + extra;
        }
    }
}