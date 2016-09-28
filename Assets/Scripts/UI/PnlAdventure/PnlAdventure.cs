using FormulaBase;
using GameLogic;

/// UI分析工具自动生成代码
/// PnlAdventureUI主模块
///
using System;
using UnityEngine;
using FormulaBase;
using System.Collections;

namespace PnlAdventure
{
    public class PnlAdventure : UIPhaseBase
    {
        private static PnlAdventure instance = null;

        public static PnlAdventure Instance
        {
            get
            {
                return instance;
            }
        }

		private const int SHOW_GIRL_LAYER_ORDER = 2;
        private const string SHOW_CHILD_UI = "PnlStage";
        public static bool backFromBattle = false;

		private GameObject currentSprite;

		public Transform spiParent;

        void Start()
        {
            instance = this;
        }

        private void Update()
        {
            if (SceneAudioManager.Instance == null)
            {
                return;
            }

            if (!SceneAudioManager.Instance.bgm.isPlaying)
            {
                this.PlayBgm();
            }
        }

        public override void OnShow() {
			if (!backFromBattle) {
				this.PlayBgm ();
			}

			backFromBattle = false;
			this.ShowRole ();
        }

        public override void OnHide()
        {
        }

		public override void BeCatched () {
			if (backFromBattle) {
				Debug.Log ("backFromBattle");
				UISceneHelper.Instance.HideUi (this.gameObject.name);
				//UISceneHelper.Instance.ShowUi (SHOW_CHILD_UI);
				UISceneHelper.Instance.MarkShowOnLoad (this.gameObject.name, false);
				UISceneHelper.Instance.MarkShowOnLoad (SHOW_CHILD_UI, true);
				return;
			}
		}

		public void ShowRole() {
			int defaultIdx = 1;
			int roleIdx = RoleManageComponent.Instance.GetFightGirlIndex();
			if (roleIdx < 0) {
				roleIdx = defaultIdx;
				RoleManageComponent.Instance.SetFightGirlIndex (roleIdx);
			}

			this.OnSpiAnimLoaded (roleIdx);
		}

		private void OnSpiAnimLoaded(int idx) {
			if (spiParent.childCount > 0) {
				spiParent.DestroyChildren ();
			}

			this.currentSprite = null;
			string path = ConfigPool.Instance.GetConfigStringValue ("character", idx.ToString (), "char_show");
			var template = Resources.Load (path) as GameObject;
			if (template) {
				var go = GameObject.Instantiate (Resources.Load (path)) as GameObject;
				go.transform.SetParent (spiParent, false);
				go.SetActive (true);
				go.transform.localPosition = Vector3.zero;
				go.transform.localEulerAngles = Vector3.zero;
				this.currentSprite = go;

				// 设置sort order以适应界面需求
				SpineActionController sac = this.currentSprite.GetComponent<SpineActionController> ();
				if (sac) {
					sac.Init (-1);
					SpineActionController.Play (ACTION_KEYS.STAND, this.currentSprite);
					SpineActionController.SetSkeletonOrder (SHOW_GIRL_LAYER_ORDER, this.currentSprite);
				}
			} else {
				Debug.LogError ("加载未获得对象");
			}
		}

        private void PlayBgm()
        {
            int heroIndex = RoleManageComponent.Instance.GetFightGirlIndex();
            if (heroIndex < 0) {
                return;
            }

            if (SoundEffectComponent.Instance == null) {
                return;
            }

            if (SoundEffectComponent.Instance.IsPause()) {
                return;
            }

            string _speaker = SoundEffectComponent.Instance.SpeakerOfType(GameGlobal.SOUND_TYPE_UI_BGM);
            string name = ConfigPool.Instance.GetConfigStringValue("character", (RoleManageComponent.RoleIndexToId(heroIndex)).ToString(), "name");
            if (SoundEffectComponent.Instance.IsPlaying(GameGlobal.SOUND_TYPE_UI_BGM) && _speaker == name)
            {
                return;
            }

            SoundEffectComponent.Instance.Say(name, GameGlobal.SOUND_TYPE_UI_BGM, "bgm_");
        }
    }
}