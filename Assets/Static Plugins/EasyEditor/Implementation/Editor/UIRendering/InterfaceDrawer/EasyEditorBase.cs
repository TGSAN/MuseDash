//
// Copyright (c) 2016 Easy Editor 
// All Rights Reserved 
//  
//

using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;
using System;
using System.Reflection;

namespace EasyEditor
{
    /// <summary>
    /// Core class of Easy Editor system. It creates a specific <c>InspectorItemRenders</c>, <c>TargetObjectRenderer</c> that parses
    /// the monobehaviour/scriptableobject fields and method, and the script that renders it methods, to generate its inspector interface.
    /// </summary>
    public class EasyEditorBase : Editor {

        private ScriptObjectRenderer scriptObjectRenderer;

        /// <summary>
        /// Hides the group by name. Can be called in <c>OnInspectorGUI</c> or in any method called in the editor script.
        /// </summary>
        /// <param name="group">The name of the group.</param>
        public void HideGroup(string group)
        {
            scriptObjectRenderer.HideGroup(group);
        }

        /// <summary>
        /// Shows the group by name. Can be called in <c>OnInspectorGUI</c> or in any method called in the editor script.
        /// </summary>
        /// <param name="group">The name of the group.</param>
        public void ShowGroup(string group)
        {
            scriptObjectRenderer.ShowGroup(group);
        }

        /// <summary>
        /// Hides the renderer by id. Can be called in <c>OnInspectorGUI</c> or in any method called in the editor script.
        /// </summary>
        /// <param name="group">The name of the group.</param>
        public void HideRenderer(string id)
        {
            scriptObjectRenderer.HideRenderer(id);
        }

        /// <summary>
        /// Show the renderer by id. Can be called in <c>OnInspectorGUI</c> or in any method called in the editor script.
        /// </summary>
        /// <param name="group">The name of the group.</param>
        public void ShowRenderer(string id)
        {
            scriptObjectRenderer.ShowRenderer(id);
        }

        public InspectorItemRenderer LookForRenderer(string id)
        {
            return scriptObjectRenderer.LookForRenderer(id);
        }

        /// <summary>
        /// Called when the editor script is enable, usually when it is going to be displayed in the inspector.
        /// </summary>
        public void OnEnable()
        {
            scriptObjectRenderer = (ScriptObjectRenderer) InspectorItemRenderer.CreateRenderer(typeof(ScriptObjectRenderer));
            scriptObjectRenderer.Initialize(this.serializedObject, this);
        }

        override public void OnInspectorGUI()
        {
            serializedObject.Update();
            scriptObjectRenderer.Render();
            serializedObject.ApplyModifiedProperties();
        }
    }
}