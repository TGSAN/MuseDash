/// UI分析工具自动生成代码
/// PnlAchievementInfoUI主模块
/// 
using System;
using UnityEngine;
namespace PnlAchievementInfo {
	public class PnlAchievementInfo : UIPhaseBase {
		private static PnlAchievementInfo instance = null;
		public static PnlAchievementInfo Instance {
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