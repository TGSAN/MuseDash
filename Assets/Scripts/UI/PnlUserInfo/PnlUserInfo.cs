/// UI分析工具自动生成代码
/// PnlUserInfoUI主模块
/// 
using System;
using UnityEngine;
namespace PnlUserInfo {
	public class PnlUserInfo : UIPhaseBase {
		private static PnlUserInfo instance = null;
		public static PnlUserInfo Instance {
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