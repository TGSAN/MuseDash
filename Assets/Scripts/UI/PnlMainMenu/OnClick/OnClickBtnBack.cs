/// UI分析工具自动生成代码
/// 未有描述
/// 
using System;
using UnityEngine;
namespace PnlMainMenu {
	public class OnClickBtnBack : UIPhaseOnClickBase {
		private static string[] UI_ORDER_STAGE = new string[]{
			"PnlAdventure",
			"PnlStage",
			"PnlAchievement"
		};
		
		public static void Do(GameObject gameObject) {
			OnDo (gameObject);
			for (int i = 0; i < UI_ORDER_STAGE.Length; i++) {
				string _uiname = UI_ORDER_STAGE [i];
				if (!UISceneHelper.Instance.IsUiActive (_uiname)) {
					continue;
				}

				if (i <= 0) {
					continue;
				}

				string _backuiname = UI_ORDER_STAGE [i - 1];
				UISceneHelper.Instance.HideUi (_uiname);
				UISceneHelper.Instance.ShowUi (_backuiname);
				break;
			}
		}
	}
}