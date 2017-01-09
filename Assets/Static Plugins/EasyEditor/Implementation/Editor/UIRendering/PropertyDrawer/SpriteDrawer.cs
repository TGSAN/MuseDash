using System;
using System.Reflection;
using UnityEditor;
using UnityEngine;
using UEObject = UnityEngine.Object;

namespace EasyEditor
{
    /// <summary>
    /// Sprite Drawer. Allows to display a sprite as a thumbnail in the editor (Only from Unity 5).
    /// </summary>
    [CustomPropertyDrawer(typeof(SpriteAttribute))]
    public class SpriteDrawer : PropertyDrawer 
    {
        UEObject newSprite;

    	public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    	{
            if(fieldInfo.FieldType.IsAssignableFrom(typeof(Sprite)))
            {
                EditorGUI.BeginChangeCheck ();
                EditorGUI.showMixedValue = property.hasMultipleDifferentValues;
                newSprite = EditorGUILayout.ObjectField(property.name, property.objectReferenceValue, typeof(Sprite), true);
                EditorGUI.showMixedValue = false;

                if (EditorGUI.EndChangeCheck ())
                {
                    property.objectReferenceValue = newSprite;
                }
            }
            else
            {
                EditorGUILayout.HelpBox("Only field of type Sprite can have the attribute [Sprite].", UnityEditor.MessageType.Error);
            }
    	}
    }
}