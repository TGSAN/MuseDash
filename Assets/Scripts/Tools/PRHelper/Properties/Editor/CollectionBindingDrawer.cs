using Assets.Scripts.Tools.Commons;
using UnityEditor;
using UnityEngine;

namespace Assets.Scripts.Tools.PRHelper.Properties.Editor
{
    [CustomPropertyDrawer(typeof(CollectionBinding))]
    public class CollectionBindingDrawer : PropertyDrawer
    {
        private float m_Height = 15;
        private float m_Gap = 5;

        public override void OnGUI(Rect rect, SerializedProperty property, GUIContent label)
        {
            var parent = Selection.activeGameObject;
            var go = property.FindPropertyRelative("sourceObj").objectReferenceValue as GameObject;
            go = go ?? parent;
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

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return 40;
        }
    }
}