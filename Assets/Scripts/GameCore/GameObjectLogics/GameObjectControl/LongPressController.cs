using UnityEngine;
using System.Collections;
using GameLogic;
using FormulaBase;

public class LongPressController: BaseEnemyObjectController {
	public static int psidx = -1;
	public override void Init () {
		base.Init ();
		if (psidx < 0) {
			psidx = this.idx;
		}
	}

	public override void OnControllerStart () {
		base.OnControllerStart ();

		//ArrayList mds = StageBattleComponent.Instance.GetMusicData ();
		//if (mds == null) {
		//	return;
		//}

		//SpineActionController.SetSkeletonOrder (mds.Count - this.idx, this.gameObject);
	}

	public override bool OnControllerMiss (int idx) {
		GameKernel.Instance.IsUnderLongPress = true;
#if !UNITY_EDITOR && !UNITY_EDITOR_OSX && !UNITY_EDITOR_64
		if (GameGlobal.gGameTouchPlay.IsPunch (Input.touchCount)) {
#else
		if (GameGlobal.gGameTouchPlay.IsPunch ()) {
#endif
			BattleEnemyManager.Instance.AddHp (this.idx, -1);
			GameGlobal.gGameTouchPlay.TouchResult (idx, GameMusic.PERFECT, GameMusic.TOUCH_ACTION_SIGNLE_PRESS);	//(mark) attack node damage to it
			BattleEnemyManager.Instance.SetPlayResult (idx, GameMusic.PERFECT);
			GameGlobal.gGameMissPlay.SetMissHardTime (0);
			AttacksController.Instance.ShowPressGirl (true);
		} else {
			if (!GameKernel.Instance.IsOnFeverState ()) {
				base.OnControllerMiss (idx);
				GameGlobal.gGameMissPlay.SetMissHardTime (0);
				GameKernel.Instance.IsLongPressFailed = true;
				GirlManager.Instance.BeAttackEffect ();
				AttacksController.Instance.BeAttacked ();
			} else {
				foreach (var girl in GirlManager.Instance.Girls) {
					if (girl != null) {
						if (SpineActionController.CurrentAnimationName (girl) != "run") {
							SpineActionController.Play (ACTION_KEYS.RUN, girl);
						}
					}
				}
			}
		}
		
		return true;
	}
}