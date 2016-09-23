/// UI分析工具自动生成代码
/// BtnCharGrooveUI主模块
/// 
using System;
using UnityEngine;
namespace BtnCharGroove {
	public class BtnCharGroove : UIPhaseBase {
		private static BtnCharGroove instance = null;
		public static BtnCharGroove Instance {
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