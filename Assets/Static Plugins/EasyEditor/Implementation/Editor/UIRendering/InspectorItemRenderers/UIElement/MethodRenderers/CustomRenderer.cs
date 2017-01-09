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
    /// Render some custom editor code.
    /// </summary>
	public class CustomRenderer : InspectorItemRenderer {

        public override void Render(Action preRender = null)
        {
            base.Render(preRender);
			entityInfo.methodInfo.Invoke(entityInfo.caller, null);
        }

	}
}