/// UI分析工具自动生成代码
/// 
/// 
using System;
using UnityEngine;
using FormulaBase;
using GameLogic;


namespace ResurgencePanel {
	public class OnClickRebornButton {
		private const int cost = 10;
		public static void Do(GameObject gameObject) {
			bool _result = AccountCrystalManagerComponent.Instance.ChangeDiamond (-cost, true, new HttpResponseDelegate (((bool result) => {
				if (!result) {
					CommonPanel.GetInstance ().ShowTextLackDiamond ("钻石不足", () => {
						OnClickGiveUpButton.Do (null);
					});
				}

				ResurgencePanelScript.Instance.ReviveShow ();
			})));

			if (!_result) {
				CommonPanel.GetInstance ().ShowTextLackDiamond ("钻石不足", () => {
					OnClickGiveUpButton.Do (null);
				});
			}
		}
	}
}