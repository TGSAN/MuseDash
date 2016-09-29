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
            cellBaseSelected.SetActive(isTo);
        }

        public override void OnShow()
        {
        }

        public override void OnHide()
        {
        }

        public override void OnShow(FormulaBase.FormulaHost host)
        {
            SetTexByHost(host);
            SetTxtByHost(host);
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
        }
    }
}