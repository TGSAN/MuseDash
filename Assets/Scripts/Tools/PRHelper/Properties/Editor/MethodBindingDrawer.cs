using System;
using System.Collections.Generic;
using System.Linq;
using Assets.Scripts.Tools.Commons;
using Assets.Scripts.Tools.Commons.Editor;
using Assets.Scripts.Tools.Managers;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.Tools.PRHelper.Properties.Editor
{
    [CustomPropertyDrawer(typeof(MethodBinding))]
    public class MethodBindingDrawer : PropertyDrawer
    {
        private float m_Height = 15;
        private float m_Gap = 5;

        public override void OnGUI(Rect rect, SerializedProperty property, GUIContent label)
        {
            var parent = Selection.activeGameObject;
            var btns = new List<Button>();
            var btn = parent.GetComponent<Button>();
            if (btn != null) btns.Add(btn);
            var hasRoot = Selection.activeGameObject.GetComponent<PRHelper>().nodes.Exists(n => n.nodeType == NodeType.Model_CollectionBinding);
            var childBtn = parent.GetComponentsInChildren<Button>();
            childBtn.ToList().ForEach(btns.Add);

            rect = EditorUtils.MakePopupField(property, "name", new GUIContent("Button Name"),
                childBtn.Select(c => c.name).ToArray(), rect, m_Gap, m_Height, false, null, false, "None");

            var sourceObjPpt = property.FindPropertyRelative("reflectObj").FindPropertyRelative("sourceObj");

            var go = sourceObjPpt.objectReferenceValue as GameObject;
            rect = EditorUtils.MakePopupField(property, "key", new GUIContent("Enum Key"),
                    CallbackManager.instance.keys, rect, m_Gap, m_Height, false, null, true);

            var key = property.FindPropertyRelative("key").stringValue;
            var obj = CallbackManager.instance[key];
            var func = obj as Action;
            if (func == null)
            {
                if (!hasRoot)
                {
                    rect = sourceObjPpt.objectReferenceValue != null ? EditorUtils.MakeObjectField(property.FindPropertyRelative("reflectObj"), new GUIContent("Param"), rect, m_Gap, m_Height) : EditorUtils.MakePropertyField("index", property, rect, m_Gap, m_Height, new GUIContent("Param"));
                    rect = EditorUtils.MakePropertyField(sourceObjPpt, rect, m_Gap, m_Height);
                }
            }
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            var extra = 0;
            var parent = Selection.activeGameObject;
            var hasRoot = parent.GetComponent<PRHelper>().nodes.Exists(n => n.nodeType == NodeType.Model_CollectionBinding);
            var key = property.FindPropertyRelative("key").stringValue;
            var obj = CallbackManager.instance[key];
            var func = obj as Action;
            extra = func == null ? 80 : 40;
            if (hasRoot)
            {
                extra = 40;
            }
            return extra;
        }
    }
}