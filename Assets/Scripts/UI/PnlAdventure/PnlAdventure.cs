using FormulaBase;
using GameLogic;

/// UI分析工具自动生成代码
/// PnlAdventureUI主模块
///
using System;
using UnityEngine;

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

        private const string SHOW_CHILD_UI = "PnlStage";
        public static bool backFromBattle = false;

        private void Start()
        {
            instance = this;
            this.PlayBgm();
            if (backFromBattle)
            {
                UISceneHelper.Instance.HideUi(this.gameObject.name);
                UISceneHelper.Instance.ShowUi(SHOW_CHILD_UI);
            }
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
        }

        public override void OnHide()
        {
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