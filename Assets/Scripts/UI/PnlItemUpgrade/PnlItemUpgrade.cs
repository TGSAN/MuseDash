/// UI分析工具自动生成代码
/// PnlItemUpgradeUI主模块
/// 
using System;
using UnityEngine;
namespace PnlItemUpgrade {
	public class PnlItemUpgrade : UIPhaseBase {
		private static PnlItemUpgrade instance = null;
		public static PnlItemUpgrade Instance {
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