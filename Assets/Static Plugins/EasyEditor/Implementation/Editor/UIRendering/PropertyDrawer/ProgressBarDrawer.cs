using System;
using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace EasyEditor
{
    /// <summary>
    /// Display a int or a float as a progress bar. This property drawer was generously provided by Alessio Marzoli.
    /// </summary>
    [CustomPropertyDrawer(typeof(ProgressBarAttribute))]
    public class ProgressBarDrawer : PropertyDrawer 
    {
        private bool isNumericValue;

        public override void OnGUI (Rect position, SerializedProperty property, GUIContent label) 
        {
            float value = 0f;
            if (property.propertyType == SerializedPropertyType.Float)
            {
                value = property.floatValue;
                isNumericValue = true;
            }
            else
            if (property.propertyType == SerializedPropertyType.Integer)
            {
                value = (float)property.intValue;
                isNumericValue = true;
            }
            else
            {
                isNumericValue = false;
            }

            if (isNumericValue)
            {
                ProgressBarAttribute barAttribute = (ProgressBarAttribute)attribute;
                EditorGUI.BeginProperty(position, label, property);
                EditorGUI.ProgressBar(position, (value / barAttribute.max), label.text);
                EditorGUI.EndProperty();
            }
            else
            {
                EditorGUI.BeginProperty(position, label, property);
                EditorGUI.PropertyField(position, property);
                EditorGUI.EndProperty();
            }
        }
    }
}