using UnityEngine;
using System.Collections;
using DYUnityLib;
using GameLogic;
using FormulaBase;

public class ResourceItemController: BaseEnemyObjectController {
	private bool isBeAttacked = false;

	public const int HIT_BY_HEAD = 0;
	public const int HIT_BY_HAND = 1;
	public const int HIT_BY_FOOT = 2;

	private int hitPosition = -1;

	public void SetHitPosition(int flag = -1){
		hitPosition = flag;
	}

	public override void OnControllerStart ()
	{
		base.OnControllerStart ();
	}

	public override bool OnControllerMiss (int idx) {
		return false;
	}

	public override void OnControllerAttacked (int result, bool isDeaded) {
		if (!isBeAttacked && result != (int)GameMusic.JUMPOVER) {
			Vector3 pos = this.gameObject.transform.position;
			//Debug.Log(this.idx + " ========================== has eat!" + pos.x);
			Vector3 recordPos = new Vector3 (pos.x, pos.y, pos.z);
			SpineActionController.Play (ACTION_KEYS.COMEOUT, this.gameObject);

			this.gameObject.transform.position = recordPos;

			isBeAttacked = true;
			TaskStageTarget.Instance.AddGoldItemCount (1);
			BattleEnemyManager.Instance.SetPlayResult (this.idx, GameMusic.PERFECT);
			BattleRoleAttributeComponent.Instance.FireSkill (SkillComponent.ON_EAT_ITEM);

			if (CharPanel.Instance != null) {
				CharPanel.Instance.SetGoldChange (100); // test value
			}

			string audioName = BattleEnemyManager.Instance.GetNodeAudioByIdx (this.idx);
			AudioManager.Instance.PlayGirlHitByName (audioName);
		}
	}
}