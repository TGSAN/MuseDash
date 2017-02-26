using Assets.Scripts.Tools.Commons;
using Assets.Scripts.Tools.Commons.Editor;
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
            EditorUtils.MakePopupField(property, new GUIContent("Node Type"), property.enumNames, rect, 0, 0,
                true, null, true);
            //property.enumValueIndex = EditorGUI.Popup(rect, new GUIContent("Node Type"), property.enumValueIndex, popupList);
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return m_Height;
        }
    }
}