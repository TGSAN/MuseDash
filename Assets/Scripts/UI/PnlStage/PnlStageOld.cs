using Assets.Scripts.NGUI;
using FormulaBase;
using GameLogic;

/// UI分析工具自动生成代码
/// PnlStageUI主模块
///
using System;
using System.Collections;
using UnityEngine;

namespace PnlStageOld
{
    public class PnlStageOld : UIPhaseBase
    {
        private static PnlStageOld instance = null;

        public static PnlStageOld Instance
        {
            get { return instance; }
        }

        private void Start()
        {
            instance = this;
        }

        public override void OnShow()
        {
            gameObject.SetActive(true);
            PnlScrollCircle.instance.OnShow();
        }

        public override void OnHide()
        {
            PnlHome.PnlHome.Instance.PlayBGM();
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