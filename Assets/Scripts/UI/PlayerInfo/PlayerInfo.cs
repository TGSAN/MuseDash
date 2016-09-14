/// UI分析工具自动生成代码
/// PlayerInfoUI主模块
/// 
using System;
using UnityEngine;
namespace PlayerInfo {
	public class PlayerInfo : UIPhaseBase {
		private static PlayerInfo instance = null;
		public static PlayerInfo Instance {
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