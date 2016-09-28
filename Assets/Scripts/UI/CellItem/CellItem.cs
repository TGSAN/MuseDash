/// UI分析工具自动生成代码
/// CellItemUI主模块
/// 
using System;
using UnityEngine;
namespace CellItem {
	public class CellItem : UIPhaseBase {
		private static CellItem instance = null;
		public static CellItem Instance {
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