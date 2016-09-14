using UnityEngine;
using System.Collections;
using System.Text;
using System;
using System.IO;
using System.Text.RegularExpressions;
using FormulaBase;
using System.Collections.Generic;

/// <summary>
/// Formula generator.
/// 
/// 代码生成器，通过公式界面的编辑操作，在保存时会自动生成代码模块
/// Scripts/Common/Formula/CustomFormula.cs
/// 
/// 通过分析配置内容，在CustomFormula.Result中生成各个对应公式
/// </summary>
public class FormulaGenerator {
	private const string PATH_NAME = "/Scripts/Common/Formula/CustomFormula.cs";
	private static string FromulaAnaly(FormulaStruct formula) {
		char space = FormulaBase.FormulaBase.SPLITE_UNIT;
		// splite by empty space
		string[] ss = formula.formula.Split (new char[] { space });
		if (ss == null || ss.Length <= 0) {
			return null;
		}

		int pIdx = 0;
		for (int i = 0; i < ss.Length; i++) {
			string _s = ss [i];
			if (_s == null || _s.Length <= 0 || (!FormulaBase.FormulaBase.IsChinese (_s [0].ToString ()) && !FormulaBase.FormulaBase.IsSignMatch (_s))) {
				if (FormulaBase.FormulaBase.IsDig (_s)) {
					_s += "f";
					//Debug.Log ("------>> " + _s);
					ss [i] = _s;
				}

				continue;
			}
			
			ss [i] = "formulaObject.GetParamValue (" + pIdx + ")";
			pIdx += 1;
		}
		
		return string.Join (space.ToString (), ss);
	}

	private static string GenerateCustomFormula() {
		FormulaStruct[] formulas = FormulaData.Instance.Formulas;

		StringBuilder sb = new StringBuilder ();
		sb.Append ("///CustomFormula 工具自动生成代码，仅供调用，无需编辑");
		sb.Append (Environment.NewLine);
		sb.Append ("using System;");
		sb.Append (Environment.NewLine);
		sb.Append ("namespace FormulaBase {");
		sb.Append (Environment.NewLine);
		sb.Append ("	public class CustomFormula {");
		sb.Append (Environment.NewLine);
		sb.Append ("		public static float Result(int idx, FormulaObject formulaObject) {");
		sb.Append (Environment.NewLine);

		if (formulas != null && formulas.Length > 0) {
			sb.Append ("			FormulaStruct[] formulas = FormulaData.Instance.Formulas;");
			sb.Append (Environment.NewLine);
			sb.Append ("			switch(idx) {");
			sb.Append (Environment.NewLine);

			for (int i = 0; i < formulas.Length; i++) {
				sb.Append ("				case " + i + ":");
				sb.Append (Environment.NewLine);

				string _s = FromulaAnaly (formulas [i]);
				sb.Append ("					return " + _s + ";");
				sb.Append (Environment.NewLine);

				//sb.Append ("					break;");
				//sb.Append (Environment.NewLine);
			}

			sb.Append ("			}");
			sb.Append (Environment.NewLine);
		}

		sb.Append ("			return 0f;");
		sb.Append (Environment.NewLine);

		sb.Append ("		}");
		sb.Append (Environment.NewLine);
		sb.Append ("	}");
		sb.Append (Environment.NewLine);
		sb.Append ("}");
		
		string code = sb.ToString ();
		
		return code;
	}

