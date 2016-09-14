using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.Linq;
using System.Collections;
using System.Text.RegularExpressions;

/// <summary>
/// Formula edit.
/// </summary>
using System.Runtime.InteropServices;
using System;

using StructCopyer;


public class FormulaEdit : EditorWindow {
	private int selIdx = 0;
	private int selIdxByType = 0;
	private int selTypeIdx = 0;
	private bool isSaving = false;
	private FormulaStruct currentFormula;
	
	private Vector2 scorllVecAction;
	
	private string[] formulaNames;
	
	private const string DEFAULT_FORMULA_NANE = "未命名";
	private const string DES_1 = "数据来源";
	private const string DES_2 = "+ 条件";
	private const string DES_3 = "公式选择";
	private const string DES_4 = "公式刷新";
	private const string DES_5 = "计算公式编辑器";
	private const string DES_6 = "+ 公式";
	private const string DES_7 = "  +条件";
	private const string DES_8 = "偏移量";
	private const string DES_9 = "公式类型";
	private const string DES_10 = "根据类型选择";
	private const string DES_11 = "同名参数标准";
	
	private static string[] DATA_SOURCE_TYPE = new string[] {
		"无",
		"配置数据(条件输入值为配置索引,默认0)",
		"引用公式(条件输入值为公式结果)",
		"动态数据(条件输入值为动态数据)",
		"固定基础值",
		"参数型配置数据(条件输入值为配置索引,默认0)"
	};
	private static string[] DATA_SOURCE_TYPE_DES1 = new string[]{"无", "配置表名.列名(填写格式)", "公式名", "动态数据源名", "基础值"};
	
	private static string[] CONDICTION_PARAM_TYPE = new string[]{
		"无",
		"动态参数数量(*偏移量)",
		"大于等于公式(+偏移量)",
		"增加偏移量",
		"等于(+偏移量)",
		"随机数大于等于(+偏移量)",
		"增加随机数",
		"比较(+偏移量)",
		"大于等于固定值(+偏移量)",
		"加配置数据"
	};
	private static string[] CONDICTION_PARAM_DES1 = new string[]{
		"",
		"动态参数名",
		"公式名",
		"",
		"动态参数名",
		"动态参数名",
		"--",
		"--",
		"动态参数名",
		"动态参数名"
	};
	private static string[] CONDICTION_PARAM_DES2 = new string[]{
		"",
		"动态参数量上限(0表示无限)",
		"值",
		"",
		"值",
		"百分比(整数)",
		"--",
		"<=",
		">=",
		"配置表名.列名",
	};
	
	[MenuItem("RHY/游戏物件设计/公式管理器")]
	static void Init () {
		
		FormulaEdit window = (FormulaEdit)EditorWindow.GetWindow (typeof(FormulaEdit));
		window.Show ();
	}
	
	void OnGUI() {
		this.isSaving = false;
		this.formulaNames = GetFormulasNames ();
		
		EditorGUILayout.BeginVertical ();
		this.MkTitle ();
		this.MkFormulaSelect ();
		this.MkFormulaForm ();
		this.MkFormulaParams ();
		EditorGUILayout.EndVertical ();
	}
	
	private void MkTitle() {
		EditorGUILayout.BeginHorizontal ();
		EditorGUILayout.LabelField (DES_5);

		//this.selTypeIdx = EditorGUILayout.Popup (DES_9, this.selTypeIdx, FormulaData.Instance.FormulaTypeNames);
		//this.selIdxByType = EditorGUILayout.Popup (this.selIdxByType, this.GetFormulasNames (this.selTypeIdx));

		if (GUILayout.Button (DES_10)) {
			this.selIdx = this.GetTrueIdxByType ();
			this.Repaint ();
			return;
		}

		if (GUILayout.Button (DES_6)) {
			this.selIdx = this.AddFormula ();
			this.formulaNames = GetFormulasNames ();
			this.Repaint ();
			return;
		}
		
		if (GUILayout.Button ("Save")) {
			this.SaveFormula ();
			return;
		}

		if (GUILayout.Button ("Copy")) {
			this.CopyFromula ();
			return;
		}
		
		EditorGUILayout.EndHorizontal ();
	}

