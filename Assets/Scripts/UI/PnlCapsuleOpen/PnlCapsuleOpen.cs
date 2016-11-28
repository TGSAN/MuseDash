using Assets.Scripts.Common.Manager;
using FormulaBase;
using LitJson;

/// UI分析工具自动生成代码
/// PnlCapsuleOpenUI主模块
///
using System;
using System.Linq;
using UnityEngine;

namespace PnlCapsuleOpen
{
    public class PnlCapsuleOpen : UIPhaseBase
    {
        public UILabel txtCharm, txtCharmMax;
        public UIButton btnOpen, btnPurchase;
        public UITexture[] texItems;
        public GameObject[] capsules;
        public UIPlayAnimation[] playAnimations;
        private static PnlCapsuleOpen instance = null;

        public static PnlCapsuleOpen Instance
        {
            get
            {
                return instance;
            }
        }

        private void Start()
        {
        }

        public override void OnShow()
        {
            var curCapsule = CapsuleManager.instance.curCapsule;
            txtCharm.text = AccountCharmComponent.Instance.GetCharm().ToString();
            txtCharmMax.text = curCapsule.charmRequire.ToString();

            for (var i = 0; i < capsules.Length; i++)
            {
                capsules[i].SetActive(i == curCapsule.path);
            }

            texItems.ToList().ForEach(t => t.transform.parent.gameObject.SetActive(false));
            var commonItemCount = 0;
            for (int i = 0, k = 0; i < curCapsule.itemsID.Count; i++)
            {
                var id = curCapsule.itemsID[i];
                var itemConfig = ItemManageComponent.Instance.GetItemConfigByUID(id);
                if (ItemManageComponent.Instance.IsCommonItem(itemConfig["type"].ToString()))
                {
                    commonItemCount++;
                    var tex = texItems[k++];
                    tex.transform.parent.gameObject.SetActive(true);
                    var iconPath = "items/icon/" + itemConfig["icon"].ToString();
                    var quality = (int)itemConfig["quality"];
                    ResourceLoader.Instance.Load(iconPath, res => tex.mainTexture = res as Texture);
                    for (var j = 0; j < tex.transform.childCount; j++)
                    {
                        var go = tex.transform.GetChild(j).gameObject;
                        go.SetActive((j + 1) == quality);
                    }
                }
                else
                {
                }
            }
            playAnimations = btnOpen.gameObject.GetComponents<UIPlayAnimation>();
            playAnimations[0].clipName = "capsule_open_item" + commonItemCount.ToString();
            var isAnimate = AccountCharmComponent.Instance.GetCharm() >= curCapsule.charmRequire;
            playAnimations.ToList().ForEach(a => a.enabled = isAnimate);
        }

        private void OnDisable()
        {
            if (PnlMainMenu.PnlMainMenu.Instance != null)
            {
                PnlMainMenu.PnlMainMenu.Instance.OnCharmUpdate();
            }
        }

        public override void BeCatched()
        {
            base.BeCatched();
            instance = this;
            btnOpen.onClick.Add(new EventDelegate(() =>
            {
                if (AccountCharmComponent.Instance.GetCharm() < CapsuleManager.instance.curCapsule.charmRequire)
                {
                    btnPurchase.gameObject.SetActive(true);
                    btnPurchase.GetComponentInChildren<UILabel>().text = "fucker";
                }
                else
                {
                    playAnimations.ToList().ForEach(a => a.enabled = true);
                    CapsuleManager.instance.OpenCapsule((result) =>
                    {
                        if (result)
                        {
                            if (PnlSuitcase.PnlSuitcase.Instance != null)
                            {
                                PnlSuitcase.PnlSuitcase.Instance.UpdateSuitcase();
                            }
                        }
                    });
                }
            }));

            btnPurchase.onClick.Add(new EventDelegate(() =>
            {
                playAnimations.ToList().ForEach(a => a.enabled = true);
                CapsuleManager.instance.OpenCapsule((result) =>
                {
                    if (result)
                    {
                        if (PnlSuitcase.PnlSuitcase.Instance != null)
                        {
                            PnlSuitcase.PnlSuitcase.Instance.UpdateSuitcase();
                        }
                    }
                });
            }));
        }
    }
}