using FormulaBase;

/// UI分析工具自动生成代码
/// PnlUnlockNewCosUI主模块
///
using System;
using UnityEngine;

namespace PnlUnlockNewCos
{
    public class PnlUnlockNewCos : UIPhaseBase
    {
        public Transform spiParent;
        public UILabel txtName;
        public UIButton btnCheck;
        private static PnlUnlockNewCos instance = null;

        public static PnlUnlockNewCos Instance
        {
            get
            {
                return instance;
            }
        }

        public override void BeCatched()
        {
            instance = this;
        }

        public override void OnShow(FormulaHost host)
        {
            var suitName = ConfigPool.Instance.GetConfigStringValue("items", host.GetDynamicStrByKey(SignKeys.ID),
                "suit");
            var charCos = new CharCos(suitName);

            txtName.text = suitName;
            ResourceLoader.Instance.Load(charCos.path, res =>
            {
                var go = Instantiate(res, spiParent) as GameObject;
                go.transform.localPosition = Vector3.zero;
                go.transform.localEulerAngles = Vector3.zero;
            });
            gameObject.SetActive(true);
        }
    }
}