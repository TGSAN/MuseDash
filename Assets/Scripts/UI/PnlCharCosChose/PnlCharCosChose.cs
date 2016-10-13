/// UI分析工具自动生成代码
/// PnlCharCosChoseUI主模块
/// 
using System;
using UnityEngine;
namespace PnlCharCosChose {
	public class PnlCharCosChose : UIPhaseBase {
		private static PnlCharCosChose instance = null;
		public static PnlCharCosChose Instance {
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