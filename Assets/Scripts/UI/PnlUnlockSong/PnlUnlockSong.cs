/// UI分析工具自动生成代码
/// PnlUnlockSongUI主模块
/// 
using System;
using UnityEngine;
namespace PnlUnlockSong {
	public class PnlUnlockSong : UIPhaseBase {
		private static PnlUnlockSong instance = null;
		public static PnlUnlockSong Instance {
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