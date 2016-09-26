/// UI分析工具自动生成代码
/// 未有描述
/// 
using System;
using UnityEngine;
using FormulaBase;
using GameLogic;
using Assets.Scripts.NGUI;
using System.Collections;

namespace PnlAchievement {
	public class OnClickBtnStart : UIPhaseOnClickBase {
		public static void Do(GameObject gameObject) {
			OnDo (gameObject);
			PnlAchievement.Instance.EnterBattle ();
		}
	}
}