/// UI分析工具自动生成代码
/// PnlOptionUI主模块
/// 
using System;
using UnityEngine;
namespace PnlOption {
	public class PnlOption : UIPhaseBase {
		private static PnlOption instance = null;
		public static PnlOption Instance {
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