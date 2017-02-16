using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.Linq;
using System.Collections;
using System.Text.RegularExpressions;

/// <summary>
/// Formula edit.
/// </summary>
public class FormulaHostEdit : EditorWindow {
	private const string DEFAULT_NAME = "未命名";
	private const string DEFAULT_FILE_NAME = "UNKNOW";

	private const string DES_1 = "游戏对象包含的属性、公式集合";
	private const string DES_2 = "公式集";
	private const string DES_3 = "动态参数集/默认值";
	private const string DES_4 = "自定义模块路径";
	private const string DES_5 = "测试 看log";

	private int selIdx;
	private FormulaBase.FormulaHost tempHost;
	private FormulaHostStruct currentHost;

	private Vector2 scorllVecAction;
	private Vector2 scorllVecAction2;
	private Vector2 scorllVecAction3;

	private bool isSaving = false;

	[MenuItem("RHY/游戏物件设计/对象管理器")]
	static void Init () {
		FormulaHostEdit window = (FormulaHostEdit)EditorWindow.GetWindow (typeof(FormulaHostEdit));
		window.Show ();
	}

	void OnGUI() {
		this.isSaving = false;
		EditorGUILayout.BeginVertical ();
		this.MkTitle ();
		this.MkHostSelect ();
		this.MkHostFomulaSet ();
		this.MkHostSignSet ();
		this.MkHostComponentSet ();
		EditorGUILayout.EndVertical ();
	}

	private void MkTitle() {
		EditorGUILayout.BeginHorizontal ();
		EditorGUILayout.LabelField (DES_1);
		if (GUILayout.Button ("+")) {
			this.AddHost ();
			this.Repaint ();
			return;
		}
		
		if (GUILayout.Button ("-")) {
			this.DelHost ();
			this.Repaint ();
			return;
		}
		
		if (GUILayout.Button ("Save")) {
			if (this.isSaving) {
				return;
			}

			this.isSaving = true;
			FormulaData.Instance.Hosts [this.selIdx] = this.currentHost;

			string filePath = FormulaData.Instance.componentModulePath;
			if (this.currentHost.fileName != null) {
				filePath += (this.currentHost.fileName + "/");
				FormulaGenerator.CreatePath (filePath);
			}
			
			EditorCommon.SaveFormula ();

			FormulaGenerator.WriteHostKey ();

			if (this.currentHost.componentNames == null || this.currentHost.componentNames.Length <= 0) {
				this.isSaving = false;
				Debug.Log ("Host obj save complete.");
				return;
			}

			for (int k = 0; k < this.currentHost.componentNames.Length; k++) {
				string cName = this.currentHost.componentNames [k];
				if (cName == null) {
					continue;
				}

				FormulaGenerator.WriteCustomComponent (filePath, this.currentHost.fileName + cName, this.selIdx);
			}

			this.isSaving = false;
			Debug.Log ("Host obj save complete.");

			return;
		}

		EditorGUILayout.EndHorizontal ();

		EditorGUILayout.BeginHorizontal ();
		FormulaData.Instance.componentModulePath = EditorGUILayout.TextField (DES_4, FormulaData.Instance.componentModulePath);
		EditorGUILayout.EndHorizontal ();
	}
	
	private void AddHost() {
		int tempIdx = 0;
		if (FormulaData.Instance.Hosts != null) {
			tempIdx = FormulaData.Instance.Hosts.Length + 1;
		}
		
		FormulaHostStruct fhs = new FormulaHostStruct ();
		fhs.name = DEFAULT_NAME + tempIdx;
		fhs.fileName = DEFAULT_FILE_NAME + tempIdx;
		
		FormulaData.Instance.AddHost (ref fhs);
		this.selIdx = FormulaData.Instance.Hosts.Length - 1;
	}
	
	private void DelHost() {
		FormulaData.Instance.DelHost (this.selIdx);
		this.selIdx -= 1;
		if (this.selIdx < 0) {
			this.selIdx = 0;
		}
	}

	// --------------------------------------------------------------------------------------------------

