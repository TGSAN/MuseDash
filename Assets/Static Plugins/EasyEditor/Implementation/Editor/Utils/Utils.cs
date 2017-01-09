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
using System.IO;
using System;
using System.Reflection;
using System.Linq;

namespace EasyEditor
{
	public class Utils {

        public static T CreateAssetOfType<T>(string customName = "", string path = "") where T : ScriptableObject
        {
            string name = string.IsNullOrEmpty(customName) ? typeof(T).Name : customName;

            CreateMissingDirectory(path);

            path = AssetDatabase.GenerateUniqueAssetPath(path + name + ".asset");
            T item = ScriptableObject.CreateInstance<T>();
            AssetDatabase.CreateAsset(item, path);
            EditorUtility.FocusProjectWindow();
            Selection.activeObject = item;
            return item;
        }

        public static void CreateAssetFrom<T>(T scriptableObject, string customName = "", string path = "") where T : ScriptableObject
        {
            string name = string.IsNullOrEmpty(customName) ? typeof(T).Name : customName;

            CreateMissingDirectory(path);

            path = AssetDatabase.GenerateUniqueAssetPath(path + name + ".asset");
            AssetDatabase.CreateAsset(scriptableObject, path);
        }

        public static void CreateEditorScript(Type monoType, string path)
        {
            string monoTypeName = monoType.ToString();
            if (monoTypeName.Contains("."))
            {
                monoTypeName = monoTypeName.Substring(monoTypeName.LastIndexOf(".") + 1);
            }

            if (!File.Exists(path + "/" + monoTypeName + "Editor.cs"))
            {
                try
                {
                    TextAsset editorScriptPattern = AssetDatabase.LoadAssetAtPath(ResourcesLocation.ResourcesPath + "Implementation/ScriptPattern/EditorScriptPattern.cs.txt", typeof(TextAsset)) as TextAsset;
                    string newEditorScript = string.Copy(editorScriptPattern.text);

                    newEditorScript = newEditorScript.Replace("[UnityObject]", monoTypeName);

					if(!string.IsNullOrEmpty(monoType.Namespace))
					{
						newEditorScript = newEditorScript.Replace("[using]", "\nnamespace " + monoType.Namespace + "\n{");
						newEditorScript = TabCodeInsideBracket(newEditorScript);
						newEditorScript = newEditorScript + "\n}";
					}
					else
					{
						newEditorScript = newEditorScript.Replace("[using]", "");
					}

                    File.WriteAllText(path + "/" + monoTypeName + "Editor.cs", newEditorScript);
                    AssetDatabase.Refresh();
                }
                catch (Exception e)
                {
                    Debug.LogError("An exception occured while trying to create the editor script for the monobehavior " + monoTypeName + " " + e.Message + "\n" + e.StackTrace);
                }
            }
        }

        public static IEnumerable<Type> FindSubClassesOf<TBaseType>()
        {
            var baseType = typeof(TBaseType);
            var assembly = baseType.Assembly;

            return assembly.GetTypes().Where(t => t.IsSubclassOf(baseType));
        }

        private static void CreateMissingDirectory(string path)
        {
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }
        }

		private static string TabCodeInsideBracket(string script)
		{
			int firstBracket = script.IndexOf('{');
			string insideBracketContent = script.Substring(firstBracket);
			insideBracketContent = insideBracketContent.Replace("\n", "\n\t");
			script = script.Substring(0, firstBracket) + insideBracketContent;

			return script;
		}
	}
}