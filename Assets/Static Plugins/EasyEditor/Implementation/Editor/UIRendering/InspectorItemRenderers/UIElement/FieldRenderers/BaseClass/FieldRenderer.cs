//
// Copyright (c) 2016 Easy Editor 
// All Rights Reserved 
//  
//

using UnityEngine;
using UnityEditor;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;

namespace EasyEditor
{
    /// <summary>
    /// Field renderer is the base class to render fields that Unity renders by default in the inspector.
    /// </summary>
    public class FieldRenderer : InspectorItemRenderer
    {
        /// <summary>
        /// The label of the field.
        /// </summary>
        protected string label = "";
        /// <summary>
        /// The serialized property, which is initialized in the function <c>Render</c>.
        /// </summary>
        protected SerializedProperty serializedProperty = null;
        /// <summary>
        /// If the field does not belong directly to the current serializedObject being rendered in the editor but
        /// another one rendered inline, we need to access this intermediary one to save the modifications done in the 
        /// editor.
        /// </summary>
        protected SerializedObject directParentSerializedObject = null;
		
		public override void InitializeFromEntityInfo(EntityInfo entityInfo)
		{
			base.InitializeFromEntityInfo (entityInfo);
			Init(ObjectNames.NicifyVariableName(this.entityInfo.fieldInfo.Name));
            FindSerializedProperty();
		}

        public override string GetLabel()
        {
            return label;
        }

        private void Init(string aLabel)
        {
            this.label = aLabel;
        }

        public override void Render(Action preRender = null)
        {
            base.Render(preRender);
        }

        /// <summary>
        /// Finds the serialized property starting from the root object (the Monobehaviour or the ScriptableObject) and
        /// going down through every composed fields (custom serializable class and struct).
        /// </summary>
        private void FindSerializedProperty()
        {
            serializedProperty = FieldInfoHelper.GetSerializedPropertyFromPath(entityInfo.propertyPath, 
                                                                               serializedObject, 
                                                                               out directParentSerializedObject);

            if (serializedProperty == null)
            {
                string path = FieldInfoHelper.GetFieldInfoPath(entityInfo.fieldInfo, serializedObject.targetObject.GetType());
                if (!string.IsNullOrEmpty(path))
                {
                    string[] pathTable = path.Split('.');
                    if (pathTable.Length > 0)
                    {
                        serializedProperty = serializedObject.FindProperty(pathTable [0]);
                        for (int i = 1; i < pathTable.Length; i++)
                        {
                            serializedProperty = serializedProperty.FindPropertyRelative(pathTable [i]);
                        }
                    }
                }
                else
                {
                    Debug.LogWarning("The field info " + entityInfo.fieldInfo.Name + " you initialized this renderer with cannot be found in the children properties of the target.");
                }
            }
        }
    }
}