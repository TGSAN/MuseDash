/// UI分析工具自动生成代码
/// PnlMainMenuUI主模块
/// 
using System;
using UnityEngine;
namespace PnlMainMenu {
	public class PnlMainMenu : UIPhaseBase {
		private static PnlMainMenu instance = null;
		public static PnlMainMenu Instance {
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