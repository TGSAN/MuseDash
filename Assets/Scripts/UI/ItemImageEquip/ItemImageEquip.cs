/// UI分析工具自动生成代码
/// ItemImageEquipUI主模块
///
using System;
using UnityEngine;

namespace ItemImageEquip
{
    public class ItemImageEquip : UIPhaseBase
    {
        private static ItemImageEquip instance = null;

        public static ItemImageEquip Instance
        {
            get
            {
                return instance;
            }
        }

        public UILabel txtLvl;
        public UITexture texIcon;
        public UILabel txtType;

        public string infoPanelName;

        private void Start()
        {
            instance = this;
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

        private void SetTxtByHost(FormulaBase.FormulaHost host)
        {
            if (host == null)
            {
                txtLvl.text = "Level";
                txtType.text = "Type";
                return;
            }
            var lvl = host.GetDynamicIntByKey(FormulaBase.SignKeys.LEVEL);
            var type = host.GetDynamicStrByKey(FormulaBase.SignKeys.TYPE);
            txtLvl.text = "LV." + lvl.ToString();
            txtType.text = type;
        }

        private void SetTexByHost(FormulaBase.FormulaHost host)
        {
            if (host == null)
            {
                texIcon.mainTexture = null;
                return;
            }
            string texName = host.GetDynamicStrByKey(FormulaBase.SignKeys.ICON);
            if (texName == null || ResourceLoader.Instance == null)
            {
                return;
            }
            ResourceLoader.Instance.Load(texName, resObj =>
            {
                if (resObj != null)
                {
                    texIcon.mainTexture = resObj as Texture;
                }
            });
        }
    }
}