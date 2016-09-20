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

	private Camera sceneUiCamera;
	private Dictionary<string, UIRootHelper> dymWidgets;

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
		this.InitCamera ();
		this.InitByWidget ();
	}

	void OnEnable() {
		if (this.dymWidgets == null) {
			this.dymWidgets = new Dictionary<string, UIRootHelper> ();
		}

		UIRootHelper[] urhs = Transform.FindObjectsOfType<UIRootHelper> ();
		foreach (UIRootHelper urh in urhs) {
			this.RegDymWidget (urh.name, urh);
		}
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
		if (this.dymWidgets == null) {
			return;
		}

		UIRootHelper urh = this.dymWidgets [uiName];
		if (urh == null) {
			return;
		}

		UIPhaseBase upb = urh.gameObject.GetComponent<UIPhaseBase> ();
		if (upb == null) {
			urh.gameObject.SetActive (true);
			return;
		}

		upb.Show (aniName);
	}

	public void HideUi(string uiName, string aniName = null) {
		if (this.dymWidgets == null) {
			return;
		}

		UIRootHelper urh = this.dymWidgets [uiName];
		if (urh == null) {
			return;
		}

		UIPhaseBase upb = urh.GetComponent<UIPhaseBase> ();
		if (upb == null) {
			urh.gameObject.SetActive (false);
			return;
		}

		upb.Hide (aniName);
	}

	public bool IsUiActive(string uiName) {
		if (this.dymWidgets == null) {
			return false;
		}

		UIRootHelper urh = this.dymWidgets [uiName];
		if (urh == null) {
			return false;
		}

		if (urh == null) {
			return false;
		}

		return urh.gameObject.activeSelf;
	}

	public GameObject FindDymWidget(string uiName) {
		if (this.dymWidgets == null) {
			return null;
		}

		if (!this.dymWidgets.ContainsKey (uiName)) {
			return null;
		}

		UIRootHelper urh = this.dymWidgets [uiName];
		if (urh == null) {
			return null;
		}

		return urh.gameObject;
	}

	public void RegDymWidget(string uiName, UIRootHelper urh) {
		if (this.dymWidgets == null) {
			this.dymWidgets = new Dictionary<string, UIRootHelper> ();
		}

		this.dymWidgets [uiName] = urh;
	}

	private void InitCamera() {
		if (this.transform.childCount <= 0) {
			return;
		}

		Transform camTransform = this.transform.GetChild (0);
		if (camTransform == null) {
			return;
		}

		this.sceneUiCamera = camTransform.gameObject.GetComponent<Camera> ();
		// Auto UIRoot for auto screen alignment 自动设置UIRoot作为屏幕对齐
		ScreenFit.CameraFit (this.sceneUiCamera);
	}

	private void InitByWidget() {
		if (this.widgets == null || this.widgets.Count <= 0) {
			return;
		}

		UIRootHelper[] urhs = Transform.FindObjectsOfType<UIRootHelper> ();
		// 加载配置好的界面perfab，并按照配置指示是否加载即显示
		for (int i = 0; i < this.widgets.Count; i++) {
			UnityEngine.Object origObj = this.widgets [i];
			if (origObj == null) {
				Debug.Log ("UISceneHelper of " + this.gameObject.name + " lost a widget at " + i);
				continue;
			}

			GameObject instObj = null;
			foreach (UIRootHelper urh in urhs) {
				if (urh.gameObject.name == origObj.name) {
					instObj = urh.gameObject;
					break;
				}
			}

			if (instObj == null) {
				instObj = GameObject.Instantiate (origObj) as GameObject;
			}

			if (instObj == null) {
				Debug.Log ("Instance of ui perfab " + origObj.name + " has some problem.");
				continue;
			}

			instObj.name = origObj.name;
			//instObj.transform.parent = this.gameObject.transform;

			UIRootHelper _urh = instObj.GetComponent<UIRootHelper> ();
			if (_urh == null) {
				Debug.Log ("Instance of ui perfab " + instObj.name + " has no UIRootHelper.");
				continue;
			}

			if (!_urh.isShowOnLoaded) {
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
