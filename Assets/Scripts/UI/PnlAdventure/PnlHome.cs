using FormulaBase;
using GameLogic;

/// UI分析工具自动生成代码
/// PnlHomeUI主模块
///
using System;
using UnityEngine;

namespace PnlHome
{
    public class PnlHome : UIPhaseBase
    {
        private static PnlHome instance = null;
        public GameObject urchin, marija, buro;

        public static PnlHome Instance
        {
            get
            {
                return instance;
            }
        }

        private const string SHOW_CHILD_UI = "PnlStage";
        public static bool backFromBattle = false;

        private void Start()
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

        public override void OnShow()
        {
            ChoseGirl();
            gameObject.SetActive(true);
            if (!backFromBattle)
            {
                this.PlayBgm();
            }
            backFromBattle = false;
        }

        public void ChoseGirl()
        {
            var idx = RoleManageComponent.Instance.GetFightGirlIndex();
            urchin.SetActive(idx == 1);
            marija.SetActive(idx == 2);
            buro.SetActive(idx == 3);
        }

        public override void OnHide()
        {
        }

        public override void BeCatched()
        {
            if (backFromBattle)
            {
                Debug.Log("backFromBattle");
                UISceneHelper.Instance.HideUi(this.gameObject.name);
                //UISceneHelper.Instance.ShowUi (SHOW_CHILD_UI);
                UISceneHelper.Instance.MarkShowOnLoad(this.gameObject.name, false);
                UISceneHelper.Instance.MarkShowOnLoad(SHOW_CHILD_UI, true);
                return;
            }
        }

        public void ShowWaitingPanel()
        {
            CommonPanel.GetInstance().ShowWaittingPanel();
        }

        public void PlayBgm()
        {
            int heroIndex = RoleManageComponent.Instance.GetFightGirlIndex();
            if (heroIndex < 0)
            {
                return;
            }

            if (SoundEffectComponent.Instance == null)
            {
                return;
            }

            if (SoundEffectComponent.Instance.IsPause())
            {
                return;
            }

            /*string _speaker = SoundEffectComponent.Instance.SpeakerOfType(GameGlobal.SOUND_TYPE_UI_BGM);
            string name = ConfigPool.Instance.GetConfigStringValue("char_info", (RoleManageComponent.RoleIndexToId(heroIndex)).ToString(), "name");
            if (SoundEffectComponent.Instance.IsPlaying(GameGlobal.SOUND_TYPE_UI_BGM) && _speaker == name)
            {
                return;
            }

            SoundEffectComponent.Instance.Say(name, GameGlobal.SOUND_TYPE_UI_BGM, "bgm_");*/
        }

        public void PlayBGM()
        {
            int heroIndex = RoleManageComponent.Instance.GetFightGirlIndex();
            if (heroIndex < 0)
            {
                return;
            }

            if (SoundEffectComponent.Instance == null)
            {
                return;
            }

            if (SoundEffectComponent.Instance.IsPause())
            {
                return;
            }

            string name = ConfigPool.Instance.GetConfigStringValue("char_info", (RoleManageComponent.RoleIndexToId(heroIndex)).ToString(), "name");
            if (("bgm_" + name) == SceneAudioManager.Instance.bgm.clip.name)
            {
                return;
            }
            SoundEffectComponent.Instance.Say(name, GameGlobal.SOUND_TYPE_UI_BGM, "bgm_");
        }
    }
}