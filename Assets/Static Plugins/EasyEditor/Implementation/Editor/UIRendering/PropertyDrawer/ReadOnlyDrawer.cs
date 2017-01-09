//
// Copyright (c) 2016 Easy Editor 
// All Rights Reserved 
//  
//

using UnityEditor;
using UnityEngine;
using System.Collections.Generic;

namespace EasyEditor
{
    /// <summary>
    /// Drawer that prevent from modifying a serialized property in the inspector.
    /// </summary>
    [CustomPropertyDrawer(typeof(ReadOnlyAttribute))]
    public class ReadOnlyDrawer : PropertyDrawer
    {
        static float inspectorWidth = 0f;

        public override void OnGUI(Rect position,
                                   SerializedProperty property,
                                   GUIContent label)
        {
            if (Event.current.type == EventType.Repaint)
            {
                inspectorWidth = position.width;
            }

            GUI.enabled = false;
            EditorGUI.PropertyField(position, property, label, true);
            GUI.enabled = true;
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            if (property.isExpanded)
            {
                return property.CountInProperty() * 16f;
            }
            else
            if (FieldInfoHelper.IsTypeOrCollectionOfType<Bounds>(fieldInfo.FieldType))
            {
                return 2 * 16f;
            }
            else if(property.propertyType == SerializedPropertyType.Vector3 && inspectorWidth < 306f + (Settings.indentation + 7) * EditorGUI.indentLevel)
            {
                return 2 * 16f;
            }
            else
            {
                return base.GetPropertyHeight(property, label);
            }
        }
    }
}