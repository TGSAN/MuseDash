/// UI分析工具自动生成代码
/// PnlItemInfoEquipUI主模块
/// 
using System;
using UnityEngine;
namespace PnlItemInfoEquip {
	public class PnlItemInfoEquip : UIPhaseBase {
		private static PnlItemInfoEquip instance = null;
		public static PnlItemInfoEquip Instance {
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