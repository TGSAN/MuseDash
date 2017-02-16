/// UI分析工具自动生成代码
/// PnlServantUI主模块
/// 
using System;
using UnityEngine;
namespace PnlServant {
	public class PnlServant : UIPhaseBase {
		private static PnlServant instance = null;
		public static PnlServant Instance {
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