/// UI分析工具自动生成代码
/// PnlItemInfoFoodUI主模块
/// 
using System;
using UnityEngine;
namespace PnlItemInfoFood {
	public class PnlItemInfoFood : UIPhaseBase {
		private static PnlItemInfoFood instance = null;
		public static PnlItemInfoFood Instance {
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