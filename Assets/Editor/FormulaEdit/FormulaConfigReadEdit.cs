using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.Linq;
using System.Collections;
using System.Text.RegularExpressions;

/// <summary>
/// Formula edit.
/// </summary>
public class FormulaConfigReadEdit : EditorWindow {
	private const char READER_SPLITE = '-';
	private const char PART_SPLITE = '_';

	private const string DES1 = "选择配置表";
	private const string DES2 = "读取规则选择";

	private bool isSaving = false;
	private int vidx;
	private int ridx;
	private string cfgname;
	private string strreader;

	[MenuItem("RHY/游戏物件设计/配置读取器")]
	static void Init () {
		FormulaConfigReadEdit window = (FormulaConfigReadEdit)EditorWindow.GetWindow (typeof(FormulaConfigReadEdit));
		window.Show ();
	}

	void OnGUI() {
		this.isSaving = false;
		EditorGUILayout.BeginVertical ();
		this.MkTitle ();
		this.MkReaderSet ();
		EditorGUILayout.EndVertical ();
	}

	private void MkTitle() {
		if (FormulaBase.ConfigKeys.NamePaths == null || FormulaBase.ConfigKeys.NamePaths.Count <= 0) {
			return;
		}

		List<string> _temp = new List<string> (FormulaBase.ConfigKeys.NamePaths.Keys);
		string[] _arrayCfg = _temp.ToArray ();
		string[] _arrayReader = FormulaData.Instance.ConfigReaders.ToArray ();

		if (this.vidx >= _temp.Count) {
			this.vidx = 0;
		}

		if (_arrayReader != null && this.ridx >= _arrayReader.Length) {
			this.ridx = 0;
		}


		EditorGUILayout.BeginVertical ();
		this.vidx = EditorGUILayout.Popup (DES1, this.vidx, _arrayCfg);
		this.cfgname = _arrayCfg [this.vidx];

		EditorGUILayout.BeginHorizontal ();
		if (GUILayout.Button ("+")) {
			this.AddReader ();
			this.Repaint ();
		}

		if (GUILayout.Button ("Save")) {
			if (this.isSaving) {
				return;
			}

			this.isSaving = true;
			EditorCommon.SaveFormula ();
			this.isSaving = false;

			Debug.Log ("Config reader keys save complete.");
		}

		EditorGUILayout.EndHorizontal ();

		if (_arrayReader != null && _arrayReader.Length > 0) {
			this.ridx = EditorGUILayout.Popup (DES2, this.ridx, _arrayReader);
			this.strreader = _arrayReader [this.ridx];
		}

		EditorGUILayout.EndVertical ();
	}

	// --------------------------------------------------------------------------------------------------

	private void MkReaderSet() {
		if (this.cfgname == null || this.strreader == null) {
			return;
		}


		EditorGUILayout.BeginVertical ();

		this.strreader = EditorGUILayout.TextField (this.strreader);
		FormulaData.Instance.ConfigReaders [this.ridx] = this.strreader;

		EditorGUILayout.EndVertical ();
	}

	private void AddReader() {
		if (this.cfgname == null) {
			return;
		}

		string defaultReader = this.cfgname + "/none";
		if (FormulaData.Instance.ConfigReaders == null) {
			FormulaData.Instance.ConfigReaders = new List<string> ();
		}

		if (FormulaData.Instance.ConfigReaders.Contains (defaultReader)) {
			return;
		}

		FormulaData.Instance.ConfigReaders.Add (defaultReader);
		EditorCommon.SaveFormula ();
	}
}