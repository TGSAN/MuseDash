using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.Linq;
using System.Collections;
using System.Text.RegularExpressions;

/// <summary>
/// Formula edit.
/// </summary>
public class FormulaSignEdit : EditorWindow {
	private Vector2 scorllVecAction;
	private bool isSaving = false;
	[MenuItem("RHY/游戏物件设计/动态参数管理器")]
	static void Init () {
		FormulaSignEdit window = (FormulaSignEdit)EditorWindow.GetWindow (typeof(FormulaSignEdit));
		window.Show ();
	}

	void OnGUI() {
		this.isSaving = false;
		EditorGUILayout.BeginVertical ();
		this.MkTitle ();
		this.MkSignSet ();
		EditorGUILayout.EndVertical ();
	}

	private void MkTitle() {
		EditorGUILayout.BeginHorizontal ();
		EditorGUILayout.LabelField ("动态参数集合");
		if (GUILayout.Button ("+")) {
			this.AddSign ();
			this.Repaint ();
		}

		if (GUILayout.Button ("Save")) {
			if (this.isSaving) {
				return;
			}

			this.isSaving = true;
			EditorCommon.SaveFormula ();
			FormulaGenerator.WriteSignKey ();
			this.isSaving = false;

			Debug.Log ("Sign keys save complete.");
		}

		EditorGUILayout.EndHorizontal ();
	}

	// --------------------------------------------------------------------------------------------------

	private void MkSignSet() {
		if (FormulaData.Instance.DynamicParams == null || FormulaData.Instance.DynamicParams.Length <= 0) {
			return;
		}

		this.scorllVecAction = EditorGUILayout.BeginScrollView (this.scorllVecAction);
		EditorGUILayout.BeginVertical ();

		for (int i = 0; i < FormulaData.Instance.DynamicParams.Length; i++) {
			EditorGUILayout.BeginHorizontal ();
			FormulaData.Instance.DynamicParams[i] = EditorGUILayout.TextField (FormulaData.Instance.DynamicParams[i]);
			//if (GUILayout.Button ("-")) {
			//}
			EditorGUILayout.EndHorizontal ();
		}
			
		EditorGUILayout.EndVertical ();

		EditorGUILayout.EndScrollView ();
	}

	private void AddSign() {
		string str = "param";
		EditorData.Instance.AddStringListItem (ref str, ref FormulaData.Instance.DynamicParams);
		EditorCommon.SaveFormula ();
	}
}