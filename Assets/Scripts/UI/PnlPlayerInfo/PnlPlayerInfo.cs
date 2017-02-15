/// UI分析工具自动生成代码
/// PnlPlayerInfoUI主模块
/// 
using System;
using UnityEngine;
namespace PnlPlayerInfo {
	public class PnlPlayerInfo : UIPhaseBase {
		private static PnlPlayerInfo instance = null;
		public static PnlPlayerInfo Instance {
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