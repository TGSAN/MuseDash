using System.Collections.Generic;
using System.Linq;
using Assets.Scripts.Tools.Commons;
using Assets.Scripts.Tools.Commons.Editor;
using Assets.Scripts.Tools.Managers;
using UnityEditor;
using UnityEngine;

namespace Assets.Scripts.Tools.PRHelper.Properties.Editor
{
    [CustomPropertyDrawer(typeof(SpineBinding))]
    public class SpineBindingDrawer : PropertyDrawer
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
            rect = EditorUtils.MakePropertyField("animationName", property, rect, m_Gap, m_Height);

            rect = EditorUtils.MakePopupField(property, "path", new GUIContent("Json Path"),
               ConfigManager.instance.configs.Select(c => c.path).ToArray(), rect, m_Gap, m_Height);
            var jdata = ConfigManager.instance.Convert(property.FindPropertyRelative("path").stringValue);
            if (jdata == null) return;
            if (!jdata.IsArray)
            {
                rect = EditorUtils.MakePopupField(property, "key", new GUIContent("Json Key"),
                 jdata.Keys.ToArray(), rect, m_Gap, m_Height);
            }
            else
            {
                var sourceObjPpt = property.FindPropertyRelative("reflectObj").FindPropertyRelative("sourceObj");
                rect = EditorUtils.MakePopupField(property, "key", new GUIContent("Json Key"),
             jdata[0].Keys.ToArray(), rect, m_Gap, m_Height);
                if (!hasRoot)
                {
                    rect = sourceObjPpt.objectReferenceValue != null ? EditorUtils.MakeObjectField(property.FindPropertyRelative("reflectObj"), new GUIContent("Param"), rect, m_Gap, m_Height) : EditorUtils.MakePropertyField("index", property, rect, m_Gap, m_Height, new GUIContent("Param"));
                    rect = EditorUtils.MakePropertyField(sourceObjPpt, rect, m_Gap, m_Height);
                }
            }
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            var hasRoot = Selection.activeGameObject.GetComponent<PRHelper>().nodes.Exists(n => n.nodeType == NodeType.Model_CollectionBinding);
            if (hasRoot)
            {
                return 80;
            }
            return 120;
        }
    }
}