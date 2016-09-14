using System;
using System.Reflection;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FormulaBase;

/// <summary>
/// User interface scene helper.
/// 挂载在场景中所有界面的根节点上
/// 该根节点应该包含UICamera
/// 同时包含各个带UIRootHelper的界面perfab
/// </summary>
public class UISceneHelper : MonoBehaviour {
	private static UISceneHelper instance = null;
	public static UISceneHelper Instance {
		get {
			return instance;
		}
	}
	
	[SerializeField]
	/// <summary>
	/// The widgets.
	/// 各个带UIRootHelper的界面perfab
	/// </summary>
	public List<UnityEngine.Object> widgets;
	//public List<string> widgetPaths;

	void Start() {
		//this.InitByPath ();
		instance = this;
		this.InitByWidget ();
	}

	void OnDestory() {
	}

	/*
	private void InitByPath() {
		if (this.widgetPaths == null || this.widgetPaths.Count <= 0) {
			return;
		}

		// 加载配置好的界面perfab，并按照配置指示是否加载即显示
		for (int i = 0; i < this.widgetPaths.Count; i++) {
			string path = this.widgetPaths [i];
			if (path == null) {
				Debug.Log ("UISceneHelper of " + this.gameObject.name + " lost a widget at " + i);
				continue;
			}


		}
	}
	*/

	/// <summary>
	/// Shows the user interface.
	/// 
	/// 根据prefab名显示某个ui
	/// </summary>
	/// <param name="uiName">User interface name.</param>
	public void ShowUi(string uiName, string aniName = null) {
		Transform t = this.transform.Find (uiName);
		if (t == null || t.gameObject == null) {
			return;
		}

		UIPhaseBase upb = t.gameObject.GetComponent<UIPhaseBase> ();
		if (upb == null) {
			t.gameObject.SetActive (true);
			return;
		}

		upb.Show (aniName);
	}

	public void HideUi(string uiName, string aniName = null) {
		Transform t = this.transform.Find (uiName);
		if (t == null || t.gameObject == null) {
			return;
		}

		UIPhaseBase upb = t.gameObject.GetComponent<UIPhaseBase> ();
		if (upb == null) {
			t.gameObject.SetActive (false);
			return;
		}

		upb.Hide (aniName);
	}

	public bool IsUiActive(string uiName) {
		Transform t = this.transform.Find (uiName);
		if (t == null || t.gameObject == null) {
			return false;
		}

		return t.gameObject.activeSelf;
	}

	private void InitByWidget() {
		if (this.widgets == null || this.widgets.Count <= 0) {
			return;
		}

		// 加载配置好的界面perfab，并按照配置指示是否加载即显示
		for (int i = 0; i < this.widgets.Count; i++) {
			UnityEngine.Object origObj = this.widgets [i];
			if (origObj == null) {
				Debug.Log ("UISceneHelper of " + this.gameObject.name + " lost a widget at " + i);
				continue;
			}

			GameObject instObj = null;
			if (this.gameObject.transform.Find (origObj.name) != null) {
				instObj = this.gameObject.transform.Find (origObj.name).gameObject;
			} else {
				instObj = GameObject.Instantiate (origObj) as GameObject;
			}

			if (instObj == null) {
				Debug.Log ("Instance of ui perfab " + origObj.name + " has some problem.");
				continue;
			}

			instObj.name = origObj.name;
			instObj.transform.parent = this.gameObject.transform;

			UIRootHelper urh = instObj.GetComponent<UIRootHelper> ();
			if (urh == null) {
				Debug.Log ("Instance of ui perfab " + instObj.name + " has no UIRootHelper.");
				continue;
			}

			if (!urh.isShowOnLoaded) {
				instObj.SetActive (false);
				continue;
			}

			UIPhaseBase upb = instObj.GetComponent<UIPhaseBase> ();
			if (upb == null) {
				Debug.Log ("Instance of ui perfab " + instObj.name + " has no UIPhaseBase.");
				instObj.SetActive (true);
				continue;
			}

			upb.Show ();
		}
	}
}
