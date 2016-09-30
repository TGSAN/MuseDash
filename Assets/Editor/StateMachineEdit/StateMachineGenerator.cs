using UnityEngine;
using System.Collections;
using System.Text;
using System;
using System.IO;
using System.Text.RegularExpressions;
using FormulaBase;
using LitJson;

/// <summary>
/// </summary>
public class StateMachineGenerator {
	private static string GenCondictionComponent(string nameSpace, string name, string des) {
		StringBuilder sb = new StringBuilder ();
		sb.Append ("/// 分析工具自动生成代码");
		sb.Append (Environment.NewLine);
		sb.Append ("/// " + des);
		sb.Append (Environment.NewLine);
		sb.Append ("/// ");
		sb.Append (Environment.NewLine);
		sb.Append ("using System;");
		sb.Append (Environment.NewLine);
		sb.Append ("using UnityEngine;");
		sb.Append (Environment.NewLine);
		sb.Append ("using FormulaBase;");
		sb.Append (Environment.NewLine);
		sb.Append ("namespace " + nameSpace + " {");
		sb.Append (Environment.NewLine);
		sb.Append ("	public class " + name + " : OnCondictionBase {");
		sb.Append (Environment.NewLine);
		sb.Append ("		public bool IsCondiction(FormulaHost host, object condiction) {");
		sb.Append (Environment.NewLine);
		sb.Append ("			return true;");
		sb.Append (Environment.NewLine);
		sb.Append ("		}");
		sb.Append (Environment.NewLine);
		sb.Append ("	}");
		sb.Append (Environment.NewLine);
		sb.Append ("}");
		
		string code = sb.ToString ();
		
		return code;
	}

	private static string GenStateComponent(string nameSpace, string name, string des) {
		StringBuilder sb = new StringBuilder ();
		sb.Append ("/// 分析工具自动生成代码");
		sb.Append (Environment.NewLine);
		sb.Append ("/// " + des);
		sb.Append (Environment.NewLine);
		sb.Append ("/// ");
		sb.Append (Environment.NewLine);
		sb.Append ("using System;");
		sb.Append (Environment.NewLine);
		sb.Append ("using UnityEngine;");
		sb.Append (Environment.NewLine);
		sb.Append ("using FormulaBase;");
		sb.Append (Environment.NewLine);
		sb.Append ("namespace " + nameSpace + " {");
		sb.Append (Environment.NewLine);
		sb.Append ("	public class " + name + " : OnStateBase {");
		sb.Append (Environment.NewLine);
		sb.Append ("		public void OnState(FormulaHost host, object param) {");
		sb.Append (Environment.NewLine);
		sb.Append ("		}");
		sb.Append (Environment.NewLine);
		sb.Append ("	}");
		sb.Append (Environment.NewLine);
		sb.Append ("}");
		
		string code = sb.ToString ();
		
		return code;
	}

	/// <summary>
	/// Creates the file.
	/// </summary>
	/// <param name="str">String.</param>
	/// <param name="isCover">If set to <c>true</c> is cover.</param>
	/// 
	/// isCover == true, new file cover old file.
	/// isCover == false, new file only, if old file exist, no generate new.
	/// 
	/// <param name="pathName">Path name.</param>
	private static void CreateFile(string str, bool isCover, string pathName) {
		// Application.dataPath  ==  /Users/xxx/workspace/rhythmgirl/Assets
		string path = Application.dataPath + pathName;
		//创建文件
		if (!File.Exists (path)) {
			StreamWriter strWriter = File.CreateText (path);
			Debug.Log ("create " + path);
			strWriter.Close ();

			if (!isCover) {
				File.WriteAllText (path, str);
			}
		}

		if (!isCover) {
			return;
		}

		Debug.Log ("cover write " + path);
		File.WriteAllText (path, str);
	}

	public static void CreatePath(string pathName) {
		DirectoryInfo dInfo = new DirectoryInfo (Application.dataPath + pathName);
		if (dInfo.Exists) {
			return;
		}

		dInfo.Create ();
	}

	public static void WriteCondiction(string path, string nameSpace, string name, string des) {
		string codeStr = GenCondictionComponent (nameSpace, name, des);
		if (codeStr == null) {
			return;
		}

		CreatePath (path);
		CreateFile (codeStr, false, path + name + ".cs");
		Debug.Log ("Generate State Machine condictions phase result finished.");
	}

	public static void WriteState(string path, string nameSpace, string name, string des) {
		string codeStr = GenStateComponent (nameSpace, name, des);
		if (codeStr == null) {
			return;
		}
		
		CreatePath (path);
		CreateFile (codeStr, false, path + name + ".cs");
		Debug.Log ("Generate State Machine states phase result finished.");
	}
}
