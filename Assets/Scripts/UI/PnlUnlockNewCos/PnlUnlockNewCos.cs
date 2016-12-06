/// UI分析工具自动生成代码
/// PnlUnlockNewCosUI主模块
/// 
using System;
using UnityEngine;
namespace PnlUnlockNewCos {
	public class PnlUnlockNewCos : UIPhaseBase {
		private static PnlUnlockNewCos instance = null;
		public static PnlUnlockNewCos Instance {
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