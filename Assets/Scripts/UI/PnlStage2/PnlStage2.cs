/// UI分析工具自动生成代码
/// PnlStage2UI主模块
/// 
using System;
using UnityEngine;
namespace PnlStage2 {
	public class PnlStage2 : UIPhaseBase {
		private static PnlStage2 instance = null;
		public static PnlStage2 Instance {
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