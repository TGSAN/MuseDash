using Assets.Scripts.Tools.Commons;
using UnityEditor;
using UnityEngine;

namespace Assets.Scripts.Tools.PRHelper.Properties.Editor
{
    [CustomPropertyDrawer(typeof(NodeType))]
    public class NodeTypeDrawer : PropertyDrawer
    {
        private readonly float m_Height = 15;

        public override void OnGUI(Rect rect, SerializedProperty property, GUIContent label)
        {
            rect = new Rect(rect.x, rect.y, rect.width, m_Height);
            var strs = property.enumNames;
            for (int i = 0; i < strs.Length; i++)
            {
                strs[i] = strs[i].Replace('_', '/');
            }
            var popupList = EditorUtils.GetGUIContentArray(strs);
            property.enumValueIndex = EditorGUI.Popup(rect, new GUIContent("Node Type"), property.enumValueIndex, popupList);
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return m_Height;
        }
    }
}