using Assets.Scripts.Common.Manager;
using FormulaBase;

/// UI分析工具自动生成代码
/// PnlAchievementInfoUI主模块
///
using System;
using System.Linq;
using UnityEngine;

namespace PnlAchievementInfo
{
    public class PnlAchievementInfo : UIPhaseBase
    {
        private static PnlAchievementInfo instance = null;
        public Transform combo, perfect, stars, clear;

        public static PnlAchievementInfo Instance
        {
            get
            {
                return instance;
            }
        }

        private void Start()
        {
        }

        public override void OnShow()
        {
            //var allAchievements = AchievementManager.instance.GetAchievements();
        }

        public override void OnHide()
        {
        }

        public override void BeCatched()
        {
            instance = this;
        }
    }
}