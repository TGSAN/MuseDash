/// UI分析工具自动生成代码
/// PnlCharUpgradeUI主模块
/// 
using System;
using UnityEngine;
namespace PnlCharUpgrade {
	public class PnlCharUpgrade : UIPhaseBase {
		private static PnlCharUpgrade instance = null;
		public static PnlCharUpgrade Instance {
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