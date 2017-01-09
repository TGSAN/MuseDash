// Copyright (c) 2016 Easy Editor 
// All Rights Reserved 
//  
//

using UnityEngine;
using UnityEditor;
using System;
using System.Reflection;

namespace EasyEditor
{
    /// <summary>
    /// Entity info is a wrapper for fieldInfo and methodInfo.
    /// </summary>
    public class EntityInfo  {

    	public readonly FieldInfo fieldInfo;
        public readonly string propertyPath;
        public readonly SerializedObject serializedObject;

    	public readonly MethodInfo methodInfo;
    	public readonly object caller;

        public readonly Type type;

    	public readonly bool isField;
    	public readonly bool isMethod;
        public readonly bool isType;

        public EntityInfo(FieldInfo fieldInfo, SerializedObject serializedObject, string propertyPath = "")
    	{
            isField = true;
            isMethod = false;
            isType = false;
            this.fieldInfo = fieldInfo;
            this.propertyPath = propertyPath;
            this.serializedObject = serializedObject;
    	}

        public EntityInfo(MethodInfo methodInfo, object caller, SerializedObject serializedObject)
        {
            isField = false;
            isMethod = true;
            isType = false;
            this.methodInfo = methodInfo;
            this.caller = caller;
            this.serializedObject = serializedObject;
        }

        public EntityInfo(Type type, SerializedObject serializedObject, string propertyPath = "")
        {
            isField = false;
            isMethod = false;
            isType = true;

            this.type = type;
            this.propertyPath = propertyPath;
            this.serializedObject = serializedObject;
        }

    	override public string ToString()
    	{
    		if (isField) 
    		{
    			return fieldInfo.Name;
    		} 
    		else 
    		{
    			return methodInfo.Name;
    		}
    	}
    }
}
