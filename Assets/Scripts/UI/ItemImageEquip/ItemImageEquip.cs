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
        public UISprite sprSelected, sprOn, sprLock;

        public static ItemImageEquip Instance
        {
            get
            {
                return instance;
            }
        }

        public UILabel txtLvl, txtLv;
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
                PnlSuitcase.PnlSuitcase.Instance.SetLock(PnlSuitcase.PnlSuitcase.Instance.upgradeSelectedHost.Count >= 3);
            }
        }

        public bool isLock
        {
            get { return sprLock.gameObject.activeSelf; }
            set
            {
                sprLock.gameObject.SetActive(value);
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

        public void OnShow(string type)
        {
            txtType.text = type;
            txtLvl.gameObject.SetActive(false);
            txtLv.gameObject.SetActive(false);
            texIcon.gameObject.SetActive(false);
        }

        public override void OnShow(FormulaBase.FormulaHost h)
        {
            host = h;
            SetTexByHost();
            SetTxtByHost();
            UIEventListener.Get(gameObject).onClick = (go) =>
            {
                if (isLock)
                {
                    return;
                }
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
                    PnlCharInfo.PnlCharInfo.Instance.OnUpgradeItemsRefresh();
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
                txtLvl.gameObject.SetActive(false);
                txtLv.gameObject.SetActive(false);
                texIcon.gameObject.SetActive(false);
                return;
            }
            txtLvl.gameObject.SetActive(true);
            txtLv.gameObject.SetActive(true);
            texIcon.gameObject.SetActive(true);
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
            ResourceLoader.Instance.Load("items/icon/" + texName, resObj => texIcon.mainTexture = resObj as Texture);
        }
    }
}