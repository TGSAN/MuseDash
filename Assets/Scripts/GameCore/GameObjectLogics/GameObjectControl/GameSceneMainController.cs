using UnityEngine;
using System.Collections;
using DYUnityLib;
using GameLogic;
using FormulaBase;

/// <summary>
/// Game scene main.
/// 
/// 战斗场景主入口
/// </summary>
public class GameSceneMainController : MonoBehaviour {
	private float secondCounter = 0f;
	private GameObject gameCamera;

	void Start () {
		//this.InitCommonObject ();
		this._ScreenFit ();

		this.secondCounter = 0f;
		Application.targetFrameRate = 60;
		GameGlobal.gCamera = this;

		#if UNITY_EDITOR || UNITY_EDITOR_OSX || UNITY_EDITOR_64
		if (StageBattleComponent.Instance.Host == null) {
			StageBattleComponent.Instance.InitById((int)GameGlobal.DEBUG_DEFAULT_STAGE);
		}
		#endif

		this.StartCoroutine (this.__OnStart ());
		//this.__OnStart ();
	}

	//void OnEnable() {
	//}

	void OnDestory() {
	}

	void FixedUpdate() {
		if (FixUpdateTimer.IsPausing ()) {
			return;
		}

		if (GameGlobal.gTouch == null || GameGlobal.gGameMusic == null) {
			return;
		}

		GameGlobal.gTouch.TouchEvntPhase ();
		GameGlobal.gGameMusic.GameMusicFixTimerUpdate ();

		this.FpsMemoryShowUpdate ();
	}

	private IEnumerator __OnStart() {
		yield return new WaitForSeconds (0.01f);

		StageBattleComponent.Instance.Init ();

		// for fps memory show
		if (SettingComponent.Instance.Host == null) {
			SettingComponent.Instance.Init ();
			SettingComponent.Instance.Host.SetAsUINotifyInstance ();
		}

		// 所有数据 对象准备完毕后才展示ui
		UISceneHelper.Instance.Show ();
	}

	private void InitCommonObject() {
		if (GameObject.Find ("CommonObjcet") != null) {
			return;
		}

		GameObject.Instantiate (Resources.Load (GameGlobal.PREFABS_PATH + "ui/NGUI/CommonObjcet") as GameObject);
	}

	private void FpsMemoryShowUpdate() {
		if (this.secondCounter < 100) {
			this.secondCounter += 1f;
			return;
		}

		// Fps memory show init.
		if (SettingComponent.Instance.Host == null) {
			this.secondCounter = 0;
			return;
		}

		this.secondCounter = 0;
		string fpsShow = SettingComponent.Instance.GetShowStr ();
		SettingComponent.Instance.Host.SetDynamicData (SignKeys.SMALLlTYPE, fpsShow);
	}

	private void _ScreenFit() {
		this.gameCamera = GameObject.Find ("GameCamera").gameObject;
		if (this.gameCamera == null) {
			return;
		}

		Camera cam = this.gameCamera.GetComponent<Camera> ();
		ScreenFit.CameraFit (cam);
	}
}
