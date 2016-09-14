using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.Linq;
using System.Collections;
using System.Text.RegularExpressions;
using LitJson;
/// <summary>
/// Formula edit.
/// </summary>
using System;
using FormulaBase;
using cn.bmob.json;
using cn.bmob.io;
using cn.bmob.api;

public class UIPhaseEdit : EditorWindow {
	private static System.Reflection.Assembly assembly = System.Reflection.Assembly.Load("Assembly-CSharp");

	private const string CODE_PATH = "/Scripts/UI/";
	private const string DES_1 = "UI分析工具(任意编辑后点Apply保存) 相关代码路径 : ";
	private const string DES_2 = "UI分析";
	private const string DES_3 = "未有描述";
	private const string DES_4 = "支持多语言";
	private const string DES_5 = "UI主模块";
	private const string DES_6 = "出现动画";
	private const string DES_7 = "消失动画";
	//private const string DES_8 = "所属场景的UI根节点(可无视)";
	private const string DES_9 = "需分析的UI(要分析的拖进这里)";
	private const string DES_10 = "是否加载即显示";

	private Vector2 scorllVecAction;

	//private GameObject _sceneRootObject = null;
	private GameObject _uiObject = null;
	private string _moduleName = null;

	[MenuItem("RHY/UI分析工具")]
	static void Init () {
		UIPhaseEdit window = (UIPhaseEdit)EditorWindow.GetWindow (typeof(UIPhaseEdit));
		window.Show ();
	}

	void OnGUI() {
		EditorGUILayout.BeginVertical ();
		this.MkTitle ();
		EditorGUILayout.EndVertical ();
	}

	private void MkTitle() {
		EditorGUILayout.BeginHorizontal ();
		EditorGUILayout.LabelField (DES_1 + this._moduleName);
		//if (GUILayout.Button ("Save")) {
		//	this.Save ();
		//}

		EditorGUILayout.EndHorizontal ();

		this.NGUIPhaseHelper ();
	}

	private void NGUIPhaseHelper() {
		if (this._uiObject != null) {
			this._moduleName = CODE_PATH + this._uiObject.name + "/";
		}

		EditorGUILayout.BeginVertical ();

		//this._sceneRootObject = (GameObject)EditorGUILayout.ObjectField (DES_8, this._sceneRootObject, typeof(GameObject), true);

		EditorGUILayout.BeginHorizontal ();
		this._uiObject = (GameObject)EditorGUILayout.ObjectField (DES_9, this._uiObject, typeof(GameObject), true);
		if (GUILayout.Button (DES_2)) {
			this.DeepPhaseUI ();
			return;
		}

		EditorGUILayout.EndHorizontal ();

		this.InitShowHideAnimation ();
		this.InitPhaseUI ();

		EditorGUILayout.EndVertical ();
	}

	private void InitShowHideAnimation() {
		if (this._uiObject == null) {
			return;
		}

		UIRootHelper rootHelper = this._uiObject.GetComponent<UIRootHelper> ();
		if (rootHelper == null) {
			return;
		}

		rootHelper.isShowOnLoaded = EditorGUILayout.Toggle (DES_10, rootHelper.isShowOnLoaded);

		UIPhaseBase upb = this._uiObject.GetComponent<UIPhaseBase> ();
		if (upb == null) {
			return;
		}

		upb.ShowAnimation = EditorGUILayout.TextField (DES_6, upb.ShowAnimation);
		upb.HideAnimation = EditorGUILayout.TextField (DES_7, upb.HideAnimation);
	}

	private void InitPhaseUI() {
		if (this._uiObject == null) {
			return;
		}

		UIRootHelper rootHelper = this._uiObject.GetComponent<UIRootHelper> ();
		if (rootHelper == null) {
			return;
		}

		List<UIPhaseHelper> _phaser = rootHelper.widgets;
		if (_phaser == null) {
			return;
		}

		this.scorllVecAction = EditorGUILayout.BeginScrollView (this.scorllVecAction);
		foreach (UIPhaseHelper s in _phaser) {
			EditorGUILayout.BeginVertical ();

			EditorGUILayout.BeginHorizontal ();
			s.des = EditorGUILayout.TextField (s.des);
			UnityEngine.Object _obj = EditorGUILayout.ObjectField (s.gameObject, typeof(GameObject), true);
			s.isNeedTranslate = EditorGUILayout.Toggle (DES_4, s.isNeedTranslate);
			EditorGUILayout.SelectableLabel (s.onClickModuleName + " / " + s.onTweenAniFinishedModuleName);
			EditorGUILayout.EndHorizontal ();

			EditorGUILayout.EndVertical ();
		}

		EditorGUILayout.EndScrollView ();
	}

