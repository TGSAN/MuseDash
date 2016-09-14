/// UI分析工具自动生成代码
/// VictoryPanelUI主模块
/// 
using System;
using UnityEngine;
namespace VictoryPanel {
	public class VictoryPanel : UIPhaseBase {
		private static VictoryPanel instance = null;
		public static VictoryPanel Instance {
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