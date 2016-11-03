using FormulaBase;

/// UI分析工具自动生成代码
/// PnlItemInfoUI主模块
///
using System;
using UnityEngine;

namespace PnlItemInfo
{
    public class PnlItemInfo : UIPhaseBase
    {
        public UILabel txtVigour, txtStamina, txtStrengh, txtExp;
        public UILabel txtDisc, txtAbility;
        public Transform star;
        private static PnlItemInfo instance = null;

        public static PnlItemInfo Instance
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

        public override void OnShow(FormulaHost host)
        {
            host.SetAsUINotifyInstance();
            var vigour = host.Result(FormulaKeys.FORMULA_50);
            var stamina = host.Result(FormulaKeys.FORMULA_51);
            var strengh = host.Result(FormulaKeys.FORMULA_56);
            var exp = host.GetDynamicDataByKey(SignKeys.EXP);
            txtVigour.gameObject.SetActive(vigour > 0);
            txtStamina.gameObject.SetActive(stamina > 0);
            txtStrengh.gameObject.SetActive(strengh > 0);
            txtExp.gameObject.SetActive(exp > 0);
        }
    }
}