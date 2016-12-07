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
            spiParent.DestroyChildren();
            ResourceLoader.Instance.Load(charCos.path, res =>
            {
                if (res != null)
                {
                    var go = Instantiate(res) as GameObject;
                    go.transform.SetParent(spiParent, false);
                    go.GetComponent<Animator>().enabled = false;
                    go.GetComponent<SpineSynchroObjects>().enabled = false;
                    go.GetComponent<SpineMountController>().enabled = false;
                    go.SetActive(true);
                    go.transform.localPosition = Vector3.zero;
                    go.transform.localEulerAngles = Vector3.zero;
                    var skeletonAnim = go.GetComponent<SkeletonAnimation>();
                    skeletonAnim.loop = true;
                    skeletonAnim.AnimationName = "standby";
                    go.GetComponent<Renderer>().sortingOrder = 100;
                }
            });
            UIEventListener.Get(btnCheck.gameObject).onClick = go =>
            {
                PnlChar.PnlChar.Instance.OnShowCos(charCos);
            };
            gameObject.SetActive(true);
        }
    }
}