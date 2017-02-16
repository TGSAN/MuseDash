using UnityEngine;
using System.Collections;
using DYUnityLib;
using GameLogic;
using FormulaBase;
using System.Collections.Generic;

public class TeachPointController: BaseEnemyObjectController {

	private AudioSource audioSource = null;
	/*
	void Update() {
		if (this.audioSource == null) {
			return;
		}

		if (!this.audioSource.isPlaying) {
			this.OnControllerMiss (this.idx);
		}
	}
	*/

	public override void OnControllerStart () {
		this.audioSource = this.gameObject.GetComponent<AudioSource> ();
		if (this.audioSource != null) {
			this.audioSource.time = 0;
			this.audioSource.Play ();
		}

		TeachPointConfigController tpcc = this.gameObject.GetComponent<TeachPointConfigController> ();
		if (tpcc == null || tpcc.JumpAt < 0) {
			Debug.Log (this.gameObject.name + " with TeachPointConfig error.");
			return;
		}

		this.StartCoroutine (this.OnJump (tpcc.JumpAt));
	}

	public override bool OnControllerMiss (int idx) {
		// Debug.Log ("Teach Point Controller check point : " + this.gameObject.name);
		if (this.audioSource != null) {
			this.audioSource.Stop ();
			this.audioSource = null;
		}

		float jumpback = this.GetJumpTime ();
		if (jumpback < 0) {
			return false;
		}

		AudioManager.Instance.PauseBackGroundMusic ();
		GameGlobal.gGameMusic.MusicJump ((decimal)jumpback);
		AudioManager.Instance.PlayBackGroundMusicAtTime (jumpback);
		SceneObjectController.Instance.ActiveObject (0);
		// Time.timeScale = GameGlobal.TIME_SCALE;

		return false;
	}

	private IEnumerator OnJump(float tick) {
		yield return new WaitForSeconds (tick);

		this.OnControllerMiss (this.idx);
	}

	private float GetJumpTime() {
		List<GameObject> sceneEventObjects = SceneObjectController.Instance.GetAllSceneEventObjects ();
		int idx = sceneEventObjects.IndexOf (this.gameObject);
		if (idx < 0) {
			// Debug.Log ("Teach point object un-normally deleted");
			return -1f;
		}

		int configCount = 0;
		float jumpback = -1f;
		float jumpforward = -1f;
		TeachPointConfigController tpcc = this.gameObject.GetComponent<TeachPointConfigController> ();
		if (tpcc != null) {
			configCount = tpcc.SucceedCount;
			jumpback = tpcc.JumpBack;
			jumpforward = tpcc.JumpForward;
		}

		// If succeed play, jump to next jumpable teach point,
		// else, jump back to nearest jumpable teach point.
		if (StageTeachComponent.Instance.TeachPlayResultCheck (configCount)) {
			Debug.Log ("Teach check point " + idx + " " + this.gameObject.name + " play succeed.");
			if (tpcc.NextTalk) {
				TeachActionTalkConfigController.Instance.NextTalk ();
			}

			if (jumpforward > 0) {
				return jumpforward;
			}
			/*
			for (int i = idx + 1; i < sceneEventObjects.Count; i++) {
				GameObject sobj = sceneEventObjects [i];
				if (sobj == null) {
					continue;
				}

				TeachPointConfigController _tpcc = sobj.GetComponent<TeachPointConfigController> ();
				if (_tpcc == null) {
					continue;
				}

				if (_tpcc.JumpBack < 0) {
					continue;
				}

				if (i == idx + 1) {
					return -1;
				}

				return _tpcc.JumpBack;
			}
			*/
		} else {
			Debug.Log ("Teach check point " + idx + " " + this.gameObject.name + " play failed.");
			if (jumpback > 0) {
				return jumpback;
			}

			for (int i = idx; i >= 0; i--) {
				GameObject sobj = sceneEventObjects [i];
				if (sobj == null) {
					continue;
				}
				
				TeachPointConfigController _tpcc = sobj.GetComponent<TeachPointConfigController> ();
				if (sobj == null) {
					continue;
				}
				
				if (_tpcc.JumpBack < 0) {
					continue;
				}
				
				return _tpcc.JumpBack;
			}
		}

		return -1f;
	}
}