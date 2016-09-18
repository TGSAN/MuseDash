using UnityEngine;
using System.Collections;
using GameLogic;
using FormulaBase;

public class LongPressEndNodeController: LongPressController {
	public override bool OnControllerMiss (int idx) {
		base.OnControllerMiss (idx);

		GameKernel.Instance.IsUnderLongPress = false;
		if (GameKernel.Instance.IsLongPressFailed) {
			StageTeachComponent.Instance.SetPlayResult (idx, GameMusic.MISS);
		} else {
			uint result = BattleEnemyManager.Instance.GetPlayResult (idx);
			StageTeachComponent.Instance.SetPlayResult (idx, result);
			TaskStageTarget.Instance.AddLongPressFinishCount (1);
			StageTeachComponent.Instance.AddPlayCountRecord (1);
		}

		GameKernel.Instance.IsLongPressFailed = false;

		AttacksController.Instance.ShowPressGirl (false);
		GirlManager.Instance.UnLockActionProtect ();
		GirlManager.Instance.AttacksWithoutExchange (GameMusic.NONE, ACTION_KEYS.RUN);

//		GameObject.Destroy(gameObject);
		return false;
	}
}
