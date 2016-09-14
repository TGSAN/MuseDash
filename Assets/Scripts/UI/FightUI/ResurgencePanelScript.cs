using UnityEngine;
using System.Collections;
using GameLogic;
using DYUnityLib;
using FormulaBase;

public class ResurgencePanelScript : MonoBehaviour {
	public const float SLOW_DOWN_WHOLE_TIME = 2f;
	private const float SLOW_DOWN_VOLUME_DOWN = 0.005f;

	private static ResurgencePanelScript instance = null;
	public static ResurgencePanelScript Instance {
		get {
			return instance;
		}
	}

	private bool isSlowDownScene;

	[SerializeField]
	GameObject bgObject;
	// Use this for initialization
	void Start () {
		instance = this;
		this.gameObject.SetActive (false);
	}

	// Update is called once per frame
	void Update() {
		if (!this.isSlowDownScene) {
			return;
		}

		var speedDown = (Time.deltaTime / SLOW_DOWN_WHOLE_TIME);
		GameGlobal.gGameMusicScene.ChangeAnimationSpeed (-speedDown);

		float _v = AudioManager.Instance.GetBgmVolume () - SLOW_DOWN_VOLUME_DOWN;
		AudioManager.Instance.SetBgmVolume (_v);
		if (_v <= 0) {
			this.Pause ();
		}
	}

	public void Show() {
		this.isSlowDownScene = false;
		this.gameObject.SetActive (true);
		this.bgObject.SetActive (true);
		TweenAlpha talpha = this.bgObject.GetComponent<TweenAlpha> ();
		talpha.enabled = true;

		Animator ani = this.gameObject.GetComponent<Animator> ();
		ani.Stop ();
		ani.Rebind ();
		ani.Play ("LostVit");

		this.Pause ();
	}

	private void Pause() {
		AudioManager.Instance.PauseBackGroundMusic ();
		FixUpdateTimer.PauseTimer ();
		Time.timeScale = 0;
	}

	public void DeadShow() {
		this.isSlowDownScene = true;
		this.bgObject.SetActive (false);
		Animator ani = this.gameObject.GetComponent<Animator> ();
		ani.Play ("Dead");
	}

	public void ReviveShow() {
		this.bgObject.SetActive (false);
		Animator ani = this.gameObject.GetComponent<Animator> ();
		ani.Play ("Reborn");
	}

	public void Dead() {
		//this.gameObject.SetActive (false);
		Time.timeScale = GameGlobal.TIME_SCALE;
		AudioManager.Instance.ResumeBackGroundMusic ();
		GirlManager.Instance.PlayGirlDeadAnimation ();
		StageBattleComponent.Instance.ClearNearByObject ();
		StageBattleComponent.Instance.Dead ();
	}

	public void Revive() {
		this.gameObject.SetActive (false);

		Time.timeScale = GameGlobal.TIME_SCALE;
		FixUpdateTimer.ResumeTimer ();
			
		AudioManager.Instance.ResumeBackGroundMusic ();
		AudioManager.Instance.SetBackGroundMusicTimeScale (GameGlobal.TIME_SCALE);
			
		StageBattleComponent.Instance.Revive ();
		//GirlManager.Instance.PlayGirlReviveAnimation ();
		GirlManager.Instance.StartPhysicDetect ();
		GirlManager.Instance.StartAutoReduceEnergy ();
			
		GameGlobal.gGameMusicScene.SetAnimationSpeed (1f);
		// StageBattleComponent.Instance.ClearNearByObject ();
	}
}