	public string[] GetFormulasNames(int fType) {
		if (FormulaData.Instance.Formulas == null || FormulaData.Instance.Formulas.Length <= 0) {
			return null;
		}

		List<string> ss = new List<string> ();
		for (int i = 0; i < FormulaData.Instance.Formulas.Length; i++) {
			FormulaStruct fs = FormulaData.Instance.Formulas [i];
			if (fs.typeIdx != fType) {
				continue;
			}

			ss.Add (fs.name);
		}
		
		return ss.ToArray ();
	}
	
	public static string[] GetFormulasNames() {
		if (FormulaData.Instance.Formulas == null || FormulaData.Instance.Formulas.Length <= 0) {
			return null;
		}
		
		string[] ss = new string[FormulaData.Instance.Formulas.Length];
		for (int i = 0; i < FormulaData.Instance.Formulas.Length; i++) {
			ss [i] = FormulaData.Instance.Formulas [i].name;
		}
		
		return ss;
	}

	public static string[] GetFormulasNamesUnderType() {
		if (FormulaData.Instance.Formulas == null || FormulaData.Instance.Formulas.Length <= 0) {
			return null;
		}

		if (FormulaData.Instance.FormulaTypeNames == null || FormulaData.Instance.FormulaTypeNames.Length <= 0) {
			Debug.Log ("No type record");
			return null;
		}
		
		string[] ss = new string[FormulaData.Instance.Formulas.Length];
		for (int i = 0; i < FormulaData.Instance.Formulas.Length; i++) {
			FormulaStruct fs = FormulaData.Instance.Formulas [i];
			string tname = FormulaData.Instance.FormulaTypeNames [fs.typeIdx];
			ss [i] = tname + "/" + fs.name;
		}
		
		return ss;
	}

	private int GetTrueIdxByType() {
		if (FormulaData.Instance.Formulas == null || FormulaData.Instance.Formulas.Length <= 0) {
			return 0;
		}

		string[] fnames = this.GetFormulasNames (this.selTypeIdx);
		if (fnames == null || fnames.Length <= 0) {
			return 0;
		}

		string _fname = fnames [this.selIdxByType];
		for (int i = 0; i < FormulaData.Instance.Formulas.Length; i++) {
			FormulaStruct fs = FormulaData.Instance.Formulas [i];
			if (fs.typeIdx == this.selTypeIdx && fs.name == _fname) {
				return i;
			}
		}

		return 0;
	}
	// --------------------------------------------------------------------------------------------------
	private void CopyFromula() {
		if (this.formulaNames == null || this.formulaNames.Length <= 0) {
			return;
		}

		if (this.selIdx < 0 || this.selIdx >= this.formulaNames.Length) {
			return;
		}

		FormulaStruct orgFs = FormulaData.Instance.Formulas [this.selIdx];
		FormulaStruct dstFs = StructCopyer.StructCopyer.Copy<FormulaStruct> (orgFs);
		dstFs.idx = FormulaData.Instance.Formulas.Length;
		dstFs.name += ("copy" + dstFs.idx);

		Debug.Log (orgFs.idx + "/" + orgFs.name + " copy new : " + dstFs.idx + "/" + dstFs.name);
		FormulaData.Instance.AddFormula (ref dstFs);
		EditorCommon.SaveFormula ();

		this.selIdx = dstFs.idx;
		this.formulaNames = GetFormulasNames ();
		this.Repaint ();
	}
	
