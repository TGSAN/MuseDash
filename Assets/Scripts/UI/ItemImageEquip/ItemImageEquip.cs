using FormulaBase;

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
        public UISprite sprSelected, sprOn;

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

        public FormulaBase.FormulaHost host
        {
            private set;
            get;
        }

        public bool isSelected
        {
            get { return sprSelected.gameObject.activeSelf; }
            set
            {
                sprSelected.gameObject.SetActive(value);
            }
        }

        public bool isUpgradeSelected
        {
            get { return sprOn.gameObject.activeSelf; }
            set
            {
                if (value)
                {
                    if (PnlSuitcase.PnlSuitcase.Instance.upgradeSelectedHost.Count < 3)
                    {
                        sprOn.gameObject.SetActive(true);
                        PnlSuitcase.PnlSuitcase.Instance.upgradeSelectedHost.Add(host);
                    }
                }
                else
                {
                    sprOn.gameObject.SetActive(false);
                    PnlSuitcase.PnlSuitcase.Instance.upgradeSelectedHost.Remove(host);
                }
            }
        }

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

        public override void OnShow(FormulaBase.FormulaHost h)
        {
            host = h;
            SetTexByHost();
            SetTxtByHost();
            UIEventListener.Get(gameObject).onClick = (go) =>
            {
                if (!PnlSuitcase.PnlSuitcase.Instance.isUpgrade)
                {
                    if (ItemManageComponent.Instance.IsEquipment(host))
                    {
                        PnlFoodInfo.PnlFoodInfo.Instance.OnExit();
                        PnlServantInfo.PnlServantInfo.Instance.OnExit();
                        PnlEquipInfo.PnlEquipInfo.Instance.OnShow(host);
                    }
                    else if (ItemManageComponent.Instance.isFood(host))
                    {
                        PnlEquipInfo.PnlEquipInfo.Instance.OnExit();
                        PnlServantInfo.PnlServantInfo.Instance.OnExit();
                        PnlFoodInfo.PnlFoodInfo.Instance.OnShow(host);
                    }
                    else
                    {
                        PnlEquipInfo.PnlEquipInfo.Instance.OnExit();
                        PnlFoodInfo.PnlFoodInfo.Instance.OnExit();
                        PnlServantInfo.PnlServantInfo.Instance.OnShow();
                    }
                    PnlSuitcase.PnlSuitcase.Instance.SetSelectedCell(h);
                }
                else
                {
                    OnUpgradeSelected();
                    PnlEquipInfo.PnlEquipInfo.Instance.OnUpgradeItemsRefresh();
                }
            };
        }

        public void OnSelected(bool selected)
        {
            sprSelected.gameObject.SetActive(selected);
        }

        public void OnUpgradeSelected()
        {
            if (PnlSuitcase.PnlSuitcase.Instance.isUpgrade)
            {
                isUpgradeSelected = !isUpgradeSelected;
            }
        }

        private void SetTxtByHost()
        {
            if (host == null)
            {
                txtLvl.text = "Level";
                txtType.text = "Type";
                return;
            }
            var lvl = host.GetDynamicIntByKey(FormulaBase.SignKeys.LEVEL);
            var type = host.GetDynamicStrByKey(FormulaBase.SignKeys.TYPE);
            txtLvl.text = lvl.ToString();
            txtType.text = type;
        }

        private void SetTexByHost()
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