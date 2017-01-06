/// UI分析工具自动生成代码
/// PnlTaskUI主模块
/// 
using System;
using UnityEngine;
namespace PnlTask {
	public class PnlTask : UIPhaseBase {
		private static PnlTask instance = null;
		public static PnlTask Instance {
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