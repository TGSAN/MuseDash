using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Assets.Scripts.Tools.Commons;
using Assets.Scripts.Tools.Commons.Editor;
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
            var datasPpt = property.FindPropertyRelative("datas");
            for (int i = 0; i < datasPpt.arraySize; i++)
            {
                var ppt = datasPpt.GetArrayElementAtIndex(i);
                rect = EditorUtils.MakePopupField(ppt, "type", new GUIContent("Source Type"),
               Enum.GetNames(typeof(TextBinding.SourceType)), rect, m_Gap, m_Height, true);
                var hasRoot = parent.GetComponent<PRHelper>().nodes.Exists(n => n.nodeType == NodeType.Model_CollectionBinding);
                var sourceType = (TextBinding.SourceType)
                        Enum.ToObject(typeof(TextBinding.SourceType), ppt.FindPropertyRelative("type").enumValueIndex);
                var sourceObjPpt = ppt.FindPropertyRelative("reflectObj").FindPropertyRelative("sourceObj");
                switch (sourceType)
                {
                    case TextBinding.SourceType.Json:
                        {
                            rect = EditorUtils.MakePopupField(ppt, "path", new GUIContent("Json Path"),
                    ConfigManager.instance.configs.Select(c => c.path).ToArray(), rect, m_Gap, m_Height);
                            var jdata = ConfigManager.instance.Convert(ppt.FindPropertyRelative("path").stringValue);
                            if (jdata == null) break;
                            if (!jdata.IsArray)
                            {
                                rect = EditorUtils.MakePopupField(ppt, "key", new GUIContent("Json Key"),
                                 jdata.Keys.ToArray(), rect, m_Gap, m_Height);
                            }
                            else
                            {
                                rect = EditorUtils.MakePopupField(ppt, "key", new GUIContent("Json Key"),
                             jdata[0].Keys.ToArray(), rect, m_Gap, m_Height);
                                if (!hasRoot)
                                {
                                    rect = sourceObjPpt.objectReferenceValue != null ? EditorUtils.MakeObjectField(ppt.FindPropertyRelative("reflectObj"), new GUIContent("Param"), rect, m_Gap, m_Height) : EditorUtils.MakePropertyField("index", ppt, rect, m_Gap, m_Height, new GUIContent("Param"));
                                    rect = EditorUtils.MakePropertyField(sourceObjPpt, rect, m_Gap, m_Height);
                                }
                            }
                        }
                        break;

                    case TextBinding.SourceType.Script:
                        {
                            var go = sourceObjPpt.objectReferenceValue as GameObject;
                            go = go ?? parent;

                            if (sourceObjPpt.objectReferenceValue != null)
                            {
                                rect = EditorUtils.MakeObjectField(go, ppt, "reflectName", new GUIContent("Member"), rect,
                                  m_Gap, m_Height);
                            }
                            else
                            {
                                rect = EditorUtils.MakePropertyField("index", ppt, rect, m_Gap, m_Height);
                            }
                            rect = EditorUtils.MakePropertyField(sourceObjPpt, rect, m_Gap, m_Height);
                        }
                        break;

                    case TextBinding.SourceType.Enum:
                        {
                            rect = EditorUtils.MakePopupField(ppt, "key", new GUIContent("Enum Key"),
                                    CallbackManager.instance.keys, rect, m_Gap, m_Height, false, null, true);

                            var key = ppt.FindPropertyRelative("key").stringValue;
                            var obj = CallbackManager.instance[key];
                            var func = obj as Func<string>;
                            if (func == null)
                            {
                                if (!hasRoot)
                                {
                                    rect = sourceObjPpt.objectReferenceValue != null ? EditorUtils.MakeObjectField(ppt.FindPropertyRelative("reflectObj"), new GUIContent("Param"), rect, m_Gap, m_Height) : EditorUtils.MakePropertyField("index", property, rect, m_Gap, m_Height, new GUIContent("Param"));
                                    rect = EditorUtils.MakePropertyField(sourceObjPpt, rect, m_Gap, m_Height);
                                }
                            }
                        }
                        break;
                }

                var i1 = i;
                rect = EditorUtils.MakeButton(new GUIContent("Delete Text Data"), rect, m_Gap, m_Height, () =>
                {
                    datasPpt.DeleteArrayElementAtIndex(i1);
                });
            }

            rect = EditorUtils.MakeButton(new GUIContent("New Text Data"), rect, m_Gap, m_Height, () =>
            {
                datasPpt.InsertArrayElementAtIndex(datasPpt.arraySize);
            });

            rect = EditorUtils.MakePropertyField("value", property, rect, m_Gap, m_Height);
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            var parent = Selection.activeGameObject;
            var hasRoot = parent.GetComponent<PRHelper>().nodes.Exists(n => n.nodeType == NodeType.Model_CollectionBinding);
            var datasPpt = property.FindPropertyRelative("datas");
            var totalExtra = 0;
            for (int i = 0; i < datasPpt.arraySize; i++)
            {
                var extra = 0;
                var ppt = datasPpt.GetArrayElementAtIndex(i);
                var sourceType = (TextBinding.SourceType)
                      Enum.ToObject(typeof(TextBinding.SourceType), ppt.FindPropertyRelative("type").enumValueIndex);

                switch (sourceType)
                {
                    case TextBinding.SourceType.Json:
                        {
                            extra = 60;
                            var pathStringValue = ppt.FindPropertyRelative("path").stringValue;
                            var jdata = ConfigManager.instance.Convert(pathStringValue);
                            if (jdata != null)
                            {
                                extra = jdata.IsArray ? 120 : 80;
                            }
                            if (hasRoot)
                            {
                                extra = 80;
                            }
                        }
                        break;

                    case TextBinding.SourceType.Script:
                        {
                            extra = 80;
                        }
                        break;

                    case TextBinding.SourceType.Enum:
                        {
                            var key = ppt.FindPropertyRelative("key").stringValue;
                            var obj = CallbackManager.instance[key];
                            var func = obj as Func<string>;
                            extra = func == null ? 100 : 60;
                            if (hasRoot)
                            {
                                extra = 60;
                            }
                        }
                        break;
                }
                totalExtra += extra;
            }

            return 60 + totalExtra;
        }
    }
}