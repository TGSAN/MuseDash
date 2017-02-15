/// UI分析工具自动生成代码
/// 未有描述
/// 
using System;
using UnityEngine;
namespace PnlAchievement {
	public class OnClickBtnBack : UIPhaseOnClickBase {
		public static void Do(GameObject gameObject) {
			Debug.Log ("===========");
				OnDo(gameObject);
			PnlStageInfo.PnlStageInfo.Instance.OnHide ();
		}
	}
}