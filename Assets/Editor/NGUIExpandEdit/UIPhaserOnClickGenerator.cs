using UnityEngine;
using System.Collections;
using System.Text;
using System;
using System.IO;
using System.Text.RegularExpressions;
using FormulaBase;
using LitJson;

public class UIPhaserOnClickGenerator {
	private static string GenUIComponent(string nameSpace, string name, string des) {
		StringBuilder sb = new StringBuilder ();
		sb.Append ("/// UI分析工具自动生成代码");
		sb.Append (Environment.NewLine);
		sb.Append ("/// " + des);
		sb.Append (Environment.NewLine);
		sb.Append ("/// ");
		sb.Append (Environment.NewLine);
		sb.Append ("using System;");
		sb.Append (Environment.NewLine);
		sb.Append ("using UnityEngine;");
		sb.Append (Environment.NewLine);
		sb.Append ("namespace " + nameSpace + " {");
		sb.Append (Environment.NewLine);
		sb.Append ("	public class " + name + " : UIPhaseOnClickBase {");
		sb.Append (Environment.NewLine);
		sb.Append ("		public static void Do(GameObject gameObject) {");
		sb.Append (Environment.NewLine);
		sb.Append ("				OnDo(gameObject);");
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

	public static void WriteCode(string path, string nameSpace, string name, string des) {
		string codeStr = GenUIComponent (nameSpace, name, des);
		if (codeStr == null) {
			return;
		}

		CreatePath (path);
		CreateFile (codeStr, false, path + name + ".cs");
	}
}
