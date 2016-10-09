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

        public override void OnShow(FormulaBase.FormulaHost host)
        {
            this.host = host;
            SetTexByHost(host);
            SetTxtByHost(host);
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
                    FormulaBase.EquipManageComponent.Instance.Equip(curEquipList[0].GetDynamicIntByKey(FormulaBase.SignKeys.ID), false);
                }
                FormulaBase.EquipManageComponent.Instance.Equip(id);
                PnlChar.PnlChar.Instance.OnEquipLoad(PnlChar.PnlChar.Instance.curRoleIdx);
                PnlCharInfo.PnlCharInfo.Instance.UpdateItemList(id);
                PnlCharInfo.PnlCharInfo.Instance.GetComponent<Animator>().Play("pnl_items_choose_out");
            }));
        }

        private void SetTexByHost(FormulaBase.FormulaHost host)
        {
            var texName = host.GetDynamicStrByKey(FormulaBase.SignKeys.ICON);
            if (texName == null || ResourceLoader.Instance == null)
            {
                return;
            }
            ResourceLoader.Instance.Load(texName, resObj => texIcon.mainTexture = resObj as Texture);
        }

        private void SetTxtByHost(FormulaBase.FormulaHost host)
        {
            var name = host.GetDynamicStrByKey(FormulaBase.SignKeys.NAME);
            var lvl = host.GetDynamicStrByKey(FormulaBase.SignKeys.LEVEL);
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