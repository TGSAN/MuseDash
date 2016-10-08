/// UI分析工具自动生成代码
/// PnlCharUI主模块
/// 
using System;
using UnityEngine;
namespace PnlChar {
	public class PnlChar : UIPhaseBase {
		private static PnlChar instance = null;
		public static PnlChar Instance {
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