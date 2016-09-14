/// UI分析工具自动生成代码
/// BagPanel界面主模块
/// 
using System;
using UnityEngine;
namespace BagPanel {
	public class BagPanel : UIPhaseBase {
		private static BagPanel instance = null;
		public static BagPanel Instance {
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