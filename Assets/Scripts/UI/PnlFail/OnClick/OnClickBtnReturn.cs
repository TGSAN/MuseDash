/// UI分析工具自动生成代码
/// 未有描述
/// 
using System;
using UnityEngine;
using FormulaBase;

namespace PnlFail {
	public class OnClickBtnReturn : UIPhaseOnClickBase {
		public static void Do(GameObject gameObject) {
			StageBattleComponent.Instance.Exit ();
			OnDo(gameObject);
		}
	}
}