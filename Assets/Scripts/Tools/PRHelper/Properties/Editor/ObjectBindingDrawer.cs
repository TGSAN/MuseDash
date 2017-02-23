using System.Collections.Generic;
using System.Linq;
using Assets.Scripts.Tools.Commons;
using Assets.Scripts.Tools.Managers;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.Tools.PRHelper.Properties.Editor
{
    [CustomPropertyDrawer(typeof(ObjectBinding))]
    public class ObjectBindingDrawer : PropertyDrawer
    {
        private float m_Height = 15;
        private float m_Gap = 5;

        public override void OnGUI(Rect rect, SerializedProperty property, GUIContent label)
        {
            var parent = Selection.activeGameObject;
            var transforms = new List<Transform>();
            var transform = parent.GetComponent<Transform>();
            if (transform != null) transforms.Add(transform);
            var hasRoot = Selection.activeGameObject.GetComponent<PRHelper>().nodes.Exists(n => n.nodeType == NodeType.Model_CollectionBinding);
            var childTrasnform = parent.GetComponentsInChildren<Transform>();
            childTrasnform.ToList().ForEach(transforms.Add);

            rect = EditorUtils.MakePopupField(property, "name", new GUIContent("Transform Name"),
                childTrasnform.Select(c => c.name).ToArray(), rect, m_Gap, m_Height, false, null, false, "None");

            rect = EditorUtils.MakePopupField(property, "path", new GUIContent("Json Path"),
               ConfigManager.instance.configs.Select(c => c.path).ToArray(), rect, m_Gap, m_Height);
            var jdata = ConfigManager.instance.Convert(property.FindPropertyRelative("path").stringValue);
            if (jdata == null) return;
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
                    if (property.FindPropertyRelative("sourceObj").objectReferenceValue != null)
                    {
                        rect = EditorUtils.MakeObjectField(go, property, "fieldName", new GUIContent("Index"),
                            rect,
                            m_Gap, m_Height);
                    }
                    else
                    {
                        rect = EditorUtils.MakePropertyField("index", property, rect, m_Gap, m_Height);
                    }
                    rect = EditorUtils.MakePropertyField("sourceObj", property, rect, m_Gap, m_Height);
                }
            }
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            var hasRoot = Selection.activeGameObject.GetComponent<PRHelper>().nodes.Exists(n => n.nodeType == NodeType.Model_CollectionBinding);
            if (hasRoot)
            {
                return 60;
            }
            return 100;
        }
    }
}