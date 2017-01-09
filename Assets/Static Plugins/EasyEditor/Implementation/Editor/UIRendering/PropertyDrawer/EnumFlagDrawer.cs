using System;
using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace EasyEditor
{
    /// <summary>
    /// Enum flag drawer provided by ikriz (https://gist.github.com/ikriz).
    /// Your enum should be declare as the following example :
    /// 
    /// public enum Compatibility
    ///{
    ///    iOS             = 0x001,
    ///    Android         = 0x002,
    ///    WindowsPhone    = 0x004,
    ///    PS4             = 0x008,
    ///    Wii             = 0x010,
    ///    XBoxOne         = 0x020,
    ///    WindowsDesktop  = 0x040,
    ///    Linux           = 0x080,
    ///    MacOS           = 0x100
    ///}
    /// 
    /// </summary>
    [CustomPropertyDrawer(typeof(EnumFlagAttribute))]
    public class EnumFlagDrawer : PropertyDrawer 
    {
    	public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    	{
    		EnumFlagAttribute flagSettings = (EnumFlagAttribute)attribute;
            Enum targetEnum = (Enum)Enum.ToObject(fieldInfo.FieldType, property.intValue);

            GUIContent propName = new GUIContent(flagSettings.enumName);
    		if (string.IsNullOrEmpty(propName.text))
                propName = label;
    		
            label = EditorGUI.BeginProperty(position, label, property);
            label.text = propName.text;

            EditorGUI.BeginChangeCheck ();
            Enum enumNew = EditorGUI.EnumMaskField(position, label, targetEnum);

            if (EditorGUI.EndChangeCheck ())
                property.intValue = (int) Convert.ChangeType(enumNew, targetEnum.GetType());
    		
    		EditorGUI.EndProperty();
    	}
    }
}