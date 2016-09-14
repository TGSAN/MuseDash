/// UI分析工具自动生成代码
/// PnlCharChoseUI主模块
/// 
using System;
using UnityEngine;
namespace PnlCharChose {
	public class PnlCharChose : UIPhaseBase {
		private static PnlCharChose instance = null;
		public static PnlCharChose Instance {
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