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
    /// Drawer displaying a image in the inspector with a specific alignement and size.
    /// </summary>
    [CustomPropertyDrawer(typeof(ImageAttribute))]
    public class ImageDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position,
                                   SerializedProperty property,
                                   GUIContent label)
        {
            if (property.propertyType == SerializedPropertyType.String)
            {
                ImageAttribute imageSettings = (ImageAttribute) attribute;
                float size = (imageSettings.size == 0f ? 70f : imageSettings.size);
                TextAnchor alignement = TextAnchor.MiddleCenter;
                switch(imageSettings.alignement)
                {
                    case ImageAlignement.Left :
                        alignement = TextAnchor.MiddleLeft;
                        break;
                    case ImageAlignement.Right :
                        alignement = TextAnchor.MiddleRight;
                        break;
                    case ImageAlignement.Center :
                        alignement = TextAnchor.MiddleCenter;
                        break;
                }


                Texture2D image = AssetDatabase.LoadAssetAtPath(property.stringValue, typeof(Texture2D)) as Texture2D;

                GUIStyle imageStyle = new GUIStyle(GUI.skin.label);
                imageStyle.alignment = alignement;
                imageStyle.imagePosition = ImagePosition.ImageOnly;
                imageStyle.fixedHeight = size;

                EditorGUI.LabelField(position, new GUIContent(image), imageStyle);
            }
            else
            {
                EditorGUI.PropertyField(position, property, label, true);
            }
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            ImageAttribute imageSettings = (ImageAttribute) attribute;
            float size = (imageSettings.size == 0f ? 70f : imageSettings.size);
            return size;
        }
    }
}