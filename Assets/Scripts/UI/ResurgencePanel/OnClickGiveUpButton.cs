/// UI分析工具自动生成代码
/// 
/// 
using System;
using UnityEngine;
using GameLogic;
using FormulaBase;


namespace ResurgencePanel {
	public class OnClickGiveUpButton {
		public static void Do(GameObject gameObject) {
			// Pay back half physical.
			//Time.timeScale = GameGlobal.TIME_SCALE;
			ResurgencePanelScript.Instance.DeadShow ();
			ResurgencePanelScript.Instance.Dead ();
			//ResurgencePanelScript.Instance.gameObject.SetActive (false);

			//AudioManager.Instance.ResumeBackGroundMusic ();
			//StageBattleComponent.Instance.Dead ();
		}
	}
}