	private void Save() {
		if (this._uiObject != null) {
			//EditorUtility.SetDirty (this._uiObject);
			UnityEngine.Object _prefab2 = PrefabUtility.GetPrefabParent (this._uiObject);
			PrefabUtility.ReplacePrefab (this._uiObject, _prefab2);
		}

		EditorApplication.SaveAssets ();
	}

	private void DeepPhaseUI() {
		/*
		if (this._sceneRootObject != null) {
			UISceneHelper ush = this._sceneRootObject.GetComponent<UISceneHelper> ();
			if (ush == null) {
				ush = this._sceneRootObject.AddComponent<UISceneHelper> ();
			}

			//UnityEngine.Object _obj = PrefabUtility.GetPrefabParent (this._uiObject);
			//ush.AddWidget (_obj);
		}
		*/
		
		if (this._uiObject == null) {
			return;
		}

		UIRootHelper rootHelper = this._uiObject.GetComponent<UIRootHelper> ();
		if (rootHelper == null) {
			rootHelper = this._uiObject.AddComponent<UIRootHelper> ();
		}

		if (rootHelper.widgets == null) {
			rootHelper.widgets = new List<UIPhaseHelper> ();
		}

		rootHelper.widgets.Clear ();
		/**/
		Component mainComp = rootHelper.gameObject.GetComponent (rootHelper.gameObject.name);
		if (mainComp == null) {
			string path = CODE_PATH + this._uiObject.name + "/";
			UIPhaserMainGenerator.WriteCode (path, this._uiObject.name, rootHelper.gameObject.name, rootHelper.gameObject.name + DES_5);
			Type t = assembly.GetType (rootHelper.gameObject.name + "." + rootHelper.gameObject.name);
			rootHelper.gameObject.AddComponent (t);
		}

		this.__PhaseObject (this._uiObject);
		this.InitPhaseUI ();
	}

	private void __PhaseObject(GameObject obj) {
		if (obj == null || obj.transform == null || obj.transform.childCount <= 0) {
			return;
		}

		for (int i = 0; i < obj.transform.childCount; i++) {
			GameObject cobj = obj.transform.GetChild (i).gameObject;
			this.__PhaseByComponent (cobj);
			this.__PhaseObject (cobj);
		}
	}

	private void __PhaseByComponent(GameObject obj) {
		if (obj == null) {
			return;
		}

		GameObject uiRoot = PrefabUtility.FindValidUploadPrefabInstanceRoot (this._uiObject);
		GameObject objRoot = PrefabUtility.FindValidUploadPrefabInstanceRoot (obj);
		if (uiRoot.name != objRoot.name) {
			Debug.Log ("独立预设 " + objRoot.name + " 不参与解释");
			return;
		}

		UIRootHelper rootHelper = this._uiObject.GetComponent<UIRootHelper> ();
		if (rootHelper == null) {
			return;
		}

		UIButton _btn = obj.GetComponent<UIButton> ();
		UITweener _twr = obj.GetComponent<UITweener> ();
		UILabel _lb = obj.GetComponent<UILabel> ();
		UISlider _sld = obj.GetComponent<UISlider> ();
		UIScrollView _srv = obj.GetComponent<UIScrollView> ();
		UIPhaseHelper _uph = obj.GetComponent<UIPhaseHelper> ();
		if (_btn == null && _twr == null && _lb == null && _sld == null && _srv == null) {
			if (_uph != null) {
				if (!_uph.isForceKeep) {
					GameObject.DestroyImmediate (_uph);
				} else {
					rootHelper.RegWidget (_uph);
				}
			}

			return;
		}

		if (_uph == null) {
			_uph = obj.AddComponent<UIPhaseHelper> ();
		}

		int _idx = rootHelper.RegWidget (_uph);
		_uph.root = this._uiObject;
		if (_uph.des == null || _uph.des == string.Empty) {
			_uph.des = DES_3;
		}

		if (_twr != null) {
			this.__PhaseTweenAni (_idx, _uph);
		}

		if (_btn != null) {
			this.__PhaseUIButton (_idx, _uph);
		}

		if (_lb != null) {
			this.__PhaseUILabel (_idx, _uph);
		}

		if (_sld != null) {
			this.__PhaseUISlider (_idx, _uph);
		}

		if (_srv != null) {
			this.__PhaseUIScrollerView (_idx, _uph);
		}
	}

