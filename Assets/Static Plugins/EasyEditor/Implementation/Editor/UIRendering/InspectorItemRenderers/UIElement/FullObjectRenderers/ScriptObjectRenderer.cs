//
// Copyright (c) 2016 Easy Editor 
// All Rights Reserved 
//  
//

using UnityEngine;
using UnityEditor;
using UEObject = UnityEngine.Object;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;

namespace EasyEditor
{
    /// <summary>
    /// Core renderer of EasyEditor. After parsing a monobehaviour/scriptableobject and its editor script, 
    /// it generates a list of sub renderer for each fields, methods
    /// and renders it in the inspector.
    /// </summary>
	public class ScriptObjectRenderer : FullObjectRenderer
	{
        public EasyEditorBase editorScript{get; private set;}

        public void Initialize(SerializedObject serializedObject, EasyEditorBase editorScript)
        {
            this.editorScript = editorScript;
            base.InitializeFromEntityInfo(new EntityInfo(serializedObject.targetObject.GetType(), serializedObject, ""));
        }

        protected override void RetrieveGroupList()
        {
            GroupsAttribute groupsAttribute = AttributeHelper.GetAttribute<GroupsAttribute>(editorScript.GetType());
            if (groupsAttribute != null)
            {
                groups = new Groups(groupsAttribute.groups);
            }
            else
            {
                groups = new Groups(new string[] { "" });
            }
        }

        /// <summary>
        /// Initializes the renderers list with monobehaviour/scriptableobjects fields and method, 
        /// then with method from the editor script delegate.
        /// </summary>
        override protected void InitializeRenderersList()
        {
            base.InitializeRenderersList();

            renderers = new List<InspectorItemRenderer>();
            
            renderers.AddRange(RendererFinder.GetListOfFields(serializedObject.targetObject, serializedObject));
            renderers.AddRange(RendererFinder.GetListOfMethods(serializedObject.targetObject, serializedObject));
            renderers.AddRange(RendererFinder.GetListOfMethods(editorScript, serializedObject));
            
            InspectorItemRendererOrderComparer comparer = new InspectorItemRendererOrderComparer(groups, renderers);
            renderers.Sort(comparer);
        }

		public override void Render (Action preRender = null)
		{
            if (editorScript == null)
            {
                Debug.LogError("You need to set the easyeditor script this renderer is rendering for.");
            }

            EditorGUILayout.BeginVertical();
            
            DrawScriptHeader();
            
            base.Render(preRender);
            
            EditorGUILayout.EndVertical();
 		}

        private void DrawScriptHeader()
        {
            UEObject script = null;

            if (serializedObject.targetObject is MonoBehaviour)
            {
                script = MonoScript.FromMonoBehaviour((MonoBehaviour)serializedObject.targetObject);
            }
            else 
            if (serializedObject.targetObject is ScriptableObject)
            {
                script = MonoScript.FromScriptableObject((ScriptableObject)serializedObject.targetObject);
            }


            EditorGUILayout.ObjectField("Script", script, typeof(MonoScript), false);
            GUILayout.Space(5f);
        }
	}
}