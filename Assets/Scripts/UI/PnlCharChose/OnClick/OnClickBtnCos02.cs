/// UI分析工具自动生成代码
/// 未有描述
/// 
using System;
using UnityEngine;
using FormulaBase;

namespace PnlCharChose {
	public class OnClickBtnCos02 : UIPhaseOnClickBase {
		public static void Do(GameObject gameObject) {
			RoleManageComponent.Instance.SetFightGirlClothByOrder (2);
				OnDo(gameObject);
		}
	}
}