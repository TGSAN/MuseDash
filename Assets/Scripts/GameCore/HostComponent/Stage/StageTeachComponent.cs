///自定义模块，可定制模块具体行为
using System;
using UnityEngine;
using GameLogic;

namespace FormulaBase {
	public class StageTeachComponent : CustomComponentBase {
		private static StageTeachComponent instance = null;
		private const int HOST_IDX = 3;
		public static StageTeachComponent Instance {
			get {
				if(instance == null) {
					instance = new StageTeachComponent();
				}
			return instance;
			}
		}

		// ----------------------- // -----------------------
		private int playCountRecord;
		private bool isTeachingStage = false;
		private GameObject teachBeatObj = null;
		private GameObject teachJumpObj = null;
		private bool beatLock = false;
		private bool jumpLock = false;

		public void Init() {
			this.playCountRecord = 0;
			this.isTeachingStage = false;
			StageTeachComponent.Instance.SetBeatLock (false);
			StageTeachComponent.Instance.SetJumpLock (false);
		}

		public bool IsTeachingStage() {
			return this.isTeachingStage;
		}

		public void SetIsTeachingStage(bool value) {
			this.isTeachingStage = value;
			Debug.Log ("Set teach stage " + value);
		}

		public void RegBeatObj(GameObject obj) {
			this.teachBeatObj = obj;
		}

		public void RegJumpObj(GameObject obj) {
			this.teachJumpObj = obj;
		}

		public bool IsBeatLock() {
			return this.beatLock;
		}

		public bool IsJumpLock() {
			return this.jumpLock;
		}

		public void SetBeatLock(bool value) {
			this.beatLock = value;
		}

		public void SetJumpLock(bool value) {
			this.jumpLock = value;
		}

		public void UnLockBeat() {
			this.SetBeatLock (false);
			if (this.teachBeatObj != null) {
				SpineActionController.Play (ACTION_KEYS.SUMMON2_END, this.teachBeatObj);
			}
		}

		public void UnLockJump() {
			this.SetJumpLock (false);
			if (this.teachJumpObj != null) {
				SpineActionController.Play (ACTION_KEYS.SUMMON2_END, this.teachJumpObj);
			}
		}

		// ---------------------------------------------------------------------------
		// New teach about.
		// ---------------------------------------------------------------------------
		public bool TeachPlayResultCheck(int configCount) {
			Debug.Log ("TeachPlayResultCheck : " + this.playCountRecord + " / " + configCount);
			bool result = (this.playCountRecord >= configCount);
			if (result) {
				this.playCountRecord = 0;
			}

			return result;
		}

		public void AddPlayCountRecord(int val) {
			this.playCountRecord += val;
			Debug.Log ("Teach play record " + this.playCountRecord);
		}

		public void SetPlayResult(int idx, uint result) {
			if (!this.IsTeachingStage ()) {
				return;
			}

			if (result == GameMusic.NONE) {
				return;
			}

			MusicData md = StageBattleComponent.Instance.GetMusicDataByIdx (idx);
			if (md.nodeData.type == GameGlobal.NODE_TYPE_PRESS) {
				return;
			}

			if (result == GameMusic.MISS) {
				// Show teach miss info.
				if (TeachActionTalkConfigController.Instance != null) { 
					TeachActionTalkConfigController.Instance.OnMissed ();
				}

				return;
			}

			this.AddPlayCountRecord (1);
			// Show teach play succeed info.
			if (TeachActionTalkConfigController.Instance != null) { 
				TeachActionTalkConfigController.Instance.OnAttacked ();
			}
		}
	}
}