	private void MkFormulaSelect() {
		if (this.formulaNames == null || this.formulaNames.Length <= 0) {
			return;
		}
		
		EditorGUILayout.BeginHorizontal ();
		
		this.selIdx = Mathf.Min (this.selIdx, this.formulaNames.Length - 1);
		// GUIStyle gs = new GUIStyle ();
		this.selIdx = EditorGUILayout.Popup (DES_3, this.selIdx, this.GetFormulasNamesUnderType ());
		
		string title = this.formulaNames [this.selIdx];
		FormulaData.Instance.Formulas [this.selIdx].name = EditorGUILayout.TextField (title);

		int typeIdx = FormulaData.Instance.Formulas [this.selIdx].typeIdx;
		FormulaData.Instance.Formulas [this.selIdx].typeIdx = EditorGUILayout.Popup (typeIdx, FormulaData.Instance.FormulaTypeNames);

		if (GUILayout.Button (DES_4)) {
			this.OnSelectFormula ();
			return;
		}
		
		EditorGUILayout.EndHorizontal ();
	}
	
	private void OnSelectFormula() {
		this.FormulaAnalysis ();
		// this.formulaNames = GetFormulasNames ();
		this.Repaint ();
	}
	
	private void SaveFormula() {
		if (this.isSaving) {
			return;
		}
		
		this.isSaving = true;
		FormulaGenerator.WriteFormulaKey ();
		FormulaGenerator.WriteCustomFormula ();
		EditorCommon.SaveFormula ();
		this.isSaving = false;
		Debug.Log ("FormulaEdit save complete.");
	}
	
	private int AddFormula() {
		FormulaStruct fs = new FormulaStruct ();
		fs.formula = "()";
		fs.idx = 0;
		if (FormulaData.Instance.Formulas != null) {
			fs.idx = FormulaData.Instance.Formulas.Length;
		}
		
		fs.name = DEFAULT_FORMULA_NANE + fs.idx;
		
		FormulaData.Instance.AddFormula (ref fs);
		EditorCommon.SaveFormula ();
		
		return fs.idx;
	}
	
	private void AddCondiction(FormulaParamStruct formulaParam) {
		FormulaParamCondictionStruct fpcs = new FormulaParamCondictionStruct ();
		fpcs.condictionType = FormulaBase.FormulaBase.CONDICTION_TYPE_NONE;
		fpcs.condictionKeyIndex = 0;
		fpcs.condictionKey = "";
		
		this.__AddCondiction (ref formulaParam, ref fpcs);
		
		EditorCommon.SaveFormula ();
	}
	
	private void __AddCondiction(ref FormulaParamStruct parent, ref FormulaParamCondictionStruct item) {
		List<FormulaParamCondictionStruct> _list;
		if (parent.condictions != null && parent.condictions.Length > 0) {
			_list = parent.condictions.ToList ();
		} else {
			_list = new List<FormulaParamCondictionStruct> ();
		}
		
		_list.Add (item);
		
		parent.condictions = _list.ToArray ();
		this.currentFormula.fparams [parent.idx] = parent;
		FormulaData.Instance.Formulas [this.selIdx] = this.currentFormula;
	}
	
	private void DeleteCondiction(int idx, ref FormulaParamStruct parent) {
		if (parent.condictions == null) {
			return;
		}
		
		if (idx >= parent.condictions.Length) {
			return;
		}
		
		List<FormulaParamCondictionStruct> _list = parent.condictions.ToList ();
		_list.RemoveAt (idx);
		
		parent.condictions = _list.ToArray ();
		this.currentFormula.fparams [parent.idx] = parent;
		FormulaData.Instance.Formulas [this.selIdx] = this.currentFormula;
		
		EditorCommon.SaveFormula ();
	}
	// --------------------------------------------------------------------------------------------------
	private string[] GetParamNamesFromFormulaStr(string formula) {
		if (formula == null) {
			return null;
		}
		
		string[] ss = this.currentFormula.formula.Split (new char[] { FormulaBase.FormulaBase.SPLITE_UNIT });
		if (ss == null) {
			return null;
		}
		
		List<string> temp = new List<string> ();
		for (int i = 0; i < ss.Length; i++) {
			string _s = ss [i];
			if (_s == null || _s.Length < 1 || (!FormulaBase.FormulaBase.IsChinese (_s [0].ToString ()) && !FormulaBase.FormulaBase.IsSignMatch (_s))) {
				continue;
			}
			
			temp.Add (_s);
		}
		
		return temp.ToArray ();
	}
	