	private static string GenerateCustomComponent(string componentName, int hostIdx) {
		StringBuilder sb = new StringBuilder ();
		sb.Append ("///自定义模块，可定制模块具体行为");
		sb.Append (Environment.NewLine);
		sb.Append ("using System;");
		sb.Append (Environment.NewLine);
		sb.Append ("namespace FormulaBase {");
		sb.Append (Environment.NewLine);
		sb.Append ("	public class " + componentName + " : CustomComponentBase {");
		sb.Append (Environment.NewLine);

		sb.Append ("		private static " + componentName + " instance = null;");
		sb.Append (Environment.NewLine);
		sb.Append ("		private const int HOST_IDX = " + hostIdx + ";");
		sb.Append (Environment.NewLine);

		sb.Append ("		public static " + componentName + " Instance {");
		sb.Append (Environment.NewLine);
		sb.Append ("			get {");
		sb.Append (Environment.NewLine);
		sb.Append ("				if(instance == null) {");
		sb.Append (Environment.NewLine);
		sb.Append ("					instance = new " + componentName + "();");
		sb.Append (Environment.NewLine);
		sb.Append ("				}");
		sb.Append (Environment.NewLine);
		sb.Append ("			return instance;");
		sb.Append (Environment.NewLine);
		sb.Append ("			}");
		sb.Append (Environment.NewLine);
		sb.Append ("		}");

		sb.Append (Environment.NewLine);
		sb.Append ("	}");
		sb.Append (Environment.NewLine);
		sb.Append ("}");
		
		string code = sb.ToString ();
		
		return code;
	}

	private const string KEY_NAME = "FORMULA_";
	private static string GenFormulaKeys() {
		StringBuilder sb = new StringBuilder ();
		sb.Append ("///FormulaKeys 工具自动生成代码，仅供调用，无需编辑");
		sb.Append (Environment.NewLine);
		sb.Append ("using System;");
		sb.Append (Environment.NewLine);
		sb.Append ("namespace FormulaBase {");
		sb.Append (Environment.NewLine);
		sb.Append ("	public class FormulaKeys {");
		sb.Append (Environment.NewLine);

		string[] names = FormulaEdit.GetFormulasNames ();
		if (names != null && names.Length > 0) {
			for (int i = 0; i < names.Length; i++) {
				string _s = names [i];
				sb.Append ("		///");
				sb.Append (Environment.NewLine);
				sb.Append ("		/// " + _s);
				sb.Append (Environment.NewLine);
				sb.Append ("		///");
				sb.Append (Environment.NewLine);
				sb.Append ("		public const int " + KEY_NAME + i + " = " + i + ";");
				sb.Append (Environment.NewLine);
			}
		}

		sb.Append (Environment.NewLine);
		sb.Append ("	}");
		sb.Append (Environment.NewLine);
		sb.Append ("}");
		
		string code = sb.ToString ();
		
		return code;
	}

	private const string HOST_KEY_NAME = "HOST_";
	private static string GenFormulaHostKeys() {
		StringBuilder sb = new StringBuilder ();
		sb.Append ("///HostKeys 工具自动生成代码，仅供调用，无需编辑");
		sb.Append (Environment.NewLine);
		sb.Append ("using System;");
		sb.Append (Environment.NewLine);
		sb.Append ("namespace FormulaBase {");
		sb.Append (Environment.NewLine);
		sb.Append ("	public class HostKeys {");
		sb.Append (Environment.NewLine);
		
		string[] names = FormulaHostEdit.GetHostNames ();
		if (names != null && names.Length > 0) {
			for (int i = 0; i < names.Length; i++) {
				string _s = names [i];
				sb.Append ("		///");
				sb.Append (Environment.NewLine);
				sb.Append ("		/// " + _s);
				sb.Append (Environment.NewLine);
				sb.Append ("		///");
				sb.Append (Environment.NewLine);
				sb.Append ("		public const int " + HOST_KEY_NAME + i + " = " + i + ";");
				sb.Append (Environment.NewLine);
			}
		}
		
		sb.Append (Environment.NewLine);
		sb.Append ("	}");
		sb.Append (Environment.NewLine);
		sb.Append ("}");
		
		string code = sb.ToString ();
		
		return code;
	}

