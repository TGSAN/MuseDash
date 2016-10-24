/// UI分析工具自动生成代码
/// PnlStoreUI主模块
/// 
using System;
using UnityEngine;
namespace PnlStore {
	public class PnlStore : UIPhaseBase {
		private static PnlStore instance = null;
		public static PnlStore Instance {
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