	public static string[] GetHostNames() {
		if (FormulaData.Instance.Hosts == null || FormulaData.Instance.Hosts.Length <= 0) {
			return null;
		}
		
		int len = FormulaData.Instance.Hosts.Length;
		string[] names = new string[len];
		for (int i = 0; i < len; i++) {
			FormulaHostStruct fhs = FormulaData.Instance.Hosts [i];
			names [i] = fhs.name;
		}
		
		return names;
	}

	private void MkHostSelect() {
		string[] names = GetHostNames ();
		if (names == null) {
			return;
		}

		EditorGUILayout.BeginHorizontal ();

		this.selIdx = EditorGUILayout.Popup (this.selIdx, names);
		this.currentHost = FormulaData.Instance.Hosts [this.selIdx];
		this.currentHost.name = EditorGUILayout.TextField (this.currentHost.name);
		this.currentHost.fileName = EditorGUILayout.TextField (this.currentHost.fileName);

		EditorGUILayout.EndHorizontal ();

		FormulaData.Instance.Hosts [this.selIdx] = this.currentHost;
		this.tempHost = FormulaBase.FomulaHostManager.Instance.CreateHost (this.selIdx);
	}

	private void MkHostFomulaSet() {
		if (this.currentHost.name == null) {
			return;
		}

		string[] formulaNames = FormulaEdit.GetFormulasNames ();
		string[] formulaShowName = FormulaEdit.GetFormulasNamesUnderType ();
		if (formulaNames == null || formulaNames.Length <= 0) {
			return;
		}

		EditorGUILayout.BeginHorizontal ();
		this.currentHost.collapsedFormula = EditorGUILayout.Foldout (this.currentHost.collapsedFormula, DES_2);
		if (GUILayout.Button ("+")) {
			int[] sets = this.currentHost.formulaSet;
			FormulaData.Instance.AddIntListItem (0, ref sets);

			this.currentHost.formulaSet = sets;
			FormulaData.Instance.Hosts [this.selIdx] = this.currentHost;

			this.Repaint ();
			return;
		}

		EditorGUILayout.EndHorizontal ();

		FormulaData.Instance.Hosts [this.selIdx] = this.currentHost;

		if (this.currentHost.formulaSet == null || this.currentHost.formulaSet.Length <= 0) {
			return;
		}

		if (this.currentHost.collapsedFormula) {


			this.scorllVecAction = EditorGUILayout.BeginScrollView (this.scorllVecAction);

			EditorGUILayout.BeginVertical ();

			for (int i = 0; i < this.currentHost.formulaSet.Length; i++) {
				EditorGUILayout.BeginHorizontal ();
				this.currentHost.formulaSet [i] = EditorGUILayout.Popup (this.currentHost.formulaSet [i], formulaShowName);
				if (GUILayout.Button ("-")) {
					int[] sets = this.currentHost.formulaSet;
					FormulaData.Instance.DelIntListItem (i, ref sets);
					
					this.currentHost.formulaSet = sets;
					FormulaData.Instance.Hosts [this.selIdx] = this.currentHost;
					
					this.Repaint ();
					return;
				}

				if (GUILayout.Button (DES_5)) {
					if (this.tempHost == null) {
						return;
					}

					this.tempHost = FormulaBase.FomulaHostManager.Instance.CreateHost (this.currentHost);
					int _idx = this.currentHost.formulaSet [i];
					float _val = this.tempHost.Result (_idx);

					Debug.Log (formulaNames [_idx] + " = " + _val);
				}

				EditorGUILayout.EndHorizontal ();
			}

			EditorGUILayout.EndVertical ();

			EditorGUILayout.EndScrollView ();
		}
	}

