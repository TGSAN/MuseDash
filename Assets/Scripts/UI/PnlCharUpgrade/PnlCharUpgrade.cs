using FormulaBase;

/// UI分析工具自动生成代码
/// PnlCharUpgradeUI主模块
///
using System;
using UnityEngine;

namespace PnlCharUpgrade
{
    public class PnlCharUpgrade : UIPhaseBase
    {
        private static PnlCharUpgrade instance = null;
        public UISprite sprChar, sprCharFade;
        public UITexture[] itemTexs;

        public static PnlCharUpgrade Instance
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

        public void OnShow(FormulaHost roleHost, FormulaHost[] hosts)
        {
            //sprChar.spriteName = ""
        }

        public override void BeCatched()
        {
            instance = this;
        }
    }
}