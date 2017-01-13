/// UI分析工具自动生成代码
/// PnlShopUI主模块
/// 
using System;
using UnityEngine;
namespace PnlShop {
	public class PnlShop : UIPhaseBase {
		private static PnlShop instance = null;
		public static PnlShop Instance {
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