	private void MkHostSignSet() {
		if (this.currentHost.name == null) {
			return;
		}

		EditorGUILayout.BeginHorizontal ();
		this.currentHost.collapsedSign = EditorGUILayout.Foldout (this.currentHost.collapsedSign, DES_3);
		if (GUILayout.Button ("+")) {
			int[] sets = this.currentHost.singSet;
			FormulaData.Instance.AddIntListItem (0, ref sets);
			
			this.currentHost.singSet = sets;

			float[] values = this.currentHost.signValueSet;
			FormulaData.Instance.AddFloatListItem (0f, ref values);
			
			this.currentHost.signValueSet = values;

			FormulaData.Instance.Hosts [this.selIdx] = this.currentHost;
			
			this.Repaint ();
		}
		
		EditorGUILayout.EndHorizontal ();

		FormulaData.Instance.Hosts [this.selIdx] = this.currentHost;

		if (this.currentHost.singSet == null || this.currentHost.singSet.Length <= 0) {
			return;
		}
		
		if (this.currentHost.collapsedSign) {
			this.scorllVecAction2 = EditorGUILayout.BeginScrollView (this.scorllVecAction2);
			EditorGUILayout.BeginVertical ();

			for (int i = 0; i < this.currentHost.singSet.Length; i++) {
				EditorGUILayout.BeginHorizontal ();
				this.currentHost.singSet [i] = EditorGUILayout.Popup (this.currentHost.singSet [i], FormulaData.Instance.DynamicParams);
				this.currentHost.signValueSet [i] = float.Parse (EditorGUILayout.TextField (this.currentHost.signValueSet [i].ToString ()));

				if (GUILayout.Button ("-")) {
					int[] sets = this.currentHost.singSet;
					FormulaData.Instance.DelIntListItem (i, ref sets);
					
					this.currentHost.singSet = sets;

					float[] values = this.currentHost.signValueSet;
					FormulaData.Instance.DelFloatListItem (i, ref values);
					
					this.currentHost.signValueSet = values;

					FormulaData.Instance.Hosts [this.selIdx] = this.currentHost;
					
					this.Repaint ();
					return;
				}

				EditorGUILayout.EndHorizontal ();
			}
			
			EditorGUILayout.EndVertical ();

			EditorGUILayout.EndScrollView ();
		}
	}

	private void MkHostComponentSet() {
		if (this.currentHost.name == null) {
			return;
		}
		
		EditorGUILayout.BeginHorizontal ();
		this.currentHost.collapsedComponent = EditorGUILayout.Foldout (this.currentHost.collapsedComponent, DES_4);
		if (GUILayout.Button ("+")) {
			int len = 0;
			if (this.currentHost.componentNames != null) {
				len = this.currentHost.componentNames.Length;
			}

			string defaultName = DEFAULT_NAME + len;
			string[] sets = this.currentHost.componentNames;
			FormulaData.Instance.AddStringListItem (ref defaultName, ref sets);
			
			this.currentHost.componentNames = sets;
			FormulaData.Instance.Hosts [this.selIdx] = this.currentHost;
			
			this.Repaint ();
			return;
		}
		
		EditorGUILayout.EndHorizontal ();
		
		FormulaData.Instance.Hosts [this.selIdx] = this.currentHost;
		
		if (this.currentHost.componentNames == null || this.currentHost.componentNames.Length <= 0) {
			return;
		}
		
		if (this.currentHost.collapsedComponent) {
			this.scorllVecAction3 = EditorGUILayout.BeginScrollView (this.scorllVecAction3);
			
			EditorGUILayout.BeginVertical ();
			
			for (int i = 0; i < this.currentHost.componentNames.Length; i++) {
				EditorGUILayout.BeginHorizontal ();

				this.currentHost.componentNames [i] = EditorGUILayout.TextField (this.currentHost.componentNames [i]);

				if (GUILayout.Button ("-")) {
					string[] sets = this.currentHost.componentNames;
					FormulaData.Instance.DelStringListItem (i, ref sets);
					
					this.currentHost.componentNames = sets;
					FormulaData.Instance.Hosts [this.selIdx] = this.currentHost;
					
					this.Repaint ();
					return;
				}
				EditorGUILayout.EndHorizontal ();
			}
			
			EditorGUILayout.EndVertical ();
			
			EditorGUILayout.EndScrollView ();
		}
	}
}