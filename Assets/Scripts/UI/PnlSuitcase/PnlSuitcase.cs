/// UI分析工具自动生成代码
/// PnlSuitcaseUI主模块
/// 
using System;
using UnityEngine;
namespace PnlSuitcase {
	public class PnlSuitcase : UIPhaseBase {
		private static PnlSuitcase instance = null;
		public static PnlSuitcase Instance {
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