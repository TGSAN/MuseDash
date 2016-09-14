/// UI分析工具自动生成代码
/// PnlBattleUI主模块
/// 
using System;
using UnityEngine;
namespace PnlBattle {
	public class PnlBattle : UIPhaseBase {
		private static PnlBattle instance = null;
		public static PnlBattle Instance {
			get {
					return instance;
			}
		}

		void Start() {
			instance = this;
		}

		public override void OnShow () {
		}

		public override void OnHide () {
		}
	}
}