/// UI分析工具自动生成代码
/// PnlSupplyUI主模块
/// 
using System;
using UnityEngine;
namespace PnlShop {
	public class PnlSupply : UIPhaseBase {
		private static PnlSupply instance = null;
		public static PnlSupply Instance {
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