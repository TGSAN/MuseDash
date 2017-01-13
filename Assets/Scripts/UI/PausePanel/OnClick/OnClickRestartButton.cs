/// UI分析工具自动生成代码
/// 未有描述
/// 
using System;
using UnityEngine;
using FormulaBase;


namespace PausePanel {
	public class OnClickRestartButton : UIPhaseOnClickBase {
		public static void Do(GameObject gameObject) {
			StageBattleComponent.Instance.ReEnter ();
			OnDo(gameObject);
		}
	}
}