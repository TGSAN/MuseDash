using System;
using Assets.Scripts.Common;
using Assets.Scripts.Tools.Commons;
using Assets.Scripts.Tools.PRHelper.Properties;
using UnityEditor;
using UnityEngine;

namespace Assets.Scripts.Tools.PRHelper.Editor
{
    [CustomPropertyDrawer(typeof(PRHelperNode))]
    public class PRHelperNodeDrawer : PropertyDrawer
    {
        private readonly float m_Gap = 5;
        private readonly float m_Height = 15;

        public override void OnGUI(Rect rect, SerializedProperty property, GUIContent label)
        {
            EditorGUI.BeginProperty(rect, label, property);
            EditorGUI.BeginChangeCheck();

            rect = new Rect(rect.x, rect.y - rect.height, rect.width, rect.height);
            var nodeTypeProperty = property.FindPropertyRelative("nodeType");
            rect = EditorUtils.MakePropertyField("nodeType", property, rect, m_Gap, m_Height);
            rect = EditorUtils.MakePropertyField("key", property, rect, m_Gap, m_Height);
            rect = EditorUtils.MakePropertyField("eventType", property, rect, m_Gap, m_Height);

            var enumType = (NodeType)Enum.Parse(typeof(NodeType), nodeTypeProperty.enumNames[nodeTypeProperty.enumValueIndex]);
            var propertyName = StringUtils.LastAfter(enumType.ToString(), '_');

            propertyName = StringUtils.FirstToLower(propertyName);
            var showProperty = property.FindPropertyRelative(propertyName);

            if (showProperty != null)
            {
                EditorGUI.PropertyField(rect, showProperty);
            }

            EditorGUI.EndProperty();
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            var nodeTypeProperty = property.FindPropertyRelative("nodeType");
            var enumType = (NodeType)Enum.Parse(typeof(NodeType), nodeTypeProperty.enumNames[nodeTypeProperty.enumValueIndex]);

            var propertyName = StringUtils.LastAfter(enumType.ToString(), '_');
            propertyName = propertyName.Substring(0, 1).ToLower() + propertyName.Substring(1);
            var showProperty = property.FindPropertyRelative(propertyName);

            if (showProperty != null)
            {
                return 3 * (m_Height + m_Gap) + EditorGUI.GetPropertyHeight(showProperty);
            }
            return 3 * (m_Height + m_Gap);
        }
    }
}