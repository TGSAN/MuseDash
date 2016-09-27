/// UI分析工具自动生成代码
/// 未有描述
/// 
using System;
using UnityEngine;
using Assets.Scripts.NGUI;
using FormulaBase;

namespace PnlStage {
	public class OnClickTxtTouchToStart : UIPhaseOnClickBase {
		public static void Do(GameObject gameObject) {
			if (PnlScrollCircle.instance.CatchClip == null) {
				return;
			}

			string catchMusicName = PnlScrollCircle.instance.CatchClip.name;
			string stageMusicName = StageBattleComponent.Instance.GetMusicName ();
			if (catchMusicName != stageMusicName) {
				Debug.Log ("Enter stage ready panel failed with different music : " + catchMusicName + " / need : " + stageMusicName);
				return;
			}

			OnDo (gameObject);
		}
	}
}