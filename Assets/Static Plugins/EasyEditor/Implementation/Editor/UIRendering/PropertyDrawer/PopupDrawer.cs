using UnityEditor;
using UnityEngine;
using System;

namespace EasyEditor{
    /// <summary>
    /// Creates a popup list with the provided values. This class was generously provided by HunterPT on the thread 
    /// http://forum.unity3d.com/threads/propertydrawers-for-easy-inspector-customization.150337/.
    /// </summary>

    [CustomPropertyDrawer(typeof(PopupAttribute))]
    public class PopupDrawer : PropertyDrawer
    {
        PopupAttribute popupAttribute { get { return ((PopupAttribute)attribute); } }
        int index;

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            label = EditorGUI.BeginProperty(position, label, property);

            // Checks to see what is the type of the provided values and acts accordingly.
            if (popupAttribute.variableType == typeof(int[]))
            {
                EditorGUI.BeginChangeCheck();

                // Checks all items in the provided list, to see if any of them is a match with the property value, if so assigns that value to the index.
                for (int i = 0; i < popupAttribute.list.Length; i++)
                {
                    if (property.intValue == int.Parse(popupAttribute.list[i]))
                    {
                        index = i;
                    }
                }

                GUIContent[] popupList = GetGUIContentArray(popupAttribute.list);

                index = EditorGUI.Popup(position, label, index, popupList);

                if (EditorGUI.EndChangeCheck())
                {
                    property.intValue = int.Parse(popupAttribute.list[index]);
                }
            }
            else if (popupAttribute.variableType == typeof(float[]))
            {
                EditorGUI.BeginChangeCheck();
                // Checks all items in the provided list, to see if any of them is a match with the property value, if so assigns that value to the index.
                for (int i = 0; i < popupAttribute.list.Length; i++)
                {
                    if (property.floatValue == Convert.ToSingle(popupAttribute.list[i]))
                    {
                        index = i;
                    }
                }

                GUIContent[] popupList = GetGUIContentArray(popupAttribute.list);

                index = EditorGUI.Popup(position, label, index, popupList);
                if (EditorGUI.EndChangeCheck())
                {
                    property.floatValue = Convert.ToSingle(popupAttribute.list[index]);
                }
            }
            else if (popupAttribute.variableType == typeof(string[]))
            {
                EditorGUI.BeginChangeCheck();
                // Checks all items in the provided list, to see if any of them is a match with the property value, if so assigns that value to the index.
                for (int i = 0; i < popupAttribute.list.Length; i++)
                {
                    if (property.stringValue == popupAttribute.list[i])
                    {
                        index = i;
                    }
                }

                GUIContent[] popupList = GetGUIContentArray(popupAttribute.list);
                index = EditorGUI.Popup(position, label, index, popupList);
                if (EditorGUI.EndChangeCheck())
                {
                    property.stringValue = popupAttribute.list[index];
                }
            }
            else
            {
                EditorGUI.LabelField(position, "ERROR READ CONSOLE FOR MORE INFO");
            }

            EditorGUI.EndProperty();
        }

        private GUIContent[] GetGUIContentArray(string[] stringArray)
        {
            GUIContent[] guiContentArray = new GUIContent[stringArray.Length];
            for(int i = 0; i < guiContentArray.Length; i++)
            {
                guiContentArray[i] = new GUIContent(stringArray[i]);
            }

            return guiContentArray;
        }
    }
}