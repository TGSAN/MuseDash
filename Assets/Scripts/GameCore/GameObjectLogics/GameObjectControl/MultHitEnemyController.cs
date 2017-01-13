using UnityEngine;
using System.Collections;
using DYUnityLib;
using GameLogic;
using FormulaBase;

public class MultHitEnemyController: BaseEnemyObjectController {
	private decimal pauseTick = -1;
	// Use this for initialization
	void Start () {
	}

	void FixedUpdate() {
		if (FixUpdateTimer.IsPausing ()) {
			return;
		}

		if (this.pauseTick < 0) {
			return;
		}

		if (this.pauseTick == 0) {
			MusicData md = StageBattleComponent.Instance.GetMusicDataByIdx (idx);
			md.SetShotPause (0m);

			uint result = BattleEnemyManager.Instance.GetPlayResult (idx);
			//if (!prd.isPressedOk) {
			if (result <= GameMusic.MISS) {
				GameGlobal.gGameMissPlay.MissCube (this.idx, GameGlobal.MISS_NO_CHECK_TICK);
			}

			if (this.IsEmptyNode ()) {
				this.gameObject.SetActive (false);
				GameObject.Destroy (this.gameObject);
			}
		}

		this.pauseTick -= FixUpdateTimer.dInterval;
	}

	public override void OnControllerAttacked (int result, bool isDeaded) {
	}

	public override bool IsShotPause() {
		return this.pauseTick > 0;
	}

	public override void SetShotPause (decimal tick) {
		if (tick < 0) {
			return;
		}

		if (this.pauseTick > -1m) {
			return;
		}

		this.pauseTick = tick;
	}
}