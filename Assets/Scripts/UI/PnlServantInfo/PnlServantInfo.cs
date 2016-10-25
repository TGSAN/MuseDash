/// UI分析工具自动生成代码
/// PnlServantInfoUI主模块
/// 
using System;
using UnityEngine;
namespace PnlServantInfo {
	public class PnlServantInfo : UIPhaseBase {
		private static PnlServantInfo instance = null;
		public static PnlServantInfo Instance {
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