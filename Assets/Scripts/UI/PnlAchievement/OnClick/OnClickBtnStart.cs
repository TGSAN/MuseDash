/// UI分析工具自动生成代码
/// 未有描述
/// 
using System;
using UnityEngine;
using FormulaBase;
using GameLogic;
using Assets.Scripts.NGUI;

namespace PnlAchievement {
	public class OnClickBtnStart : UIPhaseOnClickBase {
		public static void Do(GameObject gameObject) {
			OnDo (gameObject);

			if (UISceneHelper.Instance != null) {
				UISceneHelper.Instance.HideWidget ();
			}

			int sid = StageBattleComponent.Instance.GetId ();
			uint diff = StageBattleComponent.Instance.GetDiffcult ();
			SceneAudioManager.Instance.bgm.clip = PnlScrollCircle.instance.CatchClip;
			StageBattleComponent.Instance.Enter ((uint)sid, diff);
		}
	}
}