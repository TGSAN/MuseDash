/// UI分析工具自动生成代码
/// result_failUI主模块
/// 
using System;
using UnityEngine;
using FormulaBase;
using DYUnityLib;

namespace result_fail {
	public class result_fail : UIPhaseBase {
		private static result_fail instance = null;
		public static result_fail Instance {
			get {
					return instance;
			}
		}

		public UITexture txrCharact;

		void Start() {
			instance = this;
			this.SetTxrByCharacter ();
		}

		public override void OnShow () {
			// this.SetTxrByCharacter ();
			this.Pause ();
		}

		public override void OnHide () {
			this.gameObject.SetActive (false);
		}

		private void Pause() {
			AudioManager.Instance.PauseBackGroundMusic ();
			FixUpdateTimer.PauseTimer ();
			Time.timeScale = 0;
		}

		private void SetTxrByCharacter() {
			int heroIndex = BattleRoleAttributeComponent.Instance.Host.GetDynamicIntByKey (SignKeys.ID);
			string txrName = ConfigPool.Instance.GetConfigStringValue ("character", heroIndex.ToString (), "image_fail");
			ResourceLoader.Instance.Load (txrName, this.__LoadTxr);
		}

		private void __LoadTxr(UnityEngine.Object resObj) {
			Texture t = resObj as Texture;
			if (t == null) {
				int heroIndex = BattleRoleAttributeComponent.Instance.Host.GetDynamicIntByKey (SignKeys.ID);
				string txrName = ConfigPool.Instance.GetConfigStringValue ("character", heroIndex.ToString (), "image_fail");
				Debug.Log ("Load character " + heroIndex + " result_fail texture failed : " + txrName);
			}

			this.txrCharact.mainTexture = t;
		}
	}
}