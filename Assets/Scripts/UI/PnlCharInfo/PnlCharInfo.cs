/// UI分析工具自动生成代码
/// PnlCharInfoUI主模块
/// 
using System;
using UnityEngine;
namespace PnlCharInfo {
	public class PnlCharInfo : UIPhaseBase {
		private static PnlCharInfo instance = null;
		public static PnlCharInfo Instance {
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