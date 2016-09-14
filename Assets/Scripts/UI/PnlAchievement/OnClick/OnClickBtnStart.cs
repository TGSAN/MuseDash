/// UI分析工具自动生成代码
/// 未有描述
/// 
using System;
using UnityEngine;
using FormulaBase;
using GameLogic;

namespace PnlAchievement {
	public class OnClickBtnStart : UIPhaseOnClickBase {
		public static void Do(GameObject gameObject) {
			OnDo (gameObject);

			int sid = StageBattleComponent.Instance.GetId ();
			uint diff = StageBattleComponent.Instance.GetDiffcult ();
			StageBattleComponent.Instance.Enter ((uint)sid, diff);
		}
	}
}