using UnityEngine;
using System.Collections;
using GameLogic;
using FormulaBase;

public class LongPressEndNodeController: LongPressController {
	public override void OnControllerAttacked (int result, bool isDeaded)
	{
		base.OnControllerAttacked (result, isDeaded);
		AttacksController.Instance.ShowPressGirl (false);
		GirlManager.Instance.AttacksWithoutExchange (GameMusic.NONE, ACTION_KEYS.RUN);
	}

	public override bool OnControllerMiss (int idx) {
		base.OnControllerMiss (idx);

		GameKernel.Instance.IsUnderLongPress = false;
		if (GameKernel.Instance.IsLongPressFailed) {
			StageTeachComponent.Instance.SetPlayResult (idx, GameMusic.MISS);
		} else {
			TaskStageTarget.Instance.AddLongPressFinishCount (1);

			uint result = BattleEnemyManager.Instance.GetPlayResult (idx);
			StageTeachComponent.Instance.SetPlayResult (idx, result);
		}

		GameKernel.Instance.IsLongPressFailed = false;

		AttacksController.Instance.ShowPressGirl (false);
		GirlManager.Instance.AttacksWithoutExchange (GameMusic.NONE, ACTION_KEYS.RUN);

//		GameObject.Destroy(gameObject);
		return false;
	}
}
