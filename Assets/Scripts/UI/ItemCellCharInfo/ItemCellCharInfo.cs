using Assets.Scripts.Common;
using DG.Tweening;
using FormulaBase;

/// UI分析工具自动生成代码
/// ItemCellCharInfoUI主模块
///
using System;
using UnityEngine;

namespace ItemCellCharInfo
{
    public class ItemCellCharInfo : UIPhaseBase
    {
        public GameObject cellBaseSelected;
        public UILabel txtName, txtEffect, txtLvl;
        public UITexture texIcon;
        public UISprite sprBkg;
        public UIButton btnApply, btnUpgrade;
        public Color vigourColor, staminaColor, strColor;

        public FormulaBase.FormulaHost host
        {
            private set;
            get;
        }

        public bool isSelected
        {
            get { return cellBaseSelected.activeSelf; }
            set
            {
                if (cellBaseSelected != null)
                {
                    cellBaseSelected.SetActive(value);
                }
            }
        }

        private static ItemCellCharInfo instance = null;

        public static ItemCellCharInfo Instance
        {
            get
            {
                return instance;
            }
        }

        public override void OnShow()
        {
            InitEvent();
        }

        public override void OnHide()
        {
        }

        public override void OnShow(FormulaBase.FormulaHost h)
        {
            this.host = h;
            SetTexByHost();
            SetTxtByHost();
        }

        private void InitEvent()
        {
            btnApply.onClick.Clear();
            btnApply.onClick.Add(new EventDelegate(() =>
            {
                var bagID = host.GetDynamicIntByKey(FormulaBase.SignKeys.BAGINID);
                var curEquipList = FormulaBase.EquipManageComponent.Instance.GetGirlEquipHosts(PnlChar.PnlChar.Instance.curRoleIdx, PnlChar.PnlChar.Instance.curEquipTypeIdx, true);
                if (curEquipList.Length > 0)
                {
                    CommonPanel.GetInstance().ShowWaittingPanel(true);
                    FormulaBase.EquipManageComponent.Instance.Equip(
                        curEquipList[0].GetDynamicIntByKey(FormulaBase.SignKeys.BAGINID), false,
                        result =>
                        {
                            if (result)
                            {
                                FormulaBase.EquipManageComponent.Instance.Equip(bagID, true, r =>
                                {
                                    if (r)
                                    {
                                        PnlChar.PnlChar.Instance.OnEquipLoad(PnlChar.PnlChar.Instance.curRoleIdx);
                                        PnlCharInfo.PnlCharInfo.Instance.UpdateItemList(bagID);
                                        CommonPanel.GetInstance().ShowWaittingPanel(false);
                                    }
                                });
                            }
                        });
                }
                else
                {
                    FormulaBase.EquipManageComponent.Instance.Equip(bagID, true, r =>
                    {
                        if (r)
                        {
                            PnlChar.PnlChar.Instance.OnEquipLoad(PnlChar.PnlChar.Instance.curRoleIdx);
                            PnlCharInfo.PnlCharInfo.Instance.UpdateItemList(bagID);
                            CommonPanel.GetInstance().ShowWaittingPanel(false);
                        }
                    });
                }
            }));
            UIEventListener.Get(gameObject).onClick = (go) =>
            {
                PnlEquipInfo.PnlEquipInfo.Instance.OnShow(host);
            };

            UIEventListener.Get(btnUpgrade.gameObject).onClick = (go) =>
            {
                if (UpgradeManager.instance.IsItemLvlMax(host))
                {
                    CommonPanel.GetInstance().ShowText("物品已达最高等级，无法升级");
                }
                else
                {
                    PnlEquipInfo.PnlEquipInfo.Instance.OnShow(host);
                    DOTweenUtils.Delay(() =>
                    {
                        PnlEquipInfo.PnlEquipInfo.Instance.isUpgrade = true;
                        PnlEquipInfo.PnlEquipInfo.Instance.Play("item_upgrade_in");
                    }, 0.1f);
                }
            };
        }

        private void SetTexByHost()
        {
            var texName = host.GetDynamicStrByKey(FormulaBase.SignKeys.ICON);
            var quality = host.GetDynamicIntByKey(SignKeys.QUALITY);
            sprBkg.spriteName = "groove_" + quality.ToString();
            if (texName == null || ResourceLoader.Instance == null)
            {
                return;
            }
            ResourceLoader.Instance.LoadItemIcon(host, texIcon);
        }

        private void SetTxtByHost()
        {
            var name = host.GetDynamicStrByKey(FormulaBase.SignKeys.NAME);
            var lvl = "LV." + host.GetDynamicStrByKey(FormulaBase.SignKeys.LEVEL);

            txtName.text = name;
            txtLvl.text = lvl;

            var vigour = (int)host.Result(FormulaKeys.FORMULA_50);
            var stamina = (int)host.Result(FormulaKeys.FORMULA_51);
            var strengh = (int)host.Result(FormulaKeys.FORMULA_56);
            if (vigour > 0)
            {
                txtEffect.text = "Vigour +" + vigour.ToString();
                txtEffect.color = vigourColor;
            }
            if (stamina > 0)
            {
                txtEffect.text = "Stamina +" + stamina.ToString();
                txtEffect.color = staminaColor;
            }
            if (strengh > 0)
            {
                txtEffect.text = "Strengh +" + strengh.ToString();
                txtEffect.color = strColor;
            }
        }

        private void Start()
        {
            instance = this;
            InitEvent();
        }
    }
}