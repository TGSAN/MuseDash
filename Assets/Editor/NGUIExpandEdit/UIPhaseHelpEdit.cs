using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;
using System;
using System.IO;

[CustomEditor(typeof(UIPhaseHelper))]
public class UIPhaseHelpEdit : Editor {
	//string _path = "Resources/ui_resources/";
	//string _assPath = Application.dataPath + "/" + _path;

	private const string TITLE_DES1 = "UIPhaseHelpEdit(编辑完毕后Apply保存)";
	private const string DES1 = "激活条件";
	private const string DES2 = "字符匹配";
	private const string DES3 = "滑动条 变量值/最大值 (两个值类型相同时无效)";
	private const string DES4 = "界面内点击响应对象";
	private const string DES5 = "父节点";
	private const string DES6 = "点击响应是否隐藏父节点";
	private const string DES7 = "点击响应的其它父节点";
	private const string DES8 = "点击后是否隐藏自己";
	private const string DES9 = "自定义匹配关键字";
	private const string DES10 = "是否强制保留";

	private GameObject obj;

	public override void OnInspectorGUI() {
		this.obj = null;
		if (target != null) {
			this.obj = ((UIPhaseHelper)target).gameObject;
		}

		if (this.obj == null) {
			return;
		}

		UIPhaseHelper uph = this.obj.GetComponent<UIPhaseHelper> ();
		if (uph == null) {
			return;
		}

		int condIdx = 0;
		for (int i = 0; i < FormulaData.Instance.DynamicParams.Length; i++) {
			if (FormulaData.Instance.DynamicParams [i] == uph.activeCondiction) {
				condIdx = i;
				break;
			}
		}

		int lbmIdx = 0;
		for (int i = 0; i < FormulaData.Instance.DynamicParams.Length; i++) {
			if (FormulaData.Instance.DynamicParams [i] == uph.labelMatchSign) {
				lbmIdx = i;
				break;
			}
		}

		int sldIdx = 0;
		int sldMaxIdx = 0;
		for (int i = 0; i < FormulaData.Instance.DynamicParams.Length; i++) {
			if (FormulaData.Instance.DynamicParams [i] == uph.sliderMatchSign) {
				sldIdx = i;
				break;
			}
		}

		for (int i = 0; i < FormulaData.Instance.DynamicParams.Length; i++) {
			if (FormulaData.Instance.DynamicParams [i] == uph.sliderMaxSign) {
				sldMaxIdx = i;
				break;
			}
		}

		string[] hostNames = FormulaHostEdit.GetHostNames ();

		EditorGUILayout.BeginVertical ();
		GUILayout.Label (uph.des + " / " + TITLE_DES1, EditorStyles.boldLabel);

		uph.isForceKeep = EditorGUILayout.Toggle (DES10, uph.isForceKeep);
		uph.root = (GameObject)EditorGUILayout.ObjectField (DES5, uph.root, typeof(GameObject), true);

		UIButton _btn = uph.gameObject.GetComponent<UIButton> ();
		if (_btn != null) {
			uph.isClickHideSelf = EditorGUILayout.Toggle (DES8, uph.isClickHideSelf);
			uph.isClickHideParent = EditorGUILayout.Toggle (DES6, uph.isClickHideParent);

			if (uph.clickResponseObjects == null) {
				uph.clickResponseObjects = new List<GameObject> ();
				uph.clickResponseObjectsIsShow = new List<bool> ();
				uph.clickResponseObjectAnimations = new List<string> ();
			}

			if (uph.clickResponseObjects.Count <= 0 || uph.clickResponseObjects [uph.clickResponseObjects.Count - 1] != null) {
				uph.clickResponseObjects.Add (null);
				uph.clickResponseObjectsIsShow.Add (true);
				uph.clickResponseObjectAnimations.Add (string.Empty);
			}

			if (uph.clickResponseParentNames == null) {
				uph.clickResponseParentNames = new List<string> ();
				uph.clickResponseParentNamesIsShow = new List<bool> ();
				uph.clickResponseParentAnimations = new List<string> ();
			}

			if (uph.clickResponseParentNames.Count <= 0 || uph.clickResponseParentNames [uph.clickResponseParentNames.Count - 1] != string.Empty) {
				uph.clickResponseParentNames.Add (string.Empty);
				uph.clickResponseParentNamesIsShow.Add (true);
				uph.clickResponseParentAnimations.Add (string.Empty);
			}

			EditorGUILayout.BeginVertical ();
			uph.isFoldObjs = EditorGUILayout.Foldout (uph.isFoldObjs, DES4 + " / " + uph.clickResponseObjects.Count);
			if (uph.isFoldObjs) {
				for (int i = 0; i < uph.clickResponseObjects.Count; i++) {
					if (uph.clickResponseObjectsIsShow.Count < i + 1) {
						uph.clickResponseObjectsIsShow.Add (true);
					}

					if (uph.clickResponseObjectAnimations.Count < i + 1) {
						uph.clickResponseObjectAnimations.Add (string.Empty);
					}

					GameObject _obj = uph.clickResponseObjects [i];
					EditorGUILayout.BeginHorizontal ();

					uph.clickResponseObjects [i] = (GameObject)EditorGUILayout.ObjectField ("	", uph.clickResponseObjects [i], typeof(GameObject), true);
					uph.clickResponseObjectsIsShow [i] = EditorGUILayout.Toggle (uph.clickResponseObjectsIsShow [i]);

					EditorGUILayout.EndHorizontal ();
				}
			}
			EditorGUILayout.EndVertical ();

			EditorGUILayout.BeginVertical ();

			uph.isFoldParentObjs = EditorGUILayout.Foldout (uph.isFoldParentObjs, DES7 + " / " + uph.clickResponseParentNames.Count);
			if (uph.isFoldParentObjs) {
				/*
				List<string> _temp = new List<string> ();
				DirectoryInfo dInfo = new DirectoryInfo (_assPath);
				FileInfo[] fns = dInfo.GetFiles ();
				foreach (FileInfo _fn in fns) {
					if (_fn.Name [0] == '.') {
						continue;
					}

					if (_fn.Name.Contains (".meta")) {
						continue;
					}

					if (!_fn.Name.Contains (".perfab")) {
						continue;
					}
				}*/

				for (int i = 0; i < uph.clickResponseParentNames.Count; i++) {
					if (uph.clickResponseParentNamesIsShow.Count < i + 1) {
						uph.clickResponseParentNamesIsShow.Add (true);
					}

					if (uph.clickResponseParentAnimations.Count < i + 1) {
						uph.clickResponseParentAnimations.Add (string.Empty);
					}

					EditorGUILayout.BeginHorizontal ();

					uph.clickResponseParentNames [i] = EditorGUILayout.TextField ("	", uph.clickResponseParentNames [i]);
					uph.clickResponseParentAnimations [i] = EditorGUILayout.TextField (uph.clickResponseParentAnimations [i]);
					uph.clickResponseParentNamesIsShow [i] = EditorGUILayout.Toggle (uph.clickResponseParentNamesIsShow [i]);

					EditorGUILayout.EndHorizontal ();
				}
			}

			EditorGUILayout.EndVertical ();
		}

		EditorGUILayout.BeginHorizontal ();
		uph.activeHostKeyId = EditorGUILayout.Popup (DES1, uph.activeHostKeyId, hostNames);
		uph.activeCondiction = FormulaData.Instance.DynamicParams [EditorGUILayout.Popup (condIdx, FormulaData.Instance.DynamicParams)];
		uph.activeValue = EditorGUILayout.TextField (uph.activeValue);
		EditorGUILayout.EndHorizontal ();

		UILabel _lb = uph.gameObject.GetComponent<UILabel> ();
		if (_lb != null) {
			EditorGUILayout.BeginHorizontal ();
			uph.labelHostKeyId = EditorGUILayout.Popup (DES2, uph.labelHostKeyId, hostNames);
			uph.labelMatchSign = FormulaData.Instance.DynamicParams [EditorGUILayout.Popup (lbmIdx, FormulaData.Instance.DynamicParams)];
			EditorGUILayout.EndHorizontal ();
			uph.labelMatchSelfDefineSign = EditorGUILayout.TextField (DES9, uph.labelMatchSelfDefineSign);
		}

		UISlider _sld = uph.gameObject.GetComponent<UISlider> ();
		if (_sld != null) {
			EditorGUILayout.BeginHorizontal ();
			uph.sliderHostKeyId = EditorGUILayout.Popup (DES3, uph.sliderHostKeyId, hostNames);
			uph.sliderMatchSign = FormulaData.Instance.DynamicParams [EditorGUILayout.Popup (sldIdx, FormulaData.Instance.DynamicParams)];
			uph.sliderMaxSign = FormulaData.Instance.DynamicParams [EditorGUILayout.Popup (sldMaxIdx, FormulaData.Instance.DynamicParams)];
			EditorGUILayout.EndHorizontal ();
		}

		EditorGUILayout.EndVertical ();
	}
}