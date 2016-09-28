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

		public UIButton btn;

		void Start() {
			instance = this;
			if (btn.onClick.Count <= 0) {
				EventDelegate _dlg = new EventDelegate ();
				_dlg.Set (this, "__OnClick");
				btn.onClick.Add (_dlg);
			}
		}

		public override void OnShow () {
		}

		public override void OnHide () {
		}

		/// <summary>
		/// Ons the click.
		/// 
		/// btn挂载在uiroot上不受fq editor常规按钮处理流程控制
		/// 于是特例化挂载回调方法
		/// </summary>
		private void __OnClick() {
			if (this.gameObject.transform.childCount <= 0) {
				return;
			}

			for (int i = 0; i < this.gameObject.transform.childCount; i++) {
				Transform t = this.gameObject.transform.GetChild (i);
				if (t == null || !t.gameObject.activeSelf) {
					continue;
				}


				ItemImageEquip.ItemImageEquip iie = t.gameObject.GetComponent<ItemImageEquip.ItemImageEquip> ();
				if (iie != null) {
					PnlSuitcase.PnlSuitcase.Instance.ShowInfo (iie.infoPanelName);
					continue;
				}

				ItemImageMaterial.ItemImageMaterial iim = t.gameObject.GetComponent<ItemImageMaterial.ItemImageMaterial> ();
				if (iim != null) {
					PnlSuitcase.PnlSuitcase.Instance.ShowInfo (iim.infoPanelName);
					continue;
				}

				ItemImagePet.ItemImagePet iip = t.gameObject.GetComponent<ItemImagePet.ItemImagePet> ();
				if (iip != null) {
					PnlSuitcase.PnlSuitcase.Instance.ShowInfo (iip.infoPanelName);
					continue;
				}
			}
		}
	}
}