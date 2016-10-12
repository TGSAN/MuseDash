/// UI分析工具自动生成代码
/// PnlCapsuleOpenUI主模块
/// 
using System;
using UnityEngine;
namespace PnlCapsuleOpen {
	public class PnlCapsuleOpen : UIPhaseBase {
		private static PnlCapsuleOpen instance = null;
		public static PnlCapsuleOpen Instance {
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