/// UI分析工具自动生成代码
/// PnlAnnouncementUI主模块
/// 
using System;
using UnityEngine;
namespace PnlAnnouncement {
	public class PnlAnnouncement : UIPhaseBase {
		private static PnlAnnouncement instance = null;
		public static PnlAnnouncement Instance {
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