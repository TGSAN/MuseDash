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
        public UIButton btnApply, btnUpgrade;

        public FormulaBase.FormulaHost host
        {
            private set;
            get;
        }

        private static ItemCellCharInfo instance = null;

        public static ItemCellCharInfo Instance
        {
            get
            {
                return instance;
            }
        }

        public void SetSelected(bool isTo)
        {
            if (cellBaseSelected != null)
            {
                cellBaseSelected.SetActive(isTo);
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
                var id = host.GetDynamicIntByKey(FormulaBase.SignKeys.ID);
                var curEquipList = FormulaBase.EquipManageComponent.Instance.GetGirlEquipHosts(PnlChar.PnlChar.Instance.curRoleIdx, PnlChar.PnlChar.Instance.curEquipTypeIdx, true);
                if (curEquipList.Length > 0)
                {
                    CommonPanel.GetInstance().ShowWaittingPanel(true);
                    FormulaBase.EquipManageComponent.Instance.Equip(
                        curEquipList[0].GetDynamicIntByKey(FormulaBase.SignKeys.ID), false,
                        result =>
                        {
                            if (result)
                            {
                                FormulaBase.EquipManageComponent.Instance.Equip(id, true, r =>
                                {
                                    if (r)
                                    {
                                        PnlChar.PnlChar.Instance.OnEquipLoad(PnlChar.PnlChar.Instance.curRoleIdx);
                                        PnlCharInfo.PnlCharInfo.Instance.UpdateItemList(id);
                                        CommonPanel.GetInstance().ShowWaittingPanel(false);
                                    }
                                });
                            }
                        });
                }
                else
                {
                    FormulaBase.EquipManageComponent.Instance.Equip(id, true, r =>
                    {
                        if (r)
                        {
                            PnlChar.PnlChar.Instance.OnEquipLoad(PnlChar.PnlChar.Instance.curRoleIdx);
                            PnlCharInfo.PnlCharInfo.Instance.UpdateItemList(id);
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
                        PnlEquipInfo.PnlEquipInfo.Instance.Play("item_upgrade_in");
                        PnlEquipInfo.PnlEquipInfo.Instance.isUpgrade = true;
                    }, 0.1f);
                }
            };
        }

        private void SetTexByHost()
        {
            var texName = host.GetDynamicStrByKey(FormulaBase.SignKeys.ICON);
            if (texName == null || ResourceLoader.Instance == null)
            {
                return;
            }
            ResourceLoader.Instance.Load("items/icon/" + texName, resObj => texIcon.mainTexture = resObj as Texture);
        }

        private void SetTxtByHost()
        {
            var name = host.GetDynamicStrByKey(FormulaBase.SignKeys.NAME);
            var lvl = "LV." + host.GetDynamicStrByKey(FormulaBase.SignKeys.LEVEL);
            var effect = host.GetDynamicStrByKey(FormulaBase.SignKeys.EFFECT_DESC);
            txtName.text = name;
            txtLvl.text = lvl;
            txtEffect.text = effect;
        }

        private void Start()
        {
            instance = this;
            InitEvent();
        }
    }
}