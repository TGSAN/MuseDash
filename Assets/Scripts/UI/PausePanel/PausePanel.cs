/// UI分析工具自动生成代码
/// PausePanelUI主模块
/// 
using System;
using UnityEngine;
namespace PausePanel {
	public class PausePanel : UIPhaseBase {
		private static PausePanel instance = null;
		public static PausePanel Instance {
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