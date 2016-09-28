/// UI分析工具自动生成代码
/// PnlItemInfoServantSoulUI主模块
/// 
using System;
using UnityEngine;
namespace PnlItemInfoServantSoul {
	public class PnlItemInfoServantSoul : UIPhaseBase {
		private static PnlItemInfoServantSoul instance = null;
		public static PnlItemInfoServantSoul Instance {
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