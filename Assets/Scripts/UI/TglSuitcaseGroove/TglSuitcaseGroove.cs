/// UI分析工具自动生成代码
/// TglSuitcaseGrooveUI主模块
/// 
using System;
using UnityEngine;
namespace TglSuitcaseGroove {
	public class TglSuitcaseGroove : UIPhaseBase {
		private static TglSuitcaseGroove instance = null;
		public static TglSuitcaseGroove Instance {
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