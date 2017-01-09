//
// Copyright (c) 2016 Easy Editor 
// All Rights Reserved 
//  
//

using UnityEngine;
using UEObject = UnityEngine.Object;
using UnityEditor;
using System;
using System.Collections;
using System.Reflection;

namespace EasyEditor
{
    /// <summary>
    /// Render a button in the inspector. This button trigger a method that belongs to the editor target object.
    /// </summary>
	public class ButtonRenderer : InspectorItemRenderer {

        public string label = "";
		public string tooltip = "";

        public override void CreateAsset(string path)
        {
            Utils.CreateAssetFrom<ButtonRenderer>(this, "Button_" + label, path);
        }

		public override void InitializeFromEntityInfo (EntityInfo entityInfo)
		{
			base.InitializeFromEntityInfo (entityInfo);

			EETooltipAttribute tooltipAttribute = AttributeHelper.GetAttribute<EETooltipAttribute> (entityInfo.methodInfo);
			if (tooltipAttribute != null && !string.IsNullOrEmpty(tooltipAttribute.tooltip)) 
			{
				tooltip = tooltipAttribute.tooltip;
			}

			this.label = ObjectNames.NicifyVariableName(entityInfo.methodInfo.Name);
		}

        public override void Render(Action preRender = null)
        {
            base.Render(preRender);

            Rect position = EditorGUILayout.GetControlRect(false);
            position.x += EditorGUI.indentLevel * 15f;
            position.width -= EditorGUI.indentLevel * 15f;
            if (GUI.Button(position, new GUIContent(label, tooltip)))
            {
                if(typeof(Editor).IsAssignableFrom(entityInfo.caller.GetType()))
                {
                    entityInfo.methodInfo.Invoke(entityInfo.caller, null);
                }
                else
                {
                    for(int i = 0; i < serializedObject.targetObjects.Length; i++)
                    {
                        entityInfo.methodInfo.Invoke(serializedObject.targetObjects[i], null);
                    }
                }
            }
        }
	}
}