	private void FormulaAnalysis() {
		string[] currList = this.GetParamNamesFromFormulaStr (this.currentFormula.formula);
		
		if (currList == null || currList.Length <= 0) {
			return;
		}
		
		if (this.currentFormula.fparams == null) {
			this.currentFormula.fparams = new FormulaParamStruct[currList.Length];
		}
		
		int idx = 0;
		int len = currList.Length;
		List<FormulaParamStruct> result = new List<FormulaParamStruct> ();
		for (int i = 0; i < len; i++) {
			bool contains = false;
			string _s = currList [i];
			for (int k = idx; k < this.currentFormula.fparams.Length; k++) {
				FormulaParamStruct _fps = this.currentFormula.fparams [k];
				_fps.idx = i;
				if (_fps.paramname == _s) {
					result.Add (_fps);
					idx = k;
					contains = true;
					break;
				}
			}
			
			if (!contains) {
				FormulaParamStruct fps = new FormulaParamStruct ();
				fps.idx = i;
				fps.paramname = _s;
				fps.dataSourceType = FormulaBase.FormulaBase.DATA_SOURCE_NONE;
				result.Add (fps);
			}
		}
		
		this.currentFormula.fparams = result.ToArray ();
		FormulaData.Instance.Formulas [this.selIdx] = this.currentFormula;
	}
	
	private void MkFormulaForm() {
		if (this.formulaNames == null || this.formulaNames.Length <= 0) {
			return;
		}
		
		string title = this.formulaNames [this.selIdx];
		
		EditorGUILayout.BeginVertical ();
		
		this.currentFormula = FormulaData.Instance.Formulas [this.selIdx];
		this.currentFormula.formula = EditorGUILayout.TextField (title + " =", this.currentFormula.formula);
		
		EditorGUILayout.EndVertical ();
		
		FormulaData.Instance.Formulas [this.selIdx] = this.currentFormula;
	}
	
	private void MkFormulaParams() {
		if (this.formulaNames == null || this.formulaNames.Length <= 0) {
			return;
		}
		
		if (this.currentFormula.fparams == null || this.currentFormula.fparams.Length <= 0) {
			return;
		}
		
		this.scorllVecAction = EditorGUILayout.BeginScrollView (this.scorllVecAction);
		EditorGUILayout.BeginVertical ();
		
		for (int i = 0; i < this.currentFormula.fparams.Length; i++) {
			this.MkFormulaParamForm (i);
		}
		
		EditorGUILayout.EndVertical ();
		EditorGUILayout.EndScrollView ();
	}
	
