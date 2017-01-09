
using System;
using System.Reflection;

namespace EasyEditor
{
    public class AttributeHelper {

    	public static T GetAttribute<T>(EntityInfo entityInfo) where T : Attribute
    	{
    		T result = null;

    		if (entityInfo.isField) 
    		{
                if(entityInfo.fieldInfo != null)
                {
    			    result = GetAttribute<T>(entityInfo.fieldInfo);
                }
    		}
    		else if(entityInfo.isMethod)
    		{
    			result = GetAttribute<T>(entityInfo.methodInfo);
    		}

    		return result;
    	}

    	public static T[] GetAttributes<T>(EntityInfo entityInfo) where T : Attribute
    	{
    		T[] results = null;
    		
    		if (entityInfo.isField) 
    		{
    			results = GetAttributes<T>(entityInfo.fieldInfo);
    		}
            else if(entityInfo.isMethod)
    		{
    			results = GetAttributes<T>(entityInfo.methodInfo);
    		}

    		return results;
    	}

    	public static T GetAttribute<T>(FieldInfo fieldInfo) where T : Attribute
    	{
    		T result = null;
    		
    		object[] attributes = fieldInfo.GetCustomAttributes(typeof(T), true);
    		if (attributes.Length > 0)
    		{
    			T attribute = (T) attributes[0];
    			result = attribute;
    		}
    		
    		return result;
    	}

    	public static T[] GetAttributes<T>(FieldInfo fieldInfo) where T : Attribute
    	{
    		object[] attributes = fieldInfo.GetCustomAttributes(typeof(T), true);
    		return (T[]) attributes;
    	}

    	public static T GetAttribute<T>(MethodInfo methodInfo) where T : Attribute
    	{
    		T result = null;
    		
    		object[] attributes = methodInfo.GetCustomAttributes(typeof(T), true);
    		if (attributes.Length > 0)
    		{
    			T attribute = (T) attributes[0];
    			result = attribute;
    		}
    		
    		return result;
    	}
    	
    	public static T[] GetAttributes<T>(MethodInfo methodInfo) where T : Attribute
    	{
    		object[] attributes = methodInfo.GetCustomAttributes(typeof(T), true);
    		return (T[]) attributes;
    	}

        public static T GetAttribute<T>(Type classType) where T : Attribute
        {
            T result = null;

            object[] attributes = classType.GetCustomAttributes(typeof(T), true);
            if (attributes.Length > 0)
            {
                T attribute = (T) attributes[0];
                result = attribute;
            }
            
            return result;
        }

    	public static T[] GetAttributes<T>(Type classType) where T : Attribute
    	{
    		object[] attributes = classType.GetCustomAttributes(typeof(T), false);
            return attributes as T[];
    	}
    }
}
