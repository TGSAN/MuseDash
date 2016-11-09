using UnityEngine;
using System.Collections;
using DYUnityLib;
using GameLogic;
using FormulaBase;

public class AirMusicNodeController : BaseEnemyObjectController {
	private bool isBeAttacked = false;

	public override void OnControllerStart(){
		//		host.SetDynamicData(FormulaBase.SignKeys.COMBO, (float)true);
		base.OnControllerStart();
	}

	private void AddExtraScore(int result){
		var myHost = ItemComponent.Instance.GetItemHost ();
		MusicData md = StageBattleComponent.Instance.GetMusicDataByIdx (this.idx);
		int _iidx = NodeConfigReader.GetNodeIdxByNodeid(md.nodeData.uid);
		myHost.SetDynamicData (SignKeys.ID, _iidx);
		int _score = (int)myHost.Result (FormulaKeys.FORMULA_22);
		TaskStageTarget.Instance.AddScore (_score);
		BattleEnemyManager.Instance.SetPlayResult (this.idx, GameMusic.PERFECT);
		BattleRoleAttributeComponent.Instance.FireSkill (SkillComponent.ON_EAT_ITEM);

		CharPanel.Instance.SetHpScore (_score);
		SpineActionController.Play (ACTION_KEYS.COMEOUT, this.gameObject);

		string audioName = BattleEnemyManager.Instance.GetNodeAudioByIdx (this.idx);
		AudioManager.Instance.PlayGirlHitByName (audioName);
	}

	private IEnumerator EatMusicNodeByTime(float time,int result){
		yield return new WaitForSeconds(time);
		// Debug.Log(this.idx + "================--------------- time to eat gold!");
		AddExtraScore(result);
	}
	
	public override void OnControllerAttacked (int result, bool isDeaded) {
//		ArrayList musicData = StageBattleComponent.Instance.GetMusicData ();
//		MusicData md = (MusicData)musicData [this.idx];
//		
//		if (md.nodeData.isAirunits) {
//			if (!GameGlobal.gGameTouchPlay.IsJump ()) {
//				return;
//			}
//			var time = GameGlobal.JUMP_WHOLE_TIME * md.GetAttackrangeRate();
//			Debug.Log(md.objId + " ++++++++++++++++-------------- " + time + " delay to eat!");
//			StartCoroutine(EatMusicNodeByTime(time,result));
//		}

		if (!isBeAttacked) {
			AddExtraScore (result);
			Animator ani = this.gameObject.GetComponent<Animator> ();
			if (ani != null) {
				ani.speed = 0;
			}

			isBeAttacked = true;
			TaskStageTarget.Instance.AddNoteItemCount (1);
		}
	}
}
