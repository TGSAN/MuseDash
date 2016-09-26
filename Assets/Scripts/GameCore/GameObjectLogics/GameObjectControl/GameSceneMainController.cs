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

	public float waitForStartMusic = 5f;

	void Start () {
		this._ScreenFit ();

		this.secondCounter = 0f;
		Application.targetFrameRate = 60;
		GameGlobal.gCamera = this;

		#if UNITY_EDITOR || UNITY_EDITOR_OSX || UNITY_EDITOR_64
		if (StageBattleComponent.Instance.Host == null) {
			StageBattleComponent.Instance.InitById((int)GameGlobal.DEBUG_DEFAULT_STAGE);
		}
		#endif

		string musicName = StageBattleComponent.Instance.GetMusicName ();
		ResourceLoader.Instance.Load (musicName, this.__OnStartAfterMusicLoaded, ResourceLoader.RES_FROM_LOCAL);
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

	private void __OnStartAfterMusicLoaded(UnityEngine.Object obj) {
		if (obj == null) {
			return;
		}

		AudioManager.Instance.SetCatchClip (obj as AudioClip);
		this.__OnStat ();
	}

	private IEnumerator __StageStart() {
		yield return new WaitForSeconds (this.waitForStartMusic);

		StageBattleComponent.Instance.GameStart (null, 0, null);
	}

	private void __OnStat() {
		StageBattleComponent.Instance.Init ();

		// for fps memory show
		if (SettingComponent.Instance.Host == null) {
			SettingComponent.Instance.Init ();
			SettingComponent.Instance.Host.SetAsUINotifyInstance ();
		}

		// 所有数据 对象准备完毕后才展示ui
		UISceneHelper.Instance.Show ();
		this.StartCoroutine (this.__StageStart ());
		FightMenuPanel.Instance.OnStageReady ();
		if (CommonPanel.GetInstance () != null) {
			CommonPanel.GetInstance ().SetMask (false);
		}
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
		Camera cam = this.gameObject.GetComponent<Camera> ();
		ScreenFit.CameraFit (cam);
	}
}
