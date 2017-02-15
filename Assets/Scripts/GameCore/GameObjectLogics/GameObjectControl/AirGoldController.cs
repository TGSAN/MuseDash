using UnityEngine;
using System.Collections;
using DYUnityLib;
using GameLogic;
using FormulaBase;

public class AirGoldController : BaseEnemyObjectController {
	private bool isBeAttacked = false;

	public override bool OnControllerMiss (int idx){
		return false;
	}

	public override void OnControllerAttacked (int result, bool isDeaded) {
		if (!isBeAttacked) {
			isBeAttacked = true;
			TaskStageTarget.Instance.AddGoldItemCount (1);
			BattleEnemyManager.Instance.SetPlayResult (this.idx, GameMusic.PERFECT);
			BattleRoleAttributeComponent.Instance.FireSkill (SkillComponent.ON_EAT_ITEM);

			SpineActionController.Play (ACTION_KEYS.COMEOUT, this.gameObject);
			Animator ani = this.gameObject.GetComponent<Animator> ();
			if (ani != null) {
				ani.speed = 0;
			}

			if (CharPanel.Instance != null) {
				CharPanel.Instance.SetGoldChange (100); // test value
			}

			string audioName = BattleEnemyManager.Instance.GetNodeAudioByIdx (this.idx);
			AudioManager.Instance.PlayGirlHitByName (audioName);
		}
	}
}
