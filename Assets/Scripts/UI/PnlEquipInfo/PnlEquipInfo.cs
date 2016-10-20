/// UI分析工具自动生成代码
/// PnlEquipInfoUI主模块
/// 
using System;
using UnityEngine;
namespace PnlEquipInfo {
	public class PnlEquipInfo : UIPhaseBase {
		private static PnlEquipInfo instance = null;
		public static PnlEquipInfo Instance {
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