	private static string GenFormulaSignKeys() {
		StringBuilder sb = new StringBuilder ();
		sb.Append ("///SignKeys 工具自动生成代码，仅供调用，无需编辑");
		sb.Append (Environment.NewLine);
		sb.Append ("using System;");
		sb.Append (Environment.NewLine);
		sb.Append ("namespace FormulaBase {");
		sb.Append (Environment.NewLine);
		sb.Append ("	public class SignKeys {");
		sb.Append (Environment.NewLine);
		
		string[] names = FormulaData.Instance.DynamicParams;
		if (names != null && names.Length > 0) {
			for (int i = 0; i < names.Length; i++) {
				string _s = names [i];
				sb.Append ("		///");
				sb.Append (Environment.NewLine);
				sb.Append ("		/// " + _s);
				sb.Append (Environment.NewLine);
				sb.Append ("		///");
				sb.Append (Environment.NewLine);
				sb.Append ("		public const string " + _s + " = \"" + _s + "\";");
				sb.Append (Environment.NewLine);
			}
		}
		
		sb.Append (Environment.NewLine);
		sb.Append ("	}");
		sb.Append (Environment.NewLine);
		sb.Append ("}");
		
		string code = sb.ToString ();
		
		return code;
	}

	private static string GenConfigKeys(Dictionary<string, string> nameToPath) {
		StringBuilder sb = new StringBuilder ();
		sb.Append ("///SignKeys 工具自动生成代码，仅供调用，无需编辑");
		sb.Append (Environment.NewLine);
		sb.Append ("using System;");
		sb.Append (Environment.NewLine);
		sb.Append ("using System.Collections.Generic;");
		sb.Append (Environment.NewLine);
		sb.Append ("namespace FormulaBase {");
		sb.Append (Environment.NewLine);
		sb.Append ("	public class ConfigKeys {");
		sb.Append (Environment.NewLine);
		sb.Append ("		public static Dictionary<string, string> NamePaths = new Dictionary<string, string> {");
		sb.Append (Environment.NewLine);

		foreach (string name in nameToPath.Keys) {
			sb.Append ("			{ \"" + name + "\", \"" + nameToPath [name] + "\"},");
			Debug.Log (name + "    " + nameToPath [name]);
			sb.Append (Environment.NewLine);
		}

		sb.Append ("		};");
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
	private static void CreateFile(string str, bool isCover, string pathName = PATH_NAME) {
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

	public static void WriteCustomFormula() {
		string codeStr = GenerateCustomFormula ();
		if (codeStr == null) {
			return;
		}

		CreateFile (codeStr, true);
	}

	public static void WriteCustomComponent(string pathName, string componentName, int hostIdx) {
		string codeStr = GenerateCustomComponent (componentName, hostIdx);
		if (codeStr == null) {
			return;
		}

		CreateFile (codeStr, false, pathName + componentName + ".cs");
	}

	private const string KEY_CODE_PATH = "/Scripts/Common/Formula/FormulaKeys.cs";
	public static void WriteFormulaKey() {
		string codeStr = GenFormulaKeys ();
		if (codeStr == null) {
			return;
		}
		
		CreateFile (codeStr, true, KEY_CODE_PATH);
	}

	private const string HOST_KEY_CODE_PATH = "/Scripts/Common/Formula/HostKeys.cs";
	public static void WriteHostKey() {
		string codeStr = GenFormulaHostKeys ();
		if (codeStr == null) {
			return;
		}
		
		CreateFile (codeStr, true, HOST_KEY_CODE_PATH);
	}

	private const string SIGN_KEY_CODE_PATH = "/Scripts/Common/Formula/SignKeys.cs";
	public static void WriteSignKey() {
		string codeStr = GenFormulaSignKeys ();
		if (codeStr == null) {
			return;
		}
		
		CreateFile (codeStr, true, SIGN_KEY_CODE_PATH);
	}

	private const string SIGN_CONFIG_KEY_PATH = "/Scripts/Common/Formula/ConfigKeys.cs";
	public static void WriteConfigKey(Dictionary<string, string> nameToPath) {
		string codeStr = GenConfigKeys (nameToPath);
		if (codeStr == null) {
			return;
		}

		CreateFile (codeStr, true, SIGN_CONFIG_KEY_PATH);
	}
}
