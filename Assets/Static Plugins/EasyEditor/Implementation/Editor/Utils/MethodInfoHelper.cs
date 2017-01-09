using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;
using System;
using System.Reflection;
using System.Linq;
using UEObject = UnityEngine.Object;

namespace EasyEditor
{
	/// <summary>
	/// Tool class providing information about methods info contained into a monobehaviour/scriptableobject class.
	/// </summary>
	public class MethodInfoHelper 
	{
		static public List<MethodInfo> GetAllMethodsTillUnityBaseClass(Type t, BindingFlags flags, List<Type> childTypes = null)
		{
			if (t == null || t == typeof(MonoBehaviour) || t == typeof(ScriptableObject))
                return new List<MethodInfo>();

            List<MethodInfo> methodInfos = GetNotOverridenMethod(childTypes, t, flags);

            if(childTypes == null)
            {
                childTypes = new List<Type>();
            }

            childTypes.Add(t);

            methodInfos.AddRange(GetAllMethodsTillUnityBaseClass(t.BaseType, flags, childTypes));

            return methodInfos;
		}

        static private List<MethodInfo> GetNotOverridenMethod(List<Type> childTypes, Type parent, BindingFlags flag)
        {
            List<MethodInfo> methodInfos = new List<MethodInfo>(parent.GetMethods(flag | BindingFlags.DeclaredOnly));
            
            if(childTypes != null)
            {
                List<MethodInfo> methodToRemove = new List<MethodInfo>();

                for(int i = 0; i < methodInfos.Count; i++)
                {
                    for(int j = 0; j < childTypes.Count; j++)
                    {
                        Type[] parameters = ConvertParameterArrayToTypeArray(methodInfos[i].GetParameters());
                        if(childTypes[j].GetMethod(methodInfos[i].Name, flag | BindingFlags.DeclaredOnly, null, parameters, null) != null)
                        {
                            methodToRemove.Add(methodInfos[i]);
                        }
                    }
                }
                
                for(int i = 0; i < methodToRemove.Count; i++)
                {
                    methodInfos.Remove(methodToRemove[i]);
                }
            }

            return methodInfos;
        }

        static private Type[] ConvertParameterArrayToTypeArray(ParameterInfo[] parameters)
        {
            Type[] types = new Type[parameters.Length];

            for(int i = 0; i < parameters.Length; i++)
            {
                types[i] = parameters[i].ParameterType;
            }

            return types;
        }
	}
}