	private void __PhaseUILabel(int idx, UIPhaseHelper uph) {
		if (uph.isNeedTranslate) {
			UITranslateHelper uth = uph.gameObject.GetComponent<UITranslateHelper> ();
			if (uth == null) {
				uth = uph.gameObject.AddComponent<UITranslateHelper> ();
			}
		} else {
			UITranslateHelper uth = uph.gameObject.GetComponent<UITranslateHelper> ();
			if (uth != null) {
				GameObject.DestroyImmediate (uth);
			}
		}
	}

	private const string FILE_HEAD_SCROLLER_VIEW = "scrollerViewData";
	private void __PhaseUIScrollerView(int idx, UIPhaseHelper uph) {
		UIRootHelper rootHelper = this._uiObject.GetComponent<UIRootHelper> ();
		if (rootHelper == null) {
			return;
		}

		if (rootHelper.widgets == null) {
			return;
		}

		if (uph == null) {
			return;
		}

		string path = CODE_PATH + this._uiObject.name + "/" + FILE_HEAD_SCROLLER_VIEW + "/";
		GameObject _obj = uph.gameObject;

		UIScrollView _sld = _obj.GetComponent<UIScrollView> ();
		if (_sld == null) {
			return;
		}

		string fileName = FILE_HEAD_SCROLLER_VIEW + _obj.name;
		uph.scrollerTableDataModuleName = fileName;
		UIPhaserTableDataGenerator.WriteCode (path, this._uiObject.name, fileName, uph.des);
	}

	private const string FILE_HEAD_SLIDER = "OnSlider";
	private void __PhaseUISlider(int idx, UIPhaseHelper uph) {
		UIRootHelper rootHelper = this._uiObject.GetComponent<UIRootHelper> ();
		if (rootHelper == null) {
			return;
		}

		if (rootHelper.widgets == null) {
			return;
		}

		if (uph == null) {
			return;
		}

		string path = CODE_PATH + this._uiObject.name + "/" + FILE_HEAD_SLIDER + "/";
		GameObject _obj = uph.gameObject;

		UISlider _sld = _obj.GetComponent<UISlider> ();
		if (_sld == null) {
			return;
		}

		string fileName = FILE_HEAD_SLIDER + _obj.name;
		uph.onSliderModuleName = fileName;
		UIPhaserEditGenerator.WriteCode (path, this._uiObject.name, fileName, uph.des);
	}

	private const string FILE_HEAD_ONCLICK = "OnClick";
	private void __PhaseUIButton(int idx, UIPhaseHelper uph) {
		UIRootHelper rootHelper = this._uiObject.GetComponent<UIRootHelper> ();
		if (rootHelper == null) {
			return;
		}

		if (rootHelper.widgets == null) {
			return;
		}

		if (uph == null) {
			return;
		}

		string path = CODE_PATH + this._uiObject.name + "/" + FILE_HEAD_ONCLICK + "/";
		GameObject _obj = uph.gameObject;

		UIButton _btn = _obj.GetComponent<UIButton> ();
		if (_btn == null) {
			return;
		}

		string fileName = FILE_HEAD_ONCLICK + _obj.name;
		uph.onClickModuleName = fileName;
		UIPhaserOnClickGenerator.WriteCode (path, this._uiObject.name, fileName, uph.des);
	}

	private const string FILE_HEAD_TWEENFIN = "OnTweenFinished";
	private void __PhaseTweenAni(int idx, UIPhaseHelper uph) {
		UIRootHelper rootHelper = this._uiObject.GetComponent<UIRootHelper> ();
		if (rootHelper == null) {
			return;
		}

		if (rootHelper.widgets == null) {
			return;
		}

		if (uph == null) {
			return;
		}

		string path = CODE_PATH + this._uiObject.name + "/" + FILE_HEAD_TWEENFIN + "/";
		GameObject _obj = uph.gameObject;
		UITweener _btn = _obj.GetComponent<UITweener> ();
		if (_btn == null) {
			return;
		}

		string fileName = FILE_HEAD_TWEENFIN + _obj.name;
		uph.onTweenAniFinishedModuleName = fileName;
		UIPhaserEditGenerator.WriteCode (path, this._uiObject.name, fileName, uph.des);
	}
}