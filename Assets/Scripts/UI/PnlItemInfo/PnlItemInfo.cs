/// UI分析工具自动生成代码
/// PnlItemInfoUI主模块
/// 
using System;
using UnityEngine;
namespace PnlItemInfo {
	public class PnlItemInfo : UIPhaseBase {
		private static PnlItemInfo instance = null;
		public static PnlItemInfo Instance {
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