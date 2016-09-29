/// UI分析工具自动生成代码
/// ItemImageServantUI主模块
/// 
using System;
using UnityEngine;
namespace ItemImageServant {
	public class ItemImageServant : UIPhaseBase {
		private static ItemImageServant instance = null;
		public static ItemImageServant Instance {
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