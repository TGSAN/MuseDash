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

	public Dictionary<string, UIRootHelper> widgets {
		get {
			return this.dymWidgets;
		}
	}

	public bool isStartScene;

	void Start() {
		instance = this;
		this.InitCamera ();
		if (this.isStartScene) {
			Debug.Log ("This is the start scene.");
			return;
		}

		this.HideWidget ();
	}

	void OnDestory() {
	}

	public void Show() {
		if (this.dymWidgets == null || this.dymWidgets.Count <= 0) {
			Debug.Log (this.gameObject.name + " not reg ui in dymWidgets.");
			return;
		}

		foreach (UIRootHelper urh in this.dymWidgets.Values) {
			if (urh == null) {
				continue;
			}

			UIPhaseBase upb = urh.gameObject.GetComponent<UIPhaseBase> ();
			if (upb == null) {
				continue;
			}

			if (!urh.isShowOnLoaded) {
				urh.gameObject.SetActive (false);
				continue;
			}

			upb.Show ();
		}
	}

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

		if (!this.dymWidgets.ContainsKey (uiName)) {
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

		if (!this.dymWidgets.ContainsKey (uiName)) {
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

	public void MarkShowOnLoad(string uiName, bool value) {
		if (this.dymWidgets == null) {
			return;
		}

		if (!this.dymWidgets.ContainsKey (uiName)) {
			return;
		}

		UIRootHelper urh = this.dymWidgets [uiName];
		if (urh == null) {
			return;
		}

		urh.isShowOnLoaded = value;
	}

	public bool IsUiActive(string uiName) {
		if (this.dymWidgets == null) {
			return false;
		}

		if (!this.dymWidgets.ContainsKey (uiName)) {
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

	public void HideWidget() {
		this.dymWidgets = new Dictionary<string, UIRootHelper> ();
		UIRootHelper[] urhs = Transform.FindObjectsOfType<UIRootHelper> ();
		// First reg
		foreach (UIRootHelper urh in urhs) {
			if (urh == null) {
				continue;
			}

			this.RegDymWidget (urh.gameObject.name, urh);
		}

		// Then catch and hide;
		foreach (UIRootHelper urh in urhs) {
			if (urh == null) {
				continue;
			}

			Debug.Log ("Catch ui : " + urh.gameObject.name);
			UIPhaseBase upb = urh.gameObject.GetComponent<UIPhaseBase> ();
			if (upb != null) {
				upb.BeCatched ();
			}

			urh.gameObject.SetActive (false);
		}
	}
}
