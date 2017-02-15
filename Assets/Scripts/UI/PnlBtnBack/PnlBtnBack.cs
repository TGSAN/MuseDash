/// UI分析工具自动生成代码
/// PnlBtnBackUI主模块
/// 
using System;
using UnityEngine;
namespace PnlBtnBack {
	public class PnlBtnBack : UIPhaseBase {
		private static PnlBtnBack instance = null;
		public static PnlBtnBack Instance {
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