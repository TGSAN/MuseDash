using Assets.Scripts.Common.Manager;

/// UI分析工具自动生成代码
/// PnlDailyTaskUI主模块
///
using System;
using UnityEngine;

namespace PnlDailyTask
{
    public class PnlDailyTask : UIPhaseBase
    {
        private static PnlDailyTask instance = null;

        public static PnlDailyTask Instance
        {
            get
            {
                return instance;
            }
        }

        public override void BeCatched()
        {
            instance = this;

            var a = DailyTaskManager.instance;
        }

        public override void OnShow()
        {
        }

        public override void OnHide()
        {
        }
    }
}