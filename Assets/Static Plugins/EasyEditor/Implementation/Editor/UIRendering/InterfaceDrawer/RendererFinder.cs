//
// Copyright (c) 2016 Easy Editor 
// All Rights Reserved 
//  
//

using UnityEngine;
using UEObject = UnityEngine.Object;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;
using System;
using System.Reflection;
using System.Linq;

namespace EasyEditor
{
	public class RendererFinder {

        /// <summary>
        /// Gets the list of fields to render in inspector interface.
        /// </summary>
        /// <param name="target">The targeted object.</param>
        /// <returns></returns>
        public static List<InspectorItemRenderer> GetListOfFields(object target, SerializedObject serializedObject, string targetPath = "")
        {
            List<InspectorItemRenderer> fieldRenderers = new List<InspectorItemRenderer>();
			BindingFlags flags = (BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
			IEnumerable<FieldInfo> fieldInfos = FieldInfoHelper.GetAllFieldsTillUnityBaseClass(target.GetType(), flags);
			string currentGroup = "";
			bool reachedBaseClass = false;

            foreach (FieldInfo fieldInfo in fieldInfos)
            {
                InspectorItemRenderer renderer = null;

                if (FieldInfoHelper.IsSerializedInUnity(fieldInfo))
                {
                    string propertyPath = "";
                    if(string.IsNullOrEmpty(targetPath))
                    {
                        propertyPath = fieldInfo.Name;
                    }
                    else
                    {
                        propertyPath = targetPath + "." + fieldInfo.Name;
                    }

                    renderer = InspectorItemRenderer.GetRendererFromFieldInfo(fieldInfo, serializedObject, propertyPath);
                }
                else
                {
					InspectorAttribute inspectorAttribute = AttributeHelper.GetAttribute<InspectorAttribute> (fieldInfo);

                    if (inspectorAttribute != null && !FieldInfoHelper.IsSerializedInUnity(fieldInfo))
                    {
                        Debug.LogWarning("You assigned the attribute" +
                        " [Inspector] to the field " + fieldInfo.Name + " of object " + target.GetType() + " which is not serialized by Unity. EasyEditor will not render it.");
                    }
                }

                if (renderer != null)
                {
					if(!reachedBaseClass)
					{
						if(renderer.entityInfo.fieldInfo.DeclaringType != target.GetType())
						{
							currentGroup = "";
							reachedBaseClass = true;
						}
					}

					AssignGroup(renderer, currentGroup);
					currentGroup = renderer.inspectorAttribute.group;

                    fieldRenderers.Add(renderer);
                }
            }

            return fieldRenderers;
        }

		private static void AssignGroup(InspectorItemRenderer renderer, string currentGroup)
		{
			if(renderer.inspectorAttribute.group == "")
			{
				renderer.inspectorAttribute.group = currentGroup;
			}
		}

        /// <summary>
        /// Gets the list of methods to render in the inspector interface.
        /// </summary>
        /// <param name="caller">The caller.</param>
        /// <returns></returns>
        public static List<InspectorItemRenderer> GetListOfMethods(object caller, SerializedObject serializedObject)
        {
            List<InspectorItemRenderer> methodRenderers = new List<InspectorItemRenderer>();

			BindingFlags flags = (BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static);
			List<MethodInfo> methodInfos = MethodInfoHelper.GetAllMethodsTillUnityBaseClass(caller.GetType(), flags);

            foreach (MethodInfo methodInfo in methodInfos)
            {
                InspectorItemRenderer renderer = InspectorItemRenderer.GetRendererFromMethodInfo(methodInfo, caller, serializedObject);
                
                if (renderer != null)
                {
                    methodRenderers.Add(renderer);
                }
            }

            return methodRenderers;
		}
	}
}