using Assets.Scripts.Common;
using FormulaBase;

/// UI分析工具自动生成代码
/// PnlCharUpgradeUI主模块
///
using System;
using System.Linq;
using UnityEngine;

namespace PnlCharUpgrade
{
    public class PnlCharUpgrade : UIPhaseBase
    {
        private static PnlCharUpgrade instance = null;
        public UISprite sprChar, sprCharFade;
        public UITexture[] itemTexs;
        public GameObject[] txtsCool, txtsGreat, txtsPerfect;
        public SkeletonAnimation spiAnim;

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

        public void OnShow(FormulaHost roleHost, FormulaHost[] hosts, UpgradeInfo result)
        {
            gameObject.SetActive(true);
            var sprName = roleHost.GetDynamicStrByKey(SignKeys.NAME) + "_" + result.result.ToString().ToLower();
            sprChar.spriteName = sprName;
            sprCharFade.spriteName = sprName;
            var roleIdx = roleHost.GetDynamicIntByKey(SignKeys.ID);
            var curIdx = 1;
            switch (result.result)
            {
                case UpgradeResultType.Cool:
                    {
                        txtsCool.ToList().ForEach(txt =>
                        {
                            txt.gameObject.SetActive(curIdx++ == roleIdx);
                        });
                        txtsGreat.ToList().ForEach(txt =>
                        {
                            txt.gameObject.SetActive(false);
                        });
                        txtsPerfect.ToList().ForEach(txt =>
                        {
                            txt.gameObject.SetActive(false);
                        });
                    }
                    break;

                case UpgradeResultType.Great:
                    {
                        txtsCool.ToList().ForEach(txt =>
                        {
                            txt.gameObject.SetActive(false);
                        });
                        txtsGreat.ToList().ForEach(txt =>
                        {
                            txt.gameObject.SetActive(curIdx++ == roleIdx);
                        });
                        txtsPerfect.ToList().ForEach(txt =>
                        {
                            txt.gameObject.SetActive(false);
                        });
                    }
                    break;

                case UpgradeResultType.Perfect:
                    {
                        txtsCool.ToList().ForEach(txt =>
                        {
                            txt.gameObject.SetActive(false);
                        });
                        txtsGreat.ToList().ForEach(txt =>
                        {
                            txt.gameObject.SetActive(false);
                        });
                        txtsPerfect.ToList().ForEach(txt =>
                        {
                            txt.gameObject.SetActive(curIdx++ == roleIdx);
                        });
                    }
                    break;
            }
            for (int i = 0; i < itemTexs.Length; i++)
            {
                var tex = itemTexs[i];
                if (i < hosts.Length)
                {
                    tex.transform.parent.gameObject.SetActive(true);
                    var formulaHost = hosts[i];
                    ResourceLoader.Instance.LoadItemIcon(formulaHost, tex);
                }
                else
                {
                    tex.transform.parent.gameObject.SetActive(false);
                }
            }
            DOTweenUtils.Delay(() =>
            {
                gameObject.SetActive(false);
            }, 6.4f);
        }

        public override void BeCatched()
        {
            instance = this;
        }
    }
}