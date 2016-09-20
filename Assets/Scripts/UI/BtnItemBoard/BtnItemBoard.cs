/// UI分析工具自动生成代码
/// BtnItemBoardUI主模块
/// 
using System;
using UnityEngine;
namespace BtnItemBoard {
	public class BtnItemBoard : UIPhaseBase {
		private static BtnItemBoard instance = null;
		public static BtnItemBoard Instance {
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