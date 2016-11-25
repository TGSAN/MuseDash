using Assets.Scripts.Common.Manager;
using FormulaBase;
using LitJson;

/// UI分析工具自动生成代码
/// PnlCapsuleOpenUI主模块
///
using System;
using UnityEngine;

namespace PnlCapsuleOpen
{
    public class PnlCapsuleOpen : UIPhaseBase
    {
        public UILabel txtCharm, txtCharmMax;
        public UIButton btnOpen;
        public UITexture[] texItems;
        public GameObject[] capsules;
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

            for (var i = 0; i < curCapsule.itemsID.Count; i++)
            {
                var tex = texItems[i];
                var id = curCapsule.itemsID[i];
                var itemConfig = ItemManageComponent.Instance.GetItemConfigByUID(id);
                var iconPath = "items/icon/" + itemConfig["icon"].ToString();
                var quality = (int)itemConfig["quality"];
                ResourceLoader.Instance.Load(iconPath, res => tex.mainTexture = res as Texture);
                for (var j = 0; j < tex.transform.childCount; j++)
                {
                    var go = tex.transform.GetChild(j).gameObject;
                    go.SetActive((j + 1) == quality);
                }
            }
        }

        private void OnDisable()
        {
			if (PnlMainMenu.PnlMainMenu.Instance != null) {
				PnlMainMenu.PnlMainMenu.Instance.OnCharmUpdate();
			}
            
        }

        public override void BeCatched()
        {
            base.BeCatched();
            instance = this;
            btnOpen.onClick.Add(new EventDelegate(() =>
            {
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