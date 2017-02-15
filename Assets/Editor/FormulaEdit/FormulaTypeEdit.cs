using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.Linq;
using System.Collections;
using System.Text.RegularExpressions;

/// <summary>
/// Formula edit.
/// </summary>
public class FormulaTypeEdit : EditorWindow {
	private Vector2 scorllVecAction;
	private bool isSaving = false;
	[MenuItem("RHY/游戏物件设计/公式分类管理器")]
	static void Init () {
		FormulaTypeEdit window = (FormulaTypeEdit)EditorWindow.GetWindow (typeof(FormulaTypeEdit));
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
		EditorGUILayout.LabelField ("公式分类");
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

			Debug.Log ("Formula types save complete.");
		}

		EditorGUILayout.EndHorizontal ();
	}

	// --------------------------------------------------------------------------------------------------

	private void MkSignSet() {
		if (FormulaData.Instance.FormulaTypeNames == null || FormulaData.Instance.FormulaTypeNames.Length <= 0) {
			return;
		}

		this.scorllVecAction = EditorGUILayout.BeginScrollView (this.scorllVecAction);
		EditorGUILayout.BeginVertical ();

		for (int i = 0; i < FormulaData.Instance.FormulaTypeNames.Length; i++) {
			EditorGUILayout.BeginHorizontal ();
			FormulaData.Instance.FormulaTypeNames[i] = EditorGUILayout.TextField (FormulaData.Instance.FormulaTypeNames[i]);
			//if (GUILayout.Button ("-")) {
			//}
			EditorGUILayout.EndHorizontal ();
		}
			
		EditorGUILayout.EndVertical ();

		EditorGUILayout.EndScrollView ();
	}

	private void AddSign() {
		string str = "default type";
		EditorData.Instance.AddStringListItem (ref str, ref FormulaData.Instance.FormulaTypeNames);
		EditorCommon.SaveFormula ();
	}
}