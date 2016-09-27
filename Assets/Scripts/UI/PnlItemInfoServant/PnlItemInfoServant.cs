/// UI分析工具自动生成代码
/// PnlItemInfoServantUI主模块
/// 
using System;
using UnityEngine;
namespace PnlItemInfoServant {
	public class PnlItemInfoServant : UIPhaseBase {
		private static PnlItemInfoServant instance = null;
		public static PnlItemInfoServant Instance {
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