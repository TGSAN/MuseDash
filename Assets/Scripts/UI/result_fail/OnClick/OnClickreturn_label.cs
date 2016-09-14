/// UI分析工具自动生成代码
/// 未有描述
/// 
using System;
using UnityEngine;
using FormulaBase;


namespace result_fail {
	public class OnClickreturn_label : UIPhaseOnClickBase {
		public static void Do(GameObject gameObject) {
			OnDo(gameObject);
			StageBattleComponent.Instance.Exit ();
		}
	}
}