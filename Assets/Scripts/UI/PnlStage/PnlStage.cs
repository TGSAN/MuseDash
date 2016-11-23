using Assets.Scripts.NGUI;
using FormulaBase;
using GameLogic;

/// UI分析工具自动生成代码
/// PnlStageUI主模块
///
using System;
using System.Collections;
using UnityEngine;

namespace PnlStage
{
    public class PnlStage : UIPhaseBase
    {
        private static PnlStage instance = null;

        public static PnlStage Instance
        {
            get { return instance; }
        }

        private Coroutine _loadMusicCoroutine;
        //public AudioSource diskAudioSource;

        private void Start()
        {
            instance = this;
        }

        public override void OnShow()
        {
            PnlScrollCircle.instance.OnShow();
        }

        public override void OnHide()
        {
            PnlAdventure.PnlAdventure.Instance.PlayBGM();
        }

        public void OnSongChanged(int idx)
        {
            Debug.Log("Stage selected " + idx);
            if (idx <= 0)
            {
                Debug.Log("Unlaw stage : " + idx);
                return;
            }

            int len = ConfigPool.Instance.GetConfigLenght("stage");
            if (idx >= len)
            {
                Debug.Log("Unlaw stage : " + idx);
                return;
            }

            StageBattleComponent.Instance.InitById(idx);
        }
    }
}