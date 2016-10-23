/// UI分析工具自动生成代码
/// PnlEquipInfoUI主模块
///
using System;
using UnityEngine;

namespace PnlEquipInfo
{
    public class PnlEquipInfo : UIPhaseBase
    {
        public UILabel txtVigour, txtStamina, txtStrengh, txtExp;
        public UILabel txtDisc, txtAbility;
        public Transform star;
        private static PnlEquipInfo instance = null;

        public static PnlEquipInfo Instance
        {
            get
            {
                return instance;
            }
        }

        private void Start()
        {
            instance = this;
        }

        public override void OnShow()
        {
        }

        public override void OnHide()
        {
        }
    }
}