	private void MkFormulaParamForm(int idx) {
		FormulaParamStruct fp = this.currentFormula.fparams [idx];
		
		EditorGUILayout.BeginHorizontal ();
		
		fp.collapsed = EditorGUILayout.Foldout (fp.collapsed, fp.paramname);
		fp.dataSourceType = EditorGUILayout.Popup (DES_1, fp.dataSourceType, DATA_SOURCE_TYPE);
		
		if (fp.dataSourceType == FormulaBase.FormulaBase.DATA_SOURCE_NONE) {
			// Nothing
		} else if (fp.dataSourceType == FormulaBase.FormulaBase.DATA_SOURCE_CONFIG) {
			fp.dataSourceName = EditorGUILayout.TextField (DATA_SOURCE_TYPE_DES1 [fp.dataSourceType], fp.dataSourceName);
		} else if (fp.dataSourceType == FormulaBase.FormulaBase.DATA_SOURCE_BASE_VALUE) {
			fp.dataSourceValue = float.Parse (EditorGUILayout.TextField (DATA_SOURCE_TYPE_DES1 [fp.dataSourceType], fp.dataSourceValue.ToString ()));
		} else if (fp.dataSourceType == FormulaBase.FormulaBase.DATA_SOURCE_PARAM_CONFIG) {
			if (FormulaData.Instance.ConfigReaders != null && FormulaData.Instance.ConfigReaders.Count > 0) {
				string[] ars = FormulaData.Instance.ConfigReaders.ToArray ();
				if (!ars.Contains (fp.dataSourceName)) {
					fp.dataSourceName = ars [0];
				}

				int _idx = 0;
				foreach (string _dsn in ars) {
					if (_dsn == fp.dataSourceName) {
						break;
					}

					_idx++;
				}

				_idx = EditorGUILayout.Popup (_idx, ars);
				fp.dataSourceName = ars [_idx];
			}
		} else {
			string[] list = null;
			string[] listSourceName = null;
			if (fp.dataSourceType == FormulaBase.FormulaBase.DATA_SOURCE_DYNAMIC) {
				list = FormulaData.Instance.DynamicParams;
				listSourceName = FormulaData.Instance.DynamicParams;
			} else if (fp.dataSourceType == FormulaBase.FormulaBase.DATA_SOURCE_QUOTE_FORMULA) {
				list = this.GetFormulasNamesUnderType ();
				listSourceName = this.formulaNames;
			}
			
			if (list != null) {
				fp.dataSourceIndex = EditorGUILayout.Popup (DATA_SOURCE_TYPE_DES1 [fp.dataSourceType], fp.dataSourceIndex, list);
				if (fp.dataSourceIndex < list.Length) {
					fp.dataSourceName = listSourceName [fp.dataSourceIndex];
				}
			}
		}
		
		if (GUILayout.Button (DES_2)) {
			this.AddCondiction (fp);
			this.Repaint ();
			return;
		}

		if (GUILayout.Button (DES_11)) {
			this.SameNameParamAnalysis (fp);
			this.Repaint ();
			return;
		}
		
		EditorGUILayout.EndHorizontal ();
		
		this.currentFormula.fparams [idx] = fp;
		FormulaData.Instance.Formulas [this.selIdx] = this.currentFormula;
		
		if (fp.collapsed) {
			EditorGUILayout.BeginVertical ();
			if (fp.condictions != null && fp.condictions.Length > 0) {
				for (int i = 0; i < fp.condictions.Length; i++) {
					this.MkParamCondiction (idx, i);
				}
			}
			
			EditorGUILayout.EndVertical ();
		}
	}

	private void SameNameParamAnalysis(FormulaParamStruct formulaParam) {
		if (this.currentFormula.fparams == null || this.currentFormula.fparams.Length <= 0) {
			return;
		}

		for (int i = 0; i < this.currentFormula.fparams.Length; i++) {
			FormulaParamStruct _fp = this.currentFormula.fparams [i];
			if (_fp.idx == formulaParam.idx || _fp.paramname != formulaParam.paramname) {
				continue;
			}

			FormulaParamStruct _fpCopy = StructCopyer.StructCopyer.Copy<FormulaParamStruct> (formulaParam);
			_fpCopy.idx = i;
			this.currentFormula.fparams [i] = _fpCopy;
		}

		FormulaData.Instance.Formulas [this.selIdx] = this.currentFormula;
	}
	
	private void MkParamCondiction(int idx, int cIdx) {
		FormulaParamStruct fp = this.currentFormula.fparams [idx];
		if (fp.paramname == null) {
			return;
		}
		
		if (fp.condictions == null || fp.condictions.Length <= 0) {
			return;
		}
		
		FormulaParamCondictionStruct fpc = fp.condictions [cIdx];
		
		EditorGUILayout.BeginHorizontal ();
		
		fpc.condictionType = EditorGUILayout.Popup (DES_7 + cIdx, fpc.condictionType, CONDICTION_PARAM_TYPE);
		
		string[] list = null;
		string[] list2 = null;
		if (fpc.condictionType == FormulaBase.FormulaBase.CONDICTION_TYPE_SIGN) {
			list = FormulaData.Instance.DynamicParams;
		} else if (fpc.condictionType == FormulaBase.FormulaBase.CONDICTION_TYPE_BASE_VALUE) {
			// Do Nothing
		} else if (fpc.condictionType == FormulaBase.FormulaBase.CONDICTION_TYPE_OVER_VALUE) {
			list = this.GetFormulasNamesUnderType ();
		} else if (fpc.condictionType == FormulaBase.FormulaBase.CONDICTION_TYPE_EQUAL_VALUE) {
			list = FormulaData.Instance.DynamicParams;
		} else if (fpc.condictionType == FormulaBase.FormulaBase.CONDICTION_TYPE_OVER_RATE) {
			fpc.condictionValue = float.Parse (EditorGUILayout.TextField (CONDICTION_PARAM_DES2 [fpc.condictionType], fpc.condictionValue.ToString ()));
		} else if (fpc.condictionType == FormulaBase.FormulaBase.CONDICTION_TYPE_RATE) {
			// Do Nothing
		} else if (fpc.condictionType == FormulaBase.FormulaBase.CONDICTION_TYPE_OVER_FORMULA) {
			list = this.GetFormulasNamesUnderType ();
			list2 = this.GetFormulasNamesUnderType ();
		} else if (fpc.condictionType == FormulaBase.FormulaBase.CONDICTION_TYPE_OVER_FIX_VALUE) {
			list = FormulaData.Instance.DynamicParams;
		} else if (fpc.condictionType == FormulaBase.FormulaBase.CONDICTION_TYPE_CFG_VALUE) {
			list = FormulaData.Instance.DynamicParams;
		}
		
		if (list != null) {
			fpc.condictionKeyIndex = EditorGUILayout.Popup (CONDICTION_PARAM_DES1 [fpc.condictionType], fpc.condictionKeyIndex, list);
			if (fpc.condictionKeyIndex >= list.Length) {
				fpc.condictionKeyIndex = 0;
			}

			if (fpc.condictionType != FormulaBase.FormulaBase.CONDICTION_TYPE_CFG_VALUE) {
				fpc.condictionKey = list [fpc.condictionKeyIndex];
			}

			if (list2 == null) {
				if (fpc.condictionType != FormulaBase.FormulaBase.CONDICTION_TYPE_CFG_VALUE) {
					fpc.condictionValue = float.Parse (EditorGUILayout.TextField (CONDICTION_PARAM_DES2 [fpc.condictionType], fpc.condictionValue.ToString ()));
				} else {
					fpc.condictionKey = EditorGUILayout.TextField (CONDICTION_PARAM_DES2 [fpc.condictionType], fpc.condictionKey);
				}
			} else {
				fpc.condictionValue = (float)(EditorGUILayout.Popup (CONDICTION_PARAM_DES2 [fpc.condictionType], (int)fpc.condictionValue, list2));
			}
		}
		
		fpc.plus = float.Parse (EditorGUILayout.TextField (DES_8, fpc.plus.ToString ()));
		if (GUILayout.Button ("-")) {
			this.DeleteCondiction (cIdx, ref fp);
			this.Repaint ();
			return;
		}
		
		EditorGUILayout.EndHorizontal ();
		
		fp.condictions [cIdx] = fpc;
		this.currentFormula.fparams [idx] = fp;
		FormulaData.Instance.Formulas [this.selIdx] = this.currentFormula;
	}
}