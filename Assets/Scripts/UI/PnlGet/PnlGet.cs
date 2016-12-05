using Assets.Scripts.Common.Tools;
using com.ootii.Messages;

/// UI分析工具自动生成代码
/// PnlGetUI主模块
///
using System;
using UnityEngine;

namespace PnlGet
{
    public class PnlGet : UIPhaseBase
    {
        private static PnlGet instance = null;
        public RandomObjFly coin, crystal, charm, exp;

        public static PnlGet Instance
        {
            get
            {
                return instance;
            }
        }

        public override void OnShow()
        {
        }

        public override void OnHide()
        {
        }

        public override void BeCatched()
        {
            instance = this;
            MessageDispatcher.AddListener("ADD_COIN", m =>
            {
                coin.FlyAll(30);
            }, false);
        }

        private void OnDestroy()
        {
            instance = null;
        }
    }
}