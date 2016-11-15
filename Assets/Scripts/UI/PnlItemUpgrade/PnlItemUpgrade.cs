using Assets.Scripts.Common;
using FormulaBase;

/// UI分析工具自动生成代码
/// PnlItemUpgradeUI主模块
///
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace PnlItemUpgrade
{
    public class PnlItemUpgrade : UIPhaseBase
    {
        private static PnlItemUpgrade instance = null;
        public UITexture[] itemTexs;
        public UITexture mainTex1, mainTex2;
        public Animator animator1, animator2;
        public List<GameObject> goQualities;

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
            animator1.gameObject.SetActive(true);
            ResourceLoader.Instance.LoadItemIcon(host, mainTex1);
            ResourceLoader.Instance.LoadItemIcon(host, mainTex2);
            for (int i = 0; i < hosts.Length; i++)
            {
                ResourceLoader.Instance.LoadItemIcon(hosts[i], itemTexs[i]);
            }
            animator1.Play(hosts.Length.ToString() + "item");
            DOTweenUtils.Delay(() =>
            {
                animator1.gameObject.SetActive(false);
                animator2.gameObject.SetActive(true);
            }, 2.4f);
            DOTweenUtils.Delay(() =>
            {
                animator2.enabled = true;
            }, 2.5f);

            var quality = host.GetDynamicIntByKey(SignKeys.QUALITY);

            for (var i = 0; i < goQualities.Count; i++)
            {
                goQualities[i].gameObject.SetActive(quality == i + 1);
            }
        }
    }
}