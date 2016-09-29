/// UI分析工具自动生成代码
/// ItemImageFoodUI主模块
/// 
using System;
using UnityEngine;
namespace ItemImageFood {
	public class ItemImageFood : UIPhaseBase {
		private static ItemImageFood instance = null;
		public static ItemImageFood Instance {
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