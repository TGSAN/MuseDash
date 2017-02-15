using System;
using GameLogic;
using System.Collections;
using UnityEngine;
using FormulaBase;

public class GroundMusicNodeController : BaseEnemyObjectController {

	private bool isBeAttacked = false;

	public override void OnControllerStart() {
		base.OnControllerStart ();
	}

	private void AddExtraScore() {
//		var myHost = ItemComponent.Instance.GetItemHost ();

		ArrayList musicData = StageBattleComponent.Instance.GetMusicData ();
		if (this.idx >= 0 && this.idx < musicData.Count) {
			MusicData md = StageBattleComponent.Instance.GetMusicDataByIdx (this.idx);
			int _iidx = NodeConfigReader.GetNodeIdxByNodeid(md.nodeData.uid);
//			myHost.SetDynamicData (SignKeys.ID, _iidx);
		} else {
			// If not from stage music config, maybe from mimic.
			if (MimicParentController.Instance != null) {
				FormulaHost host = MimicParentController.Instance.GetHostByGameObject (this.gameObject);
				if (host != null) {
//					myHost.SetDynamicData (SignKeys.ID, (int)host.GetDynamicDataByKey (SignKeys.ID));
				}
			}
		}

//		int _score = (int)myHost.Result (FormulaKeys.FORMULA_22);
//		TaskStageTarget.Instance.AddScore (_score);
		BattleEnemyManager.Instance.SetPlayResult (this.idx, GameMusic.PERFECT);
		BattleRoleAttributeComponent.Instance.FireSkill (SkillComponent.ON_EAT_ITEM);

		SpineActionController.Play (ACTION_KEYS.COMEOUT, this.gameObject);
//		CharPanel.Instance.SetHpScore (_score);
		string audioName = BattleEnemyManager.Instance.GetNodeAudioByIdx (this.idx);
		AudioManager.Instance.PlayGirlHitByName (audioName);
	}

	/*
	private IEnumerator WaitForEatMusicNode(float time) {
		yield return new WaitForSeconds (time);
		AddExtraScore ();
	}*/

	public override void OnControllerAttacked (int result, bool isDeaded) {
		if (!isBeAttacked && result != (int)GameMusic.JUMPOVER) {
			AddExtraScore ();
			isBeAttacked = true;
			Animator ani = this.gameObject.GetComponent<Animator> ();
			if (ani != null) {
				ani.speed = 0;
			}

			TaskStageTarget.Instance.AddNoteItemCount (1);
		}

//		base.OnControllerAttacked (result, isDeaded);
//
//		if (GameGlobal.gGameTouchPlay.IsJump ()) {
//			return;
//		}
//		string audioPath = "Audio/GirlEffects/ball_kill";
//		AudioManager.Instance.PlayNodeEffectByNodeAudioSourceWithPath (audioPath,"ball_kill");
	}

	public override bool OnControllerMiss (int idx) {
//		ArrayList musicData = StageBattleComponent.Instance.GetMusicData ();
//		MusicData md = (MusicData)musicData [this.idx];
//		// if is air unit, only jump can pass;
//		// if is not air unit, only jump not pass.
//		if (md.nodeData.isAirunits) {
//			if (!GameGlobal.gGameTouchPlay.IsJump ()) {
//				return false;
//			}
//			var time = GameGlobal.JUMP_WHOLE_TIME * md.GetAttackrangeRate();
//			StartCoroutine(WaitForEatMusicNode(time));
//		} else {
//			if (GameGlobal.gGameTouchPlay.IsJump ()) {
//				return false;
//			}
//			Debug.Log("=========--------- OnController Miss in ground music node!");
//			AddExtraScore();
//		}
//		GameGlobal.gGameMusicScene.ShowLongPressMask (false);
		return true;
	}
}