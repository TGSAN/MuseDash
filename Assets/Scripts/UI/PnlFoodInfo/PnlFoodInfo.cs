/// UI分析工具自动生成代码
/// PnlFoodInfoUI主模块
/// 
using System;
using UnityEngine;
namespace PnlFoodInfo {
	public class PnlFoodInfo : UIPhaseBase {
		private static PnlFoodInfo instance = null;
		public static PnlFoodInfo Instance {
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