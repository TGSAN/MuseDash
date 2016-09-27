/// UI分析工具自动生成代码
/// ItemImageUI主模块
/// 
using System;
using UnityEngine;
namespace ItemImage {
	public class ItemImage : UIPhaseBase {
		private static ItemImage instance = null;
		public static ItemImage Instance {
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