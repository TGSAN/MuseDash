/// UI分析工具自动生成代码
/// PnlVictoryUI主模块
/// 
using System;
using UnityEngine;
using FormulaBase;
using GameLogic;


namespace PnlVictory {
	public class PnlVictory : UIPhaseBase {
		private static PnlVictory instance = null;
		public static PnlVictory Instance {
			get {
					return instance;
			}
		}

		public UISprite sprGrade;
		public UITexture txrCharact;

		void Start() {
			instance = this;
			this.SetTxrByCharacter ();
		}

		public override void OnShow () {
			SoundEffectComponent.Instance.SayByCurrentRole (GameGlobal.SOUND_TYPE_LAST_NODE);
		}

		public override void OnHide () {
		}

		private void SetTxrByCharacter() {
			int heroIndex = BattleRoleAttributeComponent.Instance.Host.GetDynamicIntByKey (SignKeys.ID);
			string txrName = ConfigPool.Instance.GetConfigStringValue ("character", heroIndex.ToString (), "image_victory");
			ResourceLoader.Instance.Load (txrName, this.__LoadTxr);
		}

		private void __LoadTxr(UnityEngine.Object resObj) {
			Texture t = resObj as Texture;
			if (t == null) {
				int heroIndex = BattleRoleAttributeComponent.Instance.Host.GetDynamicIntByKey (SignKeys.ID);
				string txrName = ConfigPool.Instance.GetConfigStringValue ("character", heroIndex.ToString (), "image_victory");
				Debug.Log ("Load character " + heroIndex + " PnlVictory texture failed : " + txrName);
			}

			this.txrCharact.mainTexture = t;
		}
	}
}