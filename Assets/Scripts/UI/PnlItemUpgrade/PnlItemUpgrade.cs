using Assets.Scripts.Common;
using FormulaBase;

/// UI分析工具自动生成代码
/// PnlItemUpgradeUI主模块
///
using System;
using UnityEngine;

namespace PnlItemUpgrade
{
    public class PnlItemUpgrade : UIPhaseBase
    {
        private static PnlItemUpgrade instance = null;
        public UITexture[] itemTexs;
        public UITexture mainTex1, mainTex2;
        public Animator animator1, animator2;

        public static PnlItemUpgrade Instance
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
        }

        public void OnShow(FormulaHost host, FormulaHost[] hosts, UpgradeInfo result)
        {
            gameObject.SetActive(true);
            ResourceLoader.Instance.Load(host, mainTex1);
            ResourceLoader.Instance.Load(host, mainTex2);
            for (int i = 0; i < hosts.Length; i++)
            {
                ResourceLoader.Instance.Load(hosts[i], itemTexs[i]);
            }
            animator1.Play(hosts.Length.ToString() + "item");
            DOTweenUtils.Delay(() =>
            {
                animator1.gameObject.SetActive(false);
                animator2.gameObject.SetActive(true);
            }, 2.4f);
        }
    }
}