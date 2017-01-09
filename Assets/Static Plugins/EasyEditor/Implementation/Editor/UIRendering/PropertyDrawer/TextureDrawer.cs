using System;
using System.Reflection;
using UnityEditor;
using UnityEngine;
using UEObject = UnityEngine.Object;

namespace EasyEditor
{
    /// <summary>
    /// Texture Drawer. Allows to display a texture or a sprite as a thumbnail in the editor.
    /// </summary>
    [CustomPropertyDrawer(typeof(TextureAttribute))]
    public class TextureDrawer : PropertyDrawer 
    {
        UEObject newTexture;

    	public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    	{
            if(fieldInfo.FieldType.IsAssignableFrom(typeof(Texture2D)))
            {
                EditorGUI.BeginChangeCheck ();
                EditorGUI.showMixedValue = property.hasMultipleDifferentValues;
                newTexture = EditorGUILayout.ObjectField(property.name, property.objectReferenceValue, typeof(Texture), true);
                EditorGUI.showMixedValue = false;

                if (EditorGUI.EndChangeCheck ())
                {
                    property.objectReferenceValue = newTexture;
                }
            }
            else
            {
                EditorGUILayout.HelpBox("Only texture can have the attribute [Texture].", UnityEditor.MessageType.Error);
            }
    	}
    }
}