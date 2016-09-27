/// UI分析工具自动生成代码
/// ItemImageMaterialUI主模块
/// 
using System;
using UnityEngine;
namespace ItemImageMaterial {
	public class ItemImageMaterial : UIPhaseBase {
		private static ItemImageMaterial instance = null;
		public static ItemImageMaterial Instance {
			get {
					return instance;
			}
		}

		public UITexture txrIcon;
		public string infoPanelName;

		void Start() {
			instance = this;
		}

		public override void OnShow () {
		}

		public override void OnHide () {
		}

		public override void OnShow (FormulaBase.FormulaHost host) {
			this.SetTxrByHost (host);
		}

		private void SetTxrByHost(FormulaBase.FormulaHost host) {
			string txrName = host.GetDynamicStrByKey (FormulaBase.SignKeys.ICON);
			if (txrName == null || ResourceLoader.Instance == null) {
				return;
			}

			ResourceLoader.Instance.Load (txrName, this.__LoadTxr);
		}

		private void __LoadTxr(UnityEngine.Object resObj) {
			Texture t = resObj as Texture;
			if (t == null) {
			}

			this.txrIcon.mainTexture = t;
		}
	}
}