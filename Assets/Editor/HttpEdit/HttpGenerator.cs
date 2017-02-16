using UnityEngine;
using System.Collections;
using System.Text;
using System;
using System.IO;
using System.Text.RegularExpressions;
using FormulaBase;
using LitJson;

/// <summary>
/// Http protocal generator.
/// 
/// 代码生成器，通过服务端返回 自动生成代码模块
/// /Scripts/GameData/HttpProtocal.cs
/// 
/// 通过分析配置内容，在CustomFormula.Result中生成各个对应公式
/// </summary>
public class HttpGenerator {
	private const string CODE_PATH = "/Scripts/GameData/HttpProtocal.cs";
	private static void GenHttpProtocalByFunction(StringBuilder sb, string name, JsonData protocal) {
		string des = protocal ["des"].ToJson ();
		string funcname = protocal ["path"].ToJson ();
		string funparam = "\"f=\" + " + funcname;
		sb.Append ("		///");
		sb.Append (Environment.NewLine);
		sb.Append ("		/// param is : " + des);
		sb.Append (Environment.NewLine);
		sb.Append ("		///");
		sb.Append (Environment.NewLine);
		sb.Append ("		public static void " + name + " (JsonData param = null, HttpResponseDelegate response = null) {");
		sb.Append (Environment.NewLine);
		sb.Append ("			string _param = " + funparam + ";");
		sb.Append (Environment.NewLine);
		sb.Append ("			if (param != null) {");
		sb.Append (Environment.NewLine);
		sb.Append ("				_param += " + "\"&p=\" + " + "param.ToJson();");
		sb.Append (Environment.NewLine);
		sb.Append ("			}");
		sb.Append (Environment.NewLine);
		sb.Append ("			HttpModule.Instance.Request (_param, response);");
		sb.Append (Environment.NewLine);
		sb.Append ("		}");
		sb.Append (Environment.NewLine);
	}

	private static void GenHttpProtocalByClass(StringBuilder sb, string name, JsonData protocal) {
		sb.Append ("	public class " + name + " {");
		sb.Append (Environment.NewLine);

		foreach (string fname in protocal.Keys) {
			JsonData jd = protocal [fname];
			GenHttpProtocalByFunction (sb, fname, jd);
		}
		
		sb.Append (Environment.NewLine);
		sb.Append ("	}");
		sb.Append (Environment.NewLine);
	}

	private static void GenHttpProtocalByNamespace(StringBuilder sb, string name, JsonData protocal) {
		sb.Append ("namespace " + name + " {");
		sb.Append (Environment.NewLine);

		foreach (string cname in protocal.Keys) {
			JsonData jd = protocal [cname];
			GenHttpProtocalByClass (sb, cname, jd);
		}

		sb.Append ("}");
		sb.Append (Environment.NewLine);
		sb.Append (Environment.NewLine);
	}

	private static string GenHttpProtocal(JsonData protocal) {
		if (protocal == null) {
			return null;
		}

		JsonData _moduleDepart = new JsonData ();
		foreach (string k in protocal.Keys) {
			JsonData j = protocal [k];
			if (j == null) {
				continue;
			}

			if (k != "d") {
				continue;
			}

			foreach (string funcname in j.Keys) {
				// string _finfo = funcname;
				string[] _nameSplite = funcname.Split ('.');
				if (_nameSplite.Length < 3) {
					return null;
				}

				string _namespace = _nameSplite [0];
				string _class = _nameSplite [1];
				string _function = _nameSplite [2];

				if (!_moduleDepart.Keys.Contains (_namespace)) {
					_moduleDepart [_namespace] = new JsonData ();
				}

				if (!_moduleDepart [_namespace].Keys.Contains (_class)) {
					_moduleDepart [_namespace] [_class] = new JsonData ();
				}

				JsonData _argInfos = j [funcname];
				JsonData _info = new JsonData();
				_info["des"] = _argInfos;
				_info["path"] = funcname;
				_moduleDepart [_namespace] [_class] [_function] = _info;

				/*
				for (int _k = 0; _k < _argInfos.Count; _k++) {
					JsonData _argnames = _argInfos [_k];
					_finfo += (", " + _argnames.ToJson ());
				}
				Debug.Log (_finfo);
				*/
			}
		}


		StringBuilder sb = new StringBuilder ();
		sb.Append ("///Http protocal 工具自动生成代码，仅供调用，无需编辑");
		sb.Append (Environment.NewLine);
		sb.Append ("using System;");
		sb.Append (Environment.NewLine);
		sb.Append ("using LitJson;");
		sb.Append (Environment.NewLine);
		sb.Append (Environment.NewLine);

		foreach (string np in _moduleDepart.Keys) {
			JsonData nj = _moduleDepart [np];
			GenHttpProtocalByNamespace (sb, np, nj);
		}

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

	public static void WriteCode(JsonData protocal) {
		string codeStr = GenHttpProtocal (protocal);
		if (codeStr == null) {
			return;
		}
		
		CreateFile (codeStr, true, CODE_PATH);
		Debug.Log ("Generate http protocal finished.");
	}
}
