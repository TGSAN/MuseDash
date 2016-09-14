using UnityEngine;
using System.Collections;
using DYUnityLib;
using FormulaBase;

namespace GameLogic {
	public class GameMissPlay {
		// current load to index of musicTickData
		private decimal missHardTime = -1m;

		public void Init() {
			this.missHardTime = -1m;
		}

		public void SetMissHardTime(decimal t) {
			this.missHardTime = t;
			if (t == 0) {
				GirlManager.Instance.StopBeAttckedEffect ();
			}
		}

		public decimal GetMissHardTime() {
			return this.missHardTime;
		}

		public void MissCube(int idx, decimal currentTick) {
			if (BattleRoleAttributeComponent.Instance.IsDead ()) {
				return;
			}

			GameObject obj = BattleEnemyManager.Instance.GetObj (idx);
			if (obj == null) {
				//if (GameGlobal.IS_DEBUG) {
					Debug.Log ("MissCube Already null with " + idx);
				//}
				return;
			}

			BaseSpineObjectController bsc = obj.GetComponent<BaseSpineObjectController> ();
			if (bsc == null) {
				Debug.Log ("Spine Object Controller is null with " + idx);
				return;
			}

			if (!bsc.ControllerMissCheck (idx, currentTick)) {
				return;
			}

			if (ArmActionController.Instance != null) {
				ArmActionController.Instance.OnControllerMiss (idx);
			}

			if (StageBattleComponent.Instance.IsAutoPlay ()) {
				MusicData md = StageBattleComponent.Instance.GetMusicDataByIdx (idx);
				uint pd = GameGlobal.PRESS_STATE_PUMCH;
				if (md.nodeData.jump_note) {
					pd = GameGlobal.PRESS_STATE_JUMP;
				}
				GameGlobal.gGameTouchPlay.TouchActionResult (GameMusic.TOUCH_ACTION_SIGNLE_PRESS, pd);
			} else {
				bsc.OnControllerMiss (idx);
			}

			if (GameGlobal.IS_DEBUG) {
				Debug.Log ("Miss at " + idx);
			}
		}
	}
}