/// UI分析工具自动生成代码
/// 未有描述
/// 
using System;
using UnityEngine;
using DYUnityLib;
using GameLogic;
using FormulaBase;


namespace result_fail {
	public class OnClickrestart_label : UIPhaseOnClickBase {
		public static void Do(GameObject gameObject) {
			OnDo (gameObject);
			Revive ();
		}

		public static void Revive() {
			Time.timeScale = GameGlobal.TIME_SCALE;
			FixUpdateTimer.ResumeTimer ();

			AudioManager.Instance.ResumeBackGroundMusic ();
			AudioManager.Instance.SetBackGroundMusicTimeScale (GameGlobal.TIME_SCALE);

			StageBattleComponent.Instance.Revive ();
			//GirlManager.Instance.PlayGirlReviveAnimation ();
			GirlManager.Instance.StartPhysicDetect ();
			GirlManager.Instance.StartAutoReduceEnergy ();

			GameGlobal.gGameMusicScene.SetAnimationSpeed (1f);
			// StageBattleComponent.Instance.ClearNearByObject ();